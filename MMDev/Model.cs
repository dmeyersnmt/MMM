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
        private string database;
        public Model(string database, int iterations)
        {
            this.database = database;
            mssql = new SQLTools.MSSQL($"Server=localhost; Database={database}; Integrated Security=True;");
            //get the model number
            int model_number = GetModelNumber();
            logger.Info($"Module Number start: {model_number} on database: {database}");
            //get the initial bracket setup
            dt_initial = new DataTable();
            dt_initial = RetreiveInitialBracketSetup();
            //get the seeds
            dt_seeds = RetreiveTeamSeeds();
            for (int i = 0; i < iterations; i++)
            {
                logger.Debug($"Current model number:{model_number}");
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
        /// Insert the model number into the MODELS table
        /// </summary>
        /// <param name="model_number"></param>
        private void InsertModelNumber(int model_number)
        {
            string query = $"INSERT INTO MODELS (MODEL_ID) VALUES ({model_number})";
            mssql.ExecuteNonQuery(query);
            logger.Trace($"Insert model number {model_number} into MODELS");
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
                Decision decider = new Decision(database, opponent1, opponent2, dt_seeds);
                //
                //Find the winner of the game
                //
                //int winner = decider.SeedWeighted();
                //int winner = decider.SeedWeightedUpsetter();
                int winner = decider.RankWeighterUpsetter();
                logger.Trace($"Winner for GAME_ID:{game_id} = {winner}");
                DataRow? winnerGame = dt.Select($"GAME_ID={game_id}").FirstOrDefault();
                winnerGame["WINNER"] = winner;

                //Set the opponents of the next game
                if (game_id != 67)
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
            dt.Columns["RUN_ID"].Expression = $"{model_number}";
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
        /// Get a datatable that contains the seeding for every time
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
