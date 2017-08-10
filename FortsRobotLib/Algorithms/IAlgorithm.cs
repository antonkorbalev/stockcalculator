using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FortsRobotLib;

namespace FortsRobotLib.Algorithms
{
    public interface IAlgorithm
    {
        string Name { get; }
        float[] Parameters { get; }
        void Reset();
        AlgResult Check(Candle candle);
        float[][] Data { get; }
    }
}
