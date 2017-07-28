using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace FortsRobotLib
{
    public class TextCandleProvider : ICandleProvider, IDisposable
    {
        public Candle Current { get; private set; }
        private StreamReader _reader;

        private const string DataPath = @"data\si.dat";
        private const char Separator = ';';

        public bool Initialize()
        {
            var fPath = Path.Combine(Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location), DataPath);
            bool success = File.Exists(fPath);
            Trace.Assert(success, string.Format("File not found {0}!",fPath));
            _reader = new StreamReader(fPath);
            // read header
            _reader.ReadLine();
            return success && MoveNext();      
        }

        internal Candle GetCandle(string line)
        {
            var parts = line.Split(Separator);
            DateTime timeStamp = DateTime.MinValue;
            float[] values = new float[parts.Length - 2];
            Trace.Assert(DateTime.TryParse(string.Format("{0} {1}",parts[0], parts[1]), out timeStamp),"Invalid date format in candle description!");
            int num = 0;
            foreach (var part in parts.Skip(2))
            {
                Trace.Assert(float.TryParse(part, out values[num]), "Invalid number format in candle description!");
                num++;
            }
            return new Candle()
            {
                TimeStamp = timeStamp,
                Open = values[0],
                High = values[1],
                Low = values[2],
                Close = values[3],
                Volume = values[4]
            };
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
