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
            List<Type> algTypes = new List<Type>();
            var searchPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            foreach (var file in Directory.GetFiles(searchPath, "*.dll"))
            {
                algTypes.AddRange(Assembly.LoadFrom(file).GetTypes()
                    .Where(o =>
                    o.BaseType == typeof(AlgorithmBase)
                    && typeof(IAlgorithm).IsAssignableFrom(o)));
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

            var calc = (IRealTimeCalculator) Activator.CreateInstance(typeof(RealTimeCalculator<>)
                .MakeGenericType(algTypes[num-1]));
            calc.Calculate();
        }
    }
}
