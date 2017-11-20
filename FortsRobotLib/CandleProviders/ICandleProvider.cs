using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForexRobotLib.CandleProviders
{
    public interface ICandleProvider : ICandleIterator
    {
        bool Initialize();      
    }
}
