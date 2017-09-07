using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortsRobotLib.AccAggregator
{
    public interface IAccAgregator
    {
        int Assets { get; }
        void Pass(Candle candle);
        void Buy(int amount, Candle candle);
        void Sell(int amount, Candle candle);
        void Close(Candle candle);
        float Balance { get; }
        void Reset();
        AccRow[] Data { get; }
        float[] Profits { get; }
        float SharpIndex { get; }
        float MeanProfit { get; }
        float MeanPositiveProfit { get; }
        float MeanNegativeProfit { get; }
        float SuccessRatio { get; }

    }
}
