using MMTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMTools;
using System.Reflection.Metadata;
using NLog;

namespace MMDev
{
    public static class Rank
    {
        /// <summary>
        /// Figure out the winner of a game based on various rankings
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();
        static bool use_upset = false;

        /// <summary>
        /// Random decider, basically a coin flip between the two opponents
        /// used for when the rankings are similar
        /// </summary>
        /// <param name="opponent1"></param>
        /// <param name="opponent2"></param>
        /// <returns></returns>
        public static int RandomDecider(int opponent1, int opponent2)
        {
            Random random = new Random();
            int result = random.Next(1, 10);
            if (result < 5)
            {
                return opponent1;
            }
            else
            {
                return opponent2;
            }
        }


        /// <summary>
        /// Find the winner based on the rank of each opponent
        /// </summary>
        /// <param name="opponent1"></param>
        /// <param name="opponent2"></param>
        /// <param name="mssql"></param>
        /// <returns></returns>
        public static int ReturnWinner(int opponent1, int opponent2, SQLTools.MSSQL mssql)
        {
            Random random = new Random();
            double result = random.NextDouble();

            var opp1_rank = GetRankOfOpponent(opponent1, mssql);
            var opp2_rank = GetRankOfOpponent(opponent2, mssql);

            double rank_total = opp1_rank + opp2_rank;
            double opponent1_weight = (double)1 - ((double)opp1_rank / (double)rank_total);
            double opponent2_weight = (double)1 - ((double)opp2_rank / (double)rank_total);

            if (opponent1_weight == opponent2_weight)
            {
                return RandomDecider(opponent1, opponent2);
            }
            else
            {
                if (opponent1_weight > result)
                {
                    if(use_upset)
                    {
                        //use an random upsetter that changes the outcome of the seeded victor
                        Random upsetter = new Random();
                        int upset = upsetter.Next(0, 10);
                        if (upset < 2)
                        {
                            return opponent2;
                        }
                        else
                        {
                            return opponent1;
                        }
                    }
                    else
                    {
                        return opponent1;
                    }
                }
                else
                {
                    return opponent2;
                }
            }
        }

        /// <summary>
        /// Get the rank of the opponent 
        /// </summary>
        /// <param name="opponent"></param>
        /// <param name="mssql"></param>
        /// <returns></returns>
        private static double GetRankOfOpponent(int opponent, SQLTools.MSSQL mssql)
        {
            logger.Info($"Using the metric setting of: {Settings.metric_name}");
            string query = $"SELECT {Settings.metric_name} as rank FROM RANKING WHERE TEAM_ID = {opponent}";
            DataTable dt = mssql.SelectedValues(query);
            return Convert.ToDouble(dt.Rows[0]["rank"]);
        }
    }
}
