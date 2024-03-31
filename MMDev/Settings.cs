using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMDev
{
    internal static class Settings
    {

        static public int year_run = 2023;
        static public string metric_name = string.Empty;
        static public List<string> metric_names = new List<string>() { "KPI", "NET", "POM", "SAG", "SOR", "TR", "WAB" };
        static public int model_iterates = 10000;
    }
}
