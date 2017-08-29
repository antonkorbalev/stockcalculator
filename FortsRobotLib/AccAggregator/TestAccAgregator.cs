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
                if ((_profits == null) || (!_profits.Any()))
                    return 0;
                var mean = MeanProfit;
                return mean /
                    (float)Math.Sqrt(_profits.Select(o => (o - mean) * (o - mean)).Sum());
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
