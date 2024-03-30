using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MMTest
{
    public class JudgeTester
    {
        /// <summary>
        /// New way of Judging the models, utilizing SQL functions instead of iterating via loops
        /// </summary>
        private MSSQL mssql;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public JudgeTester(string database)
        {
            
            mssql = new MSSQL($"Server=localhost; Database={database}; Integrated Security=True;");
            logger.Debug($"Started Judgetester on {database}");
            var dt = GetResultPoints();
            IterateResults(dt);
            logger.Debug("Finished Judgetester");

        }


        private DataTable GetResultPoints()
        {
            string query = @$"SELECT T1.GAME_ID, T1.WINNER, T2.POINT FROM
                            (SELECT GAME_ID, WINNER FROM [RESULT] WHERE WINNER IS NOT NULL AND SCORE_ADDED = 0) AS T1 
                            INNER JOIN
                            (SELECT GAME_ID, POINT FROM [POINTS]) AS T2 
                            ON T1.GAME_ID = T2.GAME_ID";
            return mssql.SelectedValues(query);
        }
            

        private void IterateResults(DataTable dt)
        {
            foreach(DataRow row in dt.Rows)
            {
                int points = (int)row["POINT"];
                int game_id = (int)row["GAME_ID"];
                int winner_id = (int)row["WINNER"];

                string query = $@"UPDATE t1
                    SET t1.SCORE = t1.SCORE + {points}
                    FROM MODELS as t1
                    INNER JOIN(SELECT * FROM MODELRESULT WHERE GAME_ID = {game_id} AND WINNER = {winner_id}) as t2
                    ON t1.MODEL_ID = t2.RUN_ID";

                logger.Debug($"Adding score for game: {game_id}");
                mssql.ExecuteNonQuery(query);
                string update_query = $"UPDATE RESULT SET SCORE_ADDED = 1 WHERE GAME_ID = {game_id}";
                mssql.ExecuteNonQuery(update_query);
            }
        }
    }
}
