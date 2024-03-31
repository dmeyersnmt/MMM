using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using System.Data.SqlClient;

namespace MMTools
{
    public class SQLTools
    {
        public class MSSQL
        {
            private static Logger logger = LogManager.GetCurrentClassLogger();
            public string server_name { get; set; }
            public string database_name { get; set; }
            public string connection_string { get; set; }

            public MSSQL(ConnectionSettings connection_settings)
            {
                this.server_name = connection_settings.server_name;
                this.database_name = connection_settings.database_name;
                this.connection_string = connection_settings.connectionString.ToString();
                //if (connection_settings.connectionString is not string)
                //{
                //    this.connectionString = $"Server={server_name}; Database={database_name}; Integrated Security=True;";
                //}
                //else
                //{
                //    this.connectionString = connection_settings.connectionString.ToString();
                //}
                

            }


            public DataTable SelectedValues(string query)
            {

                DataTable dt = new DataTable();
                using (SqlConnection conn = new SqlConnection(connection_string))
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
                using (SqlConnection conn = new SqlConnection(connection_string))
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
                    using (var copy = new SqlBulkCopy(connection_string))
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
                    using (var copy = new SqlBulkCopy(connection_string))
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
        
        public class ConnectionSettings
        {
            public string database_name;
            public string server_name;
            public string connectionString { get; set; } = string.Empty;

            public ConnectionSettings(string database_name, string server_name, object connection_string)
            {
                this.database_name = database_name;
                this.server_name = server_name;
                if(connection_string is not null )
                {
                    this.connectionString = (string)connection_string;
                }
                else
                {
                    this.connectionString = $"Server={server_name}; Database={database_name}; Integrated Security=True;";
                }
                
            }
        }
    }
}
