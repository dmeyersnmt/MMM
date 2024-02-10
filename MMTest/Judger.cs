using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMTest
{
    internal class Judger
    {
        private MSSQL mssql { get; set; }
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public Judger()
        {
            mssql = new MSSQL("Server=localhost; Database=MMTEST; Integrated Security=True;");
            var dt_num = GetModelNumbers();
            DataTable dt_modelresults = new DataTable();
            dt_modelresults.Columns.Add("MODEL_ID", typeof(int));
            dt_modelresults.Columns.Add("SCORE", typeof(int));
            foreach(DataRow row in dt_num.Rows)
            {
                int run_num = (int)row["MODEL_ID"];
                var dt = GetResultsTable(run_num);
                int score = CalculateScore(dt);
                logger.Debug($"Total score for model {run_num}: {score}");
                dt_modelresults.Rows.Add(new object[] {run_num, score});
                //InsertModelResults(run_num, score);
            }
            InsertModelResultsBulk(dt_modelresults);
        }


        private DataTable GetModelNumbers()
        {
            string query = "SELECT * FROM MODELS WHERE SCORE IS NULL";
            return mssql.SelectedValues(query);
        }


        private DataTable GetResultsTable(int run_num)
        {
            string query = @$"SELECT A.GAME_ID, A.RESULTWINNER, B.MODELWINNER, C.POINT FROM
                            (SELECT GAME_ID, WINNER AS RESULTWINNER FROM RESULT) AS A
                            INNER JOIN 
                            (SELECT GAME_ID, WINNER AS MODELWINNER FROM MODELRESULT WHERE RUN_ID = {run_num}) AS B 
                            ON A.GAME_ID = B.GAME_ID 
                            INNER JOIN 
                            (SELECT GAME_ID, POINT FROM POINTS) AS C
                            ON A.GAME_ID = C.GAME_ID";

            return mssql.SelectedValues(query);
        }


        private void InsertModelResults(int model_id, int score)
        {
            logger.Trace("Inserting resulting scores into MODELS");
            string query = $"UPDATE MODELS SET SCORE = {score} WHERE MODEL_ID = {model_id}";
            mssql.ExecuteNonQuery(query);
        }


        private void InsertModelResultsBulk(DataTable dt)
        {
            logger.Debug("Bulk Inserting resulting scores into MODELS");
            mssql.BulkCopyScores("MODELS", dt);

        }

        private int CalculateScore(DataTable dt)
        {
            int total_score = 0;
            foreach(DataRow row in dt.Rows)
            {
                int result_winner = (int)row["RESULTWINNER"];
                int model_winner = (int)row["MODELWINNER"];
                int points = (int)row["POINT"];

                if(result_winner == model_winner)
                {
                    total_score += points;
                }
            }
            return total_score;
        }
    }
}
