using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace MMTest
{
    internal class Decision
    {
        public int winner { get; set; }
        private MSSQL mssql;
        private int opponent1;
        private int opponent2;
        public Decision(int opponent1, int opponent2)
        {
            this.opponent1 = opponent1;
            this.opponent2 = opponent2;
            mssql = new MSSQL("Server=localhost; Database=MMTEST; Integrated Security=True;");
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
            int seed1 = GetSeed(opponent1);
            int seed2 = GetSeed(opponent2);

            int seed_total = seed1 + seed2;
            double opponent1_weight = (double)1 -((double)seed1/(double)seed_total);
            double opponent2_weight = (double)1 - ((double)seed2 / (double)seed_total);
                
            Random random = new Random();
            double result = random.NextDouble();

            if(opponent1_weight == opponent2_weight) 
            {
                return RandomDecider();
            }
            else
            {
                if(opponent1_weight > result)
                {
                    return opponent1;
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



    }
}
