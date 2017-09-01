using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FortsRobotLib.Genetics;
using FortsRobotLib.CandleProviders;
using FortsRobotLib;
using System.IO;
using FortsRobotLib.ProviderDataCache;
using Calculator.Properties;
using System.Reflection;
using FortsRobotLib.Algorithms;

namespace Calculator
{
    class Program
    {

        static void Main(string[] args)
        {
            Type algType = null;
            var searchPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            foreach (var file in Directory.GetFiles(searchPath, "*.dll"))
            {
                algType = Assembly.LoadFrom(file).GetTypes()
                    .FirstOrDefault(o => 
                    o.Name == Settings.Default.AlgorithmName
                    && o.BaseType == typeof(AlgorithmBase));
                if (algType != null)
                {
                    var inst = Activator.CreateInstance(algType) as IAlgorithm;
                    if (inst != null)
                        break;
                    else
                        algType = null;
                }  
            }    

            if (algType == null)
            {
                Console.WriteLine("Algorithm not found.");
                Console.ReadLine();
                return;
            }

            var calc = (IRealTimeCalculator) Activator.CreateInstance(typeof(RealTimeCalculator<>)
                .MakeGenericType(algType));
            calc.Calculate();
        }
    }
}
