using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FortsRobotLib.CandleProviders;
using FortsRobotLib.Algorithms;
using FortsRobotLib.AccAggregator;

namespace FortsRobotLib.Calculator
{
    public interface ICalculator
    {
        TestAccAgregator Calculate(float[] parameters);
        void Calculate(Action<CalculationResult[]> onFinish);
        void Wait();
        void Reset();
        CalculationResult[] Results { get; }
        void AddParamsForCalculation(float[] parameters);
        int ThreadsNum { get; }
        bool IsRunning { get; }
    }
}
