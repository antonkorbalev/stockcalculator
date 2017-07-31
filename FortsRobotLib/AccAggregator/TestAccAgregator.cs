using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortsRobotLib.AccAggregator
{
    public class TestAccAgregator : IAccAgregator
    {
        private List<AccRow> _data;
        private float _money;
        private int _assetsCount;
        private float _assetPrice;

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

        private float getBalance()
        {
            return _money + _assetsCount * _assetPrice;
        }

        public void Reset()
        {
            _data = new List<AccRow>();
            _assetsCount = 0;
            _money = 0;
            _assetPrice = 0;
        }

        public void Buy(int amount, Candle candle)
        {
            _money = _money - amount * candle.Close;
            _assetsCount += amount;
            Pass(candle);    
        }

        public void Sell(int amount, Candle candle)
        {
            _money = _money + amount * candle.Close;
            _assetsCount -= amount;
            Pass(candle);
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
