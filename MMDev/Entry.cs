﻿using NLog;
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
        public Entry()
        {
            RunModels();
        }

        public void RunModels()
        {
            int iterations = 100;
            DateTime startTime = DateTime.Now;
            var timer = new Stopwatch();
            timer.Start();
            logger.Info($"Start at {startTime}");
            logger.Info($"Model iterations: {iterations}");
            //Run the models
            Model model = new Model(database_name, iterations);
            timer.Stop();
            logger.Info($"Model runtime: {timer.Elapsed}");
        }
    }
}
