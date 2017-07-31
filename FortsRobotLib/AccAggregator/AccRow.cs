using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortsRobotLib.AccAggregator
{
    public struct AccRow 
    {
        public Candle Candle { get; }
        public int Amount { get;  }
        public float Balance { get; }

        public AccRow(Candle candle, int amount, float balance)
        {
            Candle = candle;
            Amount = amount;
            Balance = balance;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}",Candle.TimeStamp, Candle.Close, Amount, Balance);
        }
    }
}
