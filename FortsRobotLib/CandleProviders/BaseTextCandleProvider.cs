using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace FortsRobotLib.CandleProviders
{
    public class BaseTextCandleProvider
    {
        protected char _separator = ';';

        internal Candle GetCandle(string line)
        {
            var parts = line.Split(_separator);
            DateTime timeStamp = DateTime.MinValue;
            float[] values = new float[parts.Length - 2];
            Trace.Assert(DateTime.TryParse(string.Format("{0} {1}", parts[0], parts[1]), out timeStamp), "Invalid date format in candle description!");
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
    }
}
