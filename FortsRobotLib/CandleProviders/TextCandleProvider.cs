using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ForexRobotLib.CandleProviders
{
    /// <summary>
    /// An iterator for candles from text file
    /// </summary>
    public class TextCandleProvider : BaseTextCandleProvider, ICandleProvider
    {
        public Candle Current { get; private set; }
        private StreamReader _reader;

        private string _dataPath = @"data\si.dat";

        public void SetTextParams(string path, char separator = ';')
        {
            _dataPath = path;
            _separator = separator;
        }

        public bool Initialize()
        {
            var fPath = Path.Combine(Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location), _dataPath);
            bool success = File.Exists(fPath);
            Trace.Assert(success, string.Format("File not found {0}!",fPath));
            _reader = new StreamReader(fPath);
            // read header
            _reader.ReadLine();
            return success && MoveNext();      
        }

        public bool MoveNext()
        {
            if (_reader == null)
                return Initialize();
            if (_reader.EndOfStream)
                return false;
            Current = GetCandle(_reader.ReadLine());
            return true;    
        }

        public void Dispose()
        {
            if (_reader!=null)
                _reader.Dispose();
        }
    }
}
