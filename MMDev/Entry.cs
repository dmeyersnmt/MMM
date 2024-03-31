using MMTools;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MMDev
{
    internal class Entry
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private string database_name = "MMDEV";
        

        public SQLTools.MSSQL mssql;

        public Entry()
        {

            SQLTools.ConnectionSettings conn = new SQLTools.ConnectionSettings("MMDEV", "localhost", null);
            mssql = new SQLTools.MSSQL(conn);
            logger.Trace($"Entry mssql: {conn.connectionString}");
            RunModels();
        }

        public void RunModels()
        {
            int iterations = Settings.model_iterates;
            DateTime startTime = DateTime.Now;
            var timer = new Stopwatch();
            timer.Start();
            logger.Info($"Start at {startTime}");
            logger.Info($"Model iterations: {iterations}");
            //Run the models
            Model model = new Model(iterations, mssql);
            timer.Stop();
            logger.Info($"Model runtime: {timer.Elapsed}");
        }
    }
}
