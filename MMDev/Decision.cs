using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMTools;

namespace MMDev
{
    internal class Decision
    {
        public int winner { get; set; }
        private SQLTools.MSSQL mssql;
        private int opponent1;
        private int opponent2;
        private DataTable dt_seeds;
        public Decision(int opponent1, int opponent2, DataTable dt_seeds, SQLTools.MSSQL mssql)
        {
            this.dt_seeds = dt_seeds;
            this.opponent1 = opponent1;
            this.opponent2 = opponent2;
            this.mssql = mssql;
        }

        public int RandomDecider()
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
        /// Decide the winner by comparing the difference in seed strenght with a random deciding factor
        /// </summary>
        /// <returns></returns>
        public int SeedWeighted()
        {
            int seed1 = GetSeedDT(opponent1);
            int seed2 = GetSeedDT(opponent2);

            int seed_total = seed1 + seed2;
            double opponent1_weight = (double)1 - ((double)seed1 / (double)seed_total);
            double opponent2_weight = (double)1 - ((double)seed2 / (double)seed_total);

            Random random = new Random();
            double result = random.NextDouble();

            if (opponent1_weight == opponent2_weight)
            {
                return RandomDecider();
            }
            else
            {
                if (opponent1_weight > result)
                {
                    return opponent1;
                }
                else
                {
                    return opponent2;
                }

            }
        }

        public int SeedWeightedUpsetter()
        {
            int seed1 = GetSeedDT(opponent1);
            int seed2 = GetSeedDT(opponent2);

            int seed_total = seed1 + seed2;
            double opponent1_weight = (double)1 - ((double)seed1 / (double)seed_total);
            double opponent2_weight = (double)1 - ((double)seed2 / (double)seed_total);

            Random random = new Random();
            double result = random.NextDouble();

            if (opponent1_weight == opponent2_weight)
            {
                return RandomDecider();
            }
            else
            {
                if (opponent1_weight > result)
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
                    return opponent2;
                }

            }
        }

        public int RankWeighterUpsetter()
        {
            double rank1 = GetRankDT(opponent1);
            double rank2 = GetRankDT(opponent2);

            double rank_total = rank1 + rank2;
            double opponent1_weight = (double)1 - ((double)rank1 / (double)rank_total);
            double opponent2_weight = (double)1 - ((double)rank2 / (double)rank_total);


            Random random = new Random();
            double result = random.NextDouble();


            if (opponent1_weight == opponent2_weight)
            {
                return RandomDecider();
            }
            else
            {
                if (opponent1_weight > result)
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
                    return opponent2;
                }
            }
        }




        /// <summary>
        /// Get the seed from TEAM based on TEAM_ID 
        /// </summary>
        /// <param name="opponent"></param>
        /// <returns></returns>
        public int GetSeed(int opponent)
        {
            string query = $"SELECT SEED FROM TEAM WHERE TEAM_ID = {opponent}";
            DataTable dt = mssql.SelectedValues(query);
            var result = Convert.ToInt32(dt.Rows[0]["SEED"]);
            return result;
        }


        public int GetSeedDT(int opponent)
        {
            DataRow? seed_row = dt_seeds.Select($"TEAM_ID={opponent}").FirstOrDefault();
            int seed = Convert.ToInt32(seed_row["SEED"]);
            return seed;
        }


        public double GetRankDT(int opponent)
        {
            DataRow? row = dt_seeds.Select($"TEAM_ID={opponent}").FirstOrDefault();
            double rank = Convert.ToDouble(row["AVG_RANK"]);
            return rank;
        }


        
    }
}
