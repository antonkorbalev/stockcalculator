using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FortsRobotLib.CandleProviders;
using FortsRobotLib.Algorithms;
using FortsRobotLib.AccAggregator;
using FortsRobotLib.ProviderDataCache;

namespace FortsRobotLib.Calculator
{
    public interface ICalculator<T>
        where T : ICandleProvider
    {
        TestAccAgregator Calculate(float[] parameters, out IAlgorithm alg);
        Task CalculateAsync();
        void Wait();
        void Reset();
        CalculationResult[] Results { get; }
        void AddParamsForCalculation(float[] parameters);
        int ThreadsNum { get; }
        bool IsRunning { get; }
        MemoryCache<T> MemCache { get; set; }
    }
}
