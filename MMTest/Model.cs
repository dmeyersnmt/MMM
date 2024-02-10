using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Runtime.InteropServices;

namespace MMTest
{
    public class Model
    {

        private static Logger logger = LogManager.GetCurrentClassLogger();
        private MSSQL mssql;
        public Model(int iterations) 
        {
            mssql = new MSSQL("Server=localhost; Database=MMTEST; Integrated Security=True;");
            int model_number = GetModelNumber();  
            for(int i=0;i<iterations;i++)
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
            DataTable dt = RetreiveTables();
            foreach(DataRow dr in dt.Rows)
            {
                int game_id = (int)dr["GAME_ID"];
                int opponent1 = (int)dr["OPPONENT1"];
                int opponent2 = (int)dr["OPPONENT2"];
                Decision decider = new Decision(opponent1, opponent2);
                int winner = decider.SeedWeighted();
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


        public DataTable RetreiveTables()
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
    }
}
