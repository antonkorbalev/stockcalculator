using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortsRobotLib
{
    public interface ICandleProvider
    {
        bool Initialize();
        Candle Current { get; }
        bool MoveNext();        
    }
}
