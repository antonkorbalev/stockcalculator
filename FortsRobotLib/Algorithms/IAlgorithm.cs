using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForexRobotLib;

namespace ForexRobotLib.Algorithms
{
    public interface IAlgorithm
    {
        string Name { get; }
        float[] Parameters { get; }
        void Initialize(float[] parameters);
        void Reset();
        AlgResult Check(Candle candle);
        float[][] Data { get; }
    }
}
