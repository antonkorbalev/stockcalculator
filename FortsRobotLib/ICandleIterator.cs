using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForexRobotLib
{
    public interface ICandleIterator : IDisposable
    {
        Candle Current { get; }
        bool MoveNext();
    }
}
