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
            //RunModels();
            //RunJudger();
            JudgeTester judgeTester = new JudgeTester("MMTOURNAMENT");
            JudgeTester judgeTester2 = new JudgeTester("MMPROD");

        }


        public void RunModels()
        {
            int iterations = 3000000;
            DateTime startTime = DateTime.Now;
            var timer = new Stopwatch();
            timer.Start();
            logger.Info($"Start at {startTime}");
            logger.Info($"Model iterations: {iterations}");
            //Run the models
            Model model = new Model("MMTOURNAMENT", iterations);
            timer.Stop();
            logger.Info($"Model runtime: {timer.Elapsed}");
        }

        public void RunJudger()
        {
            //Jude the Models
            DateTime startTime = DateTime.Now;
            var timer = new Stopwatch();
            timer.Start();
            logger.Info($"Start at {startTime}");           
            Judger judger = new Judger("MMTOURNAMENT");
            logger.Info($"Judger runtime: {timer.Elapsed}");
            DateTime endTime = DateTime.Now;
            logger.Info($"Finished at {endTime}");
        }
    }
}
