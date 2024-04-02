using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMTools;

namespace MMDev
{
    internal class Model
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private SQLTools.MSSQL mssql;
        private DataTable dt_seeds;
        private DataTable dt_initial;
        
        public Model(int iterations, SQLTools.MSSQL mssql)
        {

            this.mssql = mssql;
            logger.Debug($"Connection string:{mssql.connection_string}");
            //get the model number
            int model_number = GetModelNumber();
            logger.Info($"Module Number start: {model_number} on database:{mssql.database_name}");
            //get the initial bracket setup
            dt_initial = new DataTable();
            dt_initial = RetreiveInitialBracketSetup();
            //get the seeds
            dt_seeds = RetreiveTeamSeeds();
            for (int i = 0; i < iterations; i++)
            {
                logger.Trace($"Current model number:{model_number}");
                InsertModelNumber(model_number);
                StartModel(model_number);
                model_number++;
            }
        }

        /// <summary>
        /// Find the next available model number  
        /// </summary>
        /// <returns></returns>
        private int GetModelNumber()
        {
            string query = "SELECT MAX(MODEL_ID) FROM MODELS";
            var dt = mssql.SelectedValues(query);
            int model_number;
            if (dt.Rows[0][0].GetType().Name == "DBNull")
            {
                model_number = 0;
            }
            else
            {
                model_number = Convert.ToInt32(dt.Rows[0][0]);
            }
            model_number += 1;
            return model_number;
        }

        /// <summary>
        /// Select the model_type_id from the table so that it can be added to the models table
        /// TODO:maybe do this at the end?
        /// </summary>
        /// <returns>The int that corresponds with the model name</returns>
        private int GetModelTypeID()
        {
            string query = $"SELECT MODEL_TYPE_ID FROM MODEL_TYPE WHERE MODEL_NAME = '{Settings.metric_name}'";
            var dt = mssql.SelectedValues(query);
            
            int model_number = Convert.ToInt32(dt.Rows[0][0]);
            
            return model_number;
        }


      
        /// <summary>
        /// Insert the model number into the MODELS table
        /// </summary>
        /// <param name="model_number"></param>
        private void InsertModelNumber(int model_number)
        {
            int mode_type_id = GetModelTypeID();
            string query = $"INSERT INTO MODELS (MODEL_ID, MODEL_TYPE_ID, YEAR) VALUES ({model_number}, {mode_type_id}, {Settings.year_run})";
            mssql.ExecuteNonQuery(query);
            logger.Debug($"Insert model number {model_number} into MODELS");
        }

        /// <summary>
        /// Start the model run by going through each game and finding who wins and advances
        /// </summary>
        /// <param name="model_number"></param>
        public void StartModel(int model_number)
        {
            DataTable dt = new DataTable();
            dt.Clear();
            dt = dt_initial.Copy();
            foreach (DataRow dr in dt.Rows)
            {
                int game_id = (int)dr["GAME_ID"];
                int opponent1 = (int)dr["OPPONENT1"];
                int opponent2 = (int)dr["OPPONENT2"];
                //Decision decider = new Decision(opponent1, opponent2, dt_seeds, mssql);
                //int winner = decider.RankWeighterUpsetter();
                
                int winner = Rank.ReturnWinner(opponent1, opponent2, mssql);
                logger.Trace($"Winner for GAME_ID:{game_id} = {winner}");
                DataRow? winnerGame = dt.Select($"GAME_ID={game_id}").FirstOrDefault();
                winnerGame["WINNER"] = winner;

                //Set the opponents of the next game
                //as long as is it isn't the last game (63)
                if (game_id != 63)
                {
                    int next_game = (int)dr["NEXT_GAME"];
                    DataRow? nextGame = dt.Select($"GAME_ID={next_game}").FirstOrDefault();
                    if (nextGame["OPPONENT1"].GetType().Name == "DBNull")
                    {
                        nextGame["OPPONENT1"] = winner;
                    }
                    else
                    {
                        nextGame["OPPONENT2"] = winner;
                    }
                }
            }
            dt.Columns["MODEL_ID"].Expression = $"{model_number}";
            mssql.BulkCopy("MODELRESULT", dt);
        }

        /// <summary>
        /// Get a datatable that shows the initial bracket setup joined with game ids
        /// </summary>
        /// <returns></returns>
        public DataTable RetreiveInitialBracketSetup()
        {

            string query = @"SELECT B.*, A.NEXT_GAME FROM 
                                GAME AS A
                            INNER JOIN 
                                INTIALRESULT AS B 
                            ON A.GAME_ID = B.GAME_ID";
            DataTable dt = mssql.SelectedValues(query);
            logger.Trace($"Row count: {dt.Rows.Count}");
            return dt;
        }

        /// <summary>
        /// Get a datatable that contains the seeding for every team
        /// We only need this if we utilize the seed method
        /// </summary>
        /// <returns></returns>
        public DataTable RetreiveTeamSeeds()
        {
            string query = "SELECT TEAM_ID, SEED, AVG_RANK FROM TEAM";
            DataTable dt = mssql.SelectedValues(query);
            return dt;
        }
    }
}
