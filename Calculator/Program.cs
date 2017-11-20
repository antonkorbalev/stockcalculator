using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForexRobotLib.Genetics;
using ForexRobotLib.CandleProviders;
using ForexRobotLib;
using System.IO;
using ForexRobotLib.ProviderDataCache;
using Calculator.Properties;
using System.Reflection;
using ForexRobotLib.Algorithms;
using ForexRobotLib.Calculator;

namespace Calculator
{
    class Program
    {

        static void Main(string[] args)
        {
            List<Type> algTypes = new List<Type>();
            var searchPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            foreach (var file in Directory.GetFiles(searchPath, "*.dll"))
            {
                algTypes.AddRange(Assembly.LoadFrom(file).GetTypes()
                    .Where(o =>
                    o.BaseType == typeof(AlgorithmBase)
                    && o.IsDefined(typeof(AlgorithmAttribute))));
            }

            Console.WriteLine("=== Found {0} algorithms ===", algTypes.Count());
            for (var i = 1; i <= algTypes.Count(); i++)
                Console.WriteLine(" {0}) {1}", i, algTypes[i - 1].Name);
            Console.WriteLine();
            Console.Write(" Select algorithm: ");
            int num;
            int.TryParse(Console.ReadLine(), out num);

            if (num < 1 || num > algTypes.Count())
            {
                Console.WriteLine("Incorrect choice.");
                Console.ReadLine();
                return;
            }

            if (!args.Any() || (args[0] == "--genetics"))
            {
                var calc = (IRealTimeCalculator)Activator.CreateInstance(typeof(RealTimeCalculator<>)
                    .MakeGenericType(algTypes[num - 1]));
                calc.Calculate();
            }
            if (args[0] == "--calculate")
            {
                if (args.Length <= 1)
                    return;
                using (var provider = new FinamCandleProvider(Settings.Default.InsName, Settings.Default.TimePeriod,
                    Settings.Default.MarketCode, Settings.Default.InsCode, Settings.Default.DateFrom, Settings.Default.DateTo))
                {
                    var calc = (ICalculator<FinamCandleProvider>)Activator.CreateInstance(typeof(Calculator<,>)
                    .MakeGenericType(typeof(FinamCandleProvider), algTypes[num - 1]));
                    calc.MemCache = new MemoryCache<FinamCandleProvider>(provider);

                    foreach (var arg in args.Skip(1))
                    {
                        if (!arg.StartsWith("--p="))
                            return;
                        var splits = arg.Split('=').Last().Split(',');
                        var prms = new float[splits.Length];
                        for (var i = 0; i < splits.Length; i++)
                        {
                            float p;
                            if (!float.TryParse(splits[i], out p))
                                return;
                            prms[i] = p;
                        }
                        calc.AddParamsForCalculation(prms);
                    }
                    calc.CalculateAsync();
                    calc.Wait();
                    var n = 0;
                    foreach (var r in calc.Results)
                    {
                        n++;
                        AlgDataExport.ExportToCSV(r, string.Format("{0}_{1}", 
                            n, Settings.Default.ResultsFileName), string.Format("{0}, sInd={1}", algTypes[num-1].Name, r.SharpIndex));
                    }
                }
            }
        }
    }
}
