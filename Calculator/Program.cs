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
using FortsRobotLib.ProviderDataCache;
using Calculator.Properties;

namespace Calculator
{
    class Program
    {
        static int num;
        static void Main(string[] args)
        {
            if (File.Exists(Settings.Default.ResultsFileName))
                File.Delete(Settings.Default.ResultsFileName);
            MemoryCache<FinamCandleProvider> cache;
            using (var provider = new FinamCandleProvider(Settings.Default.InsName, Settings.Default.TimePeriod,
                Settings.Default.MarketCode, Settings.Default.InsCode, Settings.Default.DateFrom, Settings.Default.DateTo))
            {
                cache = new MemoryCache<FinamCandleProvider>(provider);
            }
            File.AppendAllLines(Settings.Default.ResultsFileName, new string[] { "Parameters;Populations total;Sharp Index;Profit;Mean profit per deal; Success %;" });
            for (var i = Settings.Default.ParamsCountFrom; i < Settings.Default.ParamsCountTo; i = i + 2)
            {
                num = i;
                Console.WriteLine("Length {0}:", i);
                Console.WriteLine("----------------------");
                var genSelector = new GeneticsSelector<FinamCandleProvider, BasicAlgorithm>(cache, 3, 100, i, generationSize: Settings.Default.GenerationSize, threadsNum: Settings.Default.ThreadsCount);
                genSelector.PopulationCompleted += GenSelector_PopulationCompleted;
                genSelector.Select(Settings.Default.PopulationsCount);
                genSelector.Wait();
                var result = genSelector.GetBestResults().First();
                File.AppendAllLines(Settings.Default.ResultsFileName,
                    new string[]
                    {
                        string.Format("{0};{1};{2};{3};{4};{5};",
                        string.Join(",", result.Parameters),
                        genSelector.PopulationIndex,
                        result.SharpIndex,
                        result.Profit,
                        result.MeanProfit,
                        result.SuccessRatio )
                    });
            }
            Console.ReadLine();
        }

        private static void GenSelector_PopulationCompleted(object sender, PopulationCompletedEventArgs e)
        {
            Console.WriteLine("{0}: iteration {1}, profit = {2}, sharp ind = {3}, mean profit = {4}, success = {5}",
                num, e.PopulationIndex, e.Results.First().Profit,
                e.Results.First().SharpIndex, e.Results.First().MeanProfit,
                e.Results.First().SuccessRatio);
        }
    }
}
