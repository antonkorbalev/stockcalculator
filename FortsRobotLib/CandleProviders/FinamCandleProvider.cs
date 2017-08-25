using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace FortsRobotLib.CandleProviders
{
    public class FinamCandleProvider : BaseTextCandleProvider, ICandleProvider
    {
        private const string DateTemplate = "dd/MM/yy HH:mm";
        private const string DownloadLink = "http://export.finam.ru/data.txt?market={0}&em={1}&code={2}&df={3}&mf={4}&yf={5}&dt={6}&mt={7}&yt={8}&p={9}data&e=.txt&dtf=4&tmf=4&MSOR=1&sep=3&sep1=1&datf=5&at=1";
        private IEnumerable<Candle> _candles;
        private IEnumerator<Candle> _enumerator;
        private string _ticker;
        private TimePeriod _period;
        private string _marketCode;
        private string _insCode;
        private DateTime _dateFrom;
        private DateTime _dateTo;

        public FinamCandleProvider(string ticker, TimePeriod period, string marketCode, string insCode, DateTime dateFrom, DateTime dateTo)
        {
            _ticker = ticker;
            _period = period;
            _marketCode = marketCode;
            _insCode = insCode;
            _dateFrom = dateFrom;
            _dateTo = dateTo;
        }

        public bool MoveNext()
        {
            return _enumerator.MoveNext();
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

        public bool Initialize()
        {
            var link = string.Format(DownloadLink,
            _marketCode, _insCode,
            _ticker, _dateFrom.Day, (_dateFrom.Month - 1), _dateFrom.Year,
            _dateTo.Day, (_dateTo.Month - 1), _dateTo.Year, (_period + 2));
            var req = (HttpWebRequest)WebRequest.Create(link);
            req.Timeout = int.MaxValue;
            var resp = (HttpWebResponse)req.GetResponse();
            var data = new List<Candle>();
            using (StreamReader reader = new StreamReader(
                 resp.GetResponseStream(), Encoding.UTF8))
            {
                reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    data.Add(GetCandle(reader.ReadLine()));
                }
                _candles = data.ToArray();
            }
            if (_candles.Any())
            {
                _enumerator = _candles.GetEnumerator();
                _enumerator.MoveNext();
                return true;
            }
            return false;
        }
    }
}
