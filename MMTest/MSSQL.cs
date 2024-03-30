using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Data.SqlClient;
using NLog;

namespace MMTest
{
    public class MSSQL
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        string connectionString { get; set; }

        public MSSQL(string connectionString)
        {
            this.connectionString = connectionString;

        }


        public DataTable SelectedValues(string query)
        {

            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                try
                {
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, conn);
                    sqlDataAdapter.Fill(dt);
                }
                catch (SqlException ex)
                {
                    logger.Error(ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
            return dt;
        }

        public void ExecuteNonQuery(string query)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                try
                {
                    SqlCommand command = new SqlCommand(query, conn);
                    command.CommandTimeout = 600;
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    logger.Error(ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void BulkCopy(string tableName, DataTable dt)
        {
            try
            {
                using (var copy = new SqlBulkCopy(connectionString))
                {
                    copy.DestinationTableName = tableName;
                    copy.ColumnMappings.Add(1, 1);
                    copy.ColumnMappings.Add(2, 2);
                    copy.ColumnMappings.Add(3, 3);
                    copy.ColumnMappings.Add(4, 4);
                    copy.ColumnMappings.Add(5, 5);
                    copy.WriteToServer(dt);
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
        }

        public void BulkCopyScores(string tableName, DataTable dt)
        {
            try
            {
                using (var copy = new SqlBulkCopy(connectionString))
                {
                    copy.DestinationTableName = tableName;
                    copy.ColumnMappings.Add(0, 0);
                    copy.ColumnMappings.Add(1, 1);
                    copy.WriteToServer(dt);
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
        }
    }
}
