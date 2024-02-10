using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMTest
{
    internal class Entry
    {
        /// <summary>
        /// Entry point into the program
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public Entry()
        {
            int iterations = 10000;
            DateTime startTime = DateTime.Now;
            var timer = new Stopwatch();
            timer.Start();
            logger.Info($"Start at {startTime}");
            logger.Info($"Model iterations: {iterations}");
            Model model = new Model(iterations);
            timer.Stop();
            logger.Info($"Model runtime: {timer.Elapsed}");
            timer.Restart();
            Judger judger = new Judger();
            logger.Info($"Judger runtime: {timer.Elapsed}");
            DateTime endTime =DateTime.Now;
            logger.Info($"Finished at {endTime}");
        }
    }
}
