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
using System.Diagnostics;

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
                Console.Title = string.Format("Length {0}", _num);
                Console.WriteLine();
                Console.WriteLine(" Length {0}:", i);
                Console.WriteLine(" ----------------------------------------------------------------");
                Console.WriteLine(" | iter | profit | sharp | mean p | success | mean p+ | mean p- |");
                Console.WriteLine(" ----------------------------------------------------------------");
                var genSelector = new GeneticsSelector<FinamCandleProvider, T>(cache, 3, 100, i, generationSize: Settings.Default.GenerationSize, threadsNum: Settings.Default.ThreadsCount, crossPercent: Settings.Default.CrossPercent);
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
                // generate result file for best result
                var algDataFileName = string.Format("{0}_{1}params_{2}",
                    typeof(T).Name, i, Settings.Default.ResultsFileName);
                AlgDataExport.ExportToCSV(result, algDataFileName, typeof(T).Name);
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Calculation completed.");
            Console.ReadLine();
            _num = 0;
        }

        private void GenSelector_PopulationCompleted(object sender, PopulationCompletedEventArgs e)
        {
            Console.Title = string.Format("Length {0}, iteration {1}", _num, e.PopulationIndex);
            string balance = e.Results.First().Balance.ToString();
            string sharpIndex = e.Results.First().SharpIndex.ToString();
            string meanProfit = e.Results.First().MeanProfit.ToString();
            string success = e.Results.First().SuccessRatio.ToString();
            string meanPosProfit = e.Results.First().MeanPositiveProfit.ToString();
            string meanNegProfit = e.Results.First().MeanNegativeProfit.ToString();
            Console.WriteLine(" | {0,-5}| {1,-7}| {2,-6}| {3,-7}| {4,-8}| {5,-8}| {6,-8}|",
                e.PopulationIndex, 
                balance.Substring(0, Math.Min(7, balance.Length)),
                sharpIndex.Substring(0, Math.Min(6, sharpIndex.Length)),
                meanProfit.Substring(0, Math.Min(7, meanProfit.Length)),
                success.Substring(0, Math.Min(8, success.Length)),
                meanPosProfit.Substring(0, Math.Min(8, meanPosProfit.Length)),
                meanNegProfit.Substring(0, Math.Min(8, meanNegProfit.Length)));
        }
    }
}
