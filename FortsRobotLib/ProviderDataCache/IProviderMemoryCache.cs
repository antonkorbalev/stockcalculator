using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FortsRobotLib.CandleProviders;

namespace FortsRobotLib.ProviderDataCache
{
    public interface IProviderMemoryCache : ICandleIterator 
    {
        void Reset();
    }
}
