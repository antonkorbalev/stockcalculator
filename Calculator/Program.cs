using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FortsRobotLib.Genetics;
using FortsRobotLib.CandleProviders;
using FortsRobotLib;
using BasicAlgorithms;
using System.IO;

namespace Calculator
{
    class Program
    {
        static int num;
        static string fName = "Results.txt";
        static void Main(string[] args)
        {
            var lowDate = new DateTime(2017, 1, 11);
            var highDate = new DateTime(2017, 8, 20);
            if (File.Exists(fName))
                File.Delete(fName);
            for (var i = 2; i < 100; i = i + 2)
            {
                using (var provider = new FinamCandleProvider("SPFB.SI", TimePeriod.Hour,
                "14", "19899", lowDate, highDate))
                {
                    num = i;
                    File.AppendAllLines(fName, new string[] { num.ToString(), "Population;Mean profit;Max profit;" });
                    Console.WriteLine("Length {0}:", i);
                    Console.WriteLine("----------------------");
                    provider.Initialize();
                    var genSelector = new GeneticsSelector<FinamCandleProvider, BasicAlgorithm>(provider, 3, 100, i);
                    genSelector.PopulationCompleted += GenSelector_PopulationCompleted;
                    genSelector.Select(30);
                    genSelector.Wait();
                }
            }
        }

        private static void GenSelector_PopulationCompleted(object sender, PopulationCompletedEventArgs e)
        {
            var mean = e.Results.Select(o => o.Profit).Sum() / e.Results.Length;
            Console.WriteLine("{0}: iteration {1}, mean profit {2}, max profit = {3}", num, e.PopulationIndex, mean, e.Results.Max(o => o.Profit));
            File.AppendAllLines(fName, new string[] { string.Format("{0};{1};{2}", e.PopulationIndex, mean, e.Results.Max(o => o.Profit)) });
        }
    }
}
