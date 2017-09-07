using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortsRobotLib.AccAggregator
{
    /// <summary>
    /// Account emulator for testing
    /// </summary>
    public class TestAccAgregator : IAccAgregator
    {
        private List<AccRow> _data;
        private float _money;
        private int _assetsCount;
        private float _assetPrice;
        private List<float> _profits;
        private float _lastDealBal;

        public float SuccessRatio
        {
            get
            {
                if ((_profits == null) || (!_profits.Any()))
                    return 0;
                return (float)_profits.Where(o => o > 0).Count() / _profits.Count();
            }
        }

        public float MeanPositiveProfit
        {
            get
            {
                return getMeanProfit(o => o > 0);
            }
        }

        public float MeanNegativeProfit
        {
            get
            {
                return getMeanProfit(o => o < 0);
            }
        }

        public float MeanProfit
        {
            get
            {
                return getMeanProfit(o => o != 0);
            }
        }

        public float SharpIndex
        {
            get
            {
                return GetSharpIndex();
            }
        }
        public float[] Profits
        {
            get
            {
                return _profits.ToArray();
            }
        }

        public int Assets
        {
            get
            {
                return _assetsCount;
            }
        }
        public TestAccAgregator()
        {
            Reset();
        }

        public AccRow[] Data
        {
            get
            {
                return _data.ToArray();
            }
        }

        public float Balance
        {
            get
            {
                return getBalance();
            }
        }

        private float getMeanProfit(Func<float, bool> condition)
        {
            if ((_profits == null) || (!_profits.Any()))
                return 0;
            var selection = _profits.Where(condition);
            return selection.Sum() / selection.Count();
        }

        private float getBalance()
        {
            return _money + _assetsCount * _assetPrice;
        }

        internal float GetSharpIndex()
        {
            if (!Data.Any())
                return 0;
            var profitsPerMonths = new Dictionary<DateTime, float>();
            var data = Data.First();
            var maxTimeStamp = Data.Max(o => o.Candle.TimeStamp);
            // profit for every month
            while (maxTimeStamp >= data.Candle.TimeStamp.AddMonths(1)) 
            {
                var nextData = Data.First(o => o.Candle.TimeStamp >= data.Candle.TimeStamp.AddMonths(1));
                profitsPerMonths.Add(data.Candle.TimeStamp, nextData.Balance - data.Balance);
                data = nextData;
            }

            var meanProfitPerMonth = profitsPerMonths.Sum(o => o.Value) / profitsPerMonths.Count();
            if (profitsPerMonths.Count() <= 1)
                return 0;
            else
                return
                    meanProfitPerMonth /
                    (float)Math.Sqrt(profitsPerMonths.Select(o => 
                    (o.Value - meanProfitPerMonth) * (o.Value - meanProfitPerMonth))
                    .Sum());    
        }

        public void Reset()
        {
            _data = new List<AccRow>();
            _profits = new List<float>();
            _assetsCount = 0;
            _money = 0;
            _assetPrice = 0;
        }

        public void Buy(int amount, Candle candle)
        {
            _money = _money - amount * candle.Close;
            _assetsCount += amount;
            Pass(candle);
            var profit = getBalance() - _lastDealBal;
            if (profit != 0)
                _profits.Add(profit);
            _lastDealBal = getBalance();
        }

        public void Sell(int amount, Candle candle)
        {
            _money = _money + amount * candle.Close;
            _assetsCount -= amount;
            Pass(candle);
            var profit = getBalance() - _lastDealBal;
            if (profit != 0)
                _profits.Add(profit);
            _lastDealBal = getBalance();
        }

        public void Close(Candle candle)
        {
            if (_assetsCount > 0)
                Sell(_assetsCount, candle);
            else
                Buy(-_assetsCount, candle);
        }

        public void Pass(Candle candle)
        {
            _assetPrice = candle.Close;
            _data.Add(new AccRow(candle, _assetsCount, getBalance()));
        }
    }
}
