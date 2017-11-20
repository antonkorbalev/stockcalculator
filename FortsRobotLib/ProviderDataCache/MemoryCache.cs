using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForexRobotLib.CandleProviders;
using ForexRobotLib;

namespace ForexRobotLib.ProviderDataCache
{
    /// <summary>
    /// This class is used to cache candles from providers
    /// to get faster optimization (a wrapper for providers)
    /// </summary>
    public class MemoryCache<T> : IProviderMemoryCache
        where T : ICandleProvider
    {
        private IEnumerable<Candle> _candles;
        private IEnumerator<Candle> _enumerator;

        internal IEnumerable<Candle> Candles
        {
            get
            {
                return _candles;
            }
        }

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
            _enumerator.Dispose();
            _candles = null;
        }

        /// <summary>
        /// Caches data from provider
        /// </summary>
        /// <param name="provider">Provider</param>
        public MemoryCache(T provider)
        {
            Initialize(provider);
        }

        /// <summary>
        /// Caches data from existing memory cache
        /// </summary>
        /// <param name="cache">Existing cache</param>
        public MemoryCache(MemoryCache<T> cache)
        {
            _candles = cache.Candles;
            _enumerator = _candles.GetEnumerator();
        }

        private void Initialize(T provider)
        {
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
