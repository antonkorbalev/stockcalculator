using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace ForexRobotLib.CandleProviders
{
    public class FinamCandleProvider : BaseTextCandleProvider, ICandleProvider
    {
        private const string DateTemplate = "dd/MM/yy HH:mm";
        private const string DownloadLink = "http://export.finam.ru/data.txt?market={0}&em={1}&code={2}&df={3}&mf={4}&yf={5}&dt={6}&mt={7}&yt={8}&p={9}&e=.txt&dtf=4&tmf=4&MSOR=1&sep=3&sep1=1&datf=5&at=0";
        private string _ticker;
        private TimePeriod _period;
        private string _marketCode;
        private string _insCode;
        private DateTime _dateFrom;
        private DateTime _dateTo;
        private StreamReader _reader;
        private Candle _current;

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
            if (_reader.EndOfStream)
                return false;
            _current = GetCandle(_reader.ReadLine());
            return true;
        }

        public Candle Current
        {
            get
            {
                return _current;
            }
        }

        public void Dispose()
        {
            _reader.Dispose();
        }

        public bool Initialize()
        {
            var link = string.Format(DownloadLink,
            _marketCode, _insCode,
            _ticker, _dateFrom.Day, (_dateFrom.Month - 1), _dateFrom.Year,
            _dateTo.Day, (_dateTo.Month - 1), _dateTo.Year, ((int)_period + 2));
            var req = (HttpWebRequest)WebRequest.Create(link);
            req.Timeout = int.MaxValue;
            var resp = (HttpWebResponse)req.GetResponse();
            var data = new List<Candle>();
            _reader = new StreamReader(resp.GetResponseStream(), Encoding.UTF8);
            return MoveNext();
        }
    }
}
