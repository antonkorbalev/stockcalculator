using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForexRobotLib.AccAggregator;

namespace ForexRobotLib.Calculator
{
    public class CalculationResult
    {
        public float Balance { get; set; }
        public float SharpIndex { get; set; }
        public float[] Parameters { get; set; }
        public float MeanProfit { get; set; }
        public float MeanPositiveProfit { get; set; }
        public float MeanNegativeProfit { get; set; }
        public float SuccessRatio { get; set; }
        public AccRow[] Data { get; set; }
        public float[][] AlgorithmData { get; set; }
    }
}
