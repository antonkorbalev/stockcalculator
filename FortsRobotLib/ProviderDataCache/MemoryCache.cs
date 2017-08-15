using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FortsRobotLib.CandleProviders;
using FortsRobotLib;

namespace FortsRobotLib.ProviderDataCache
{
    /// <summary>
    /// This class is used to cache candles from providers
    /// to get faster optimization (a wrapper for providers)
    /// </summary>
    public class MemoryCache<T> : IProviderMemoryCache<ICandleProvider>
        where T : ICandleProvider
    {
        private IEnumerable<Candle> _candles;
        private IEnumerator<Candle> _enumerator;
        private ICandleProvider _provider;

        public Candle Current
        {
            get
            {
                if (_enumerator == null)
                    return new Candle();
                return _enumerator.Current;
            }
        }

        public void Dispose()
        {
            _provider.Dispose();
            _enumerator.Dispose();
            _candles = null;
        }

        public void Initialize(ICandleProvider provider)
        {
            _provider = provider;
            var candles = new List<Candle>();
            using (provider)
            {
                provider.Initialize();
                do
                {
                    candles.Add(provider.Current);
                }
                while (provider.MoveNext());
            }
            _candles = candles;
            _enumerator = _candles.GetEnumerator();
        }

        public bool MoveNext()
        {
            return _enumerator.MoveNext();    
        }

        public void Reset()
        {
            _enumerator.Reset();
        }
    }
}
