using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForexRobotLib.CandleProviders;

namespace ForexRobotLib.ProviderDataCache
{
    public interface IProviderMemoryCache : ICandleIterator 
    {
        void Reset();
    }
}
