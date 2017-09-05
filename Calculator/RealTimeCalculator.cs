using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FortsRobotLib.Genetics;
using FortsRobotLib.CandleProviders;
using System.IO;
using FortsRobotLib.ProviderDataCache;
using Calculator.Properties;
using System.Reflection;
using FortsRobotLib.Algorithms;

namespace Calculator
{
    public class RealTimeCalculator<T> : IRealTimeCalculator
        where T : AlgorithmBase, IAlgorithm, new()
    {
        private int _num;
        public void Calculate()
        {
            var fName = string.Format("{0}_{1}", typeof(T).Name, Settings.Default.ResultsFileName);
            if (File.Exists(fName))
                File.Delete(fName);
            MemoryCache<FinamCandleProvider> cache;
            using (var provider = new FinamCandleProvider(Settings.Default.InsName, Settings.Default.TimePeriod,
                Settings.Default.MarketCode, Settings.Default.InsCode, Settings.Default.DateFrom, Settings.Default.DateTo))
            {
                cache = new MemoryCache<FinamCandleProvider>(provider);
            }
            File.AppendAllLines(fName, new string[] { "Parameters;Populations total;Sharp Index;Profit;Mean profit per deal;Mean +profit per deal;Mean -profit per deal;Success %;" });
            for (var i = Settings.Default.ParamsCountFrom; i < Settings.Default.ParamsCountTo; i = i + 2)
            {
                _num = i;
                Console.WriteLine("Length {0}:", i);
                Console.WriteLine("----------------------");
                var genSelector = new GeneticsSelector<FinamCandleProvider, T>(cache, 3, 100, i, generationSize: Settings.Default.GenerationSize, threadsNum: Settings.Default.ThreadsCount);
                genSelector.PopulationCompleted += GenSelector_PopulationCompleted;
                genSelector.Select(Settings.Default.PopulationsCount);
                genSelector.Wait();
                var result = genSelector.GetBestResults().First();
                File.AppendAllLines(fName,
                    new string[]
                    {
                        string.Format("{0};{1};{2};{3};{4};{5};{6};{7};",
                        string.Join(",", result.Parameters),
                        genSelector.PopulationIndex,
                        result.SharpIndex,
                        result.Balance,
                        result.MeanProfit,
                        result.MeanPositiveProfit,
                        result.MeanNegativeProfit,
                        result.SuccessRatio )
                    });
            }
            Console.ReadLine();
            _num = 0;
        }

        private void GenSelector_PopulationCompleted(object sender, PopulationCompletedEventArgs e)
        {
            Console.WriteLine("{0}: iteration {1}, profit = {2}, sharp ind = {3}, mean profit = {4}, success = {5}, mean +profit = {6}, mean -profit = {7}",
                _num, e.PopulationIndex, e.Results.First().Balance,
                e.Results.First().SharpIndex, e.Results.First().MeanProfit,
                e.Results.First().SuccessRatio, e.Results.First().MeanPositiveProfit, e.Results.First().MeanNegativeProfit);
        }
    }
}
