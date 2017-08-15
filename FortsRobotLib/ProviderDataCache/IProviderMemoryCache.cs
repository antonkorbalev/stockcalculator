using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FortsRobotLib.CandleProviders;

namespace FortsRobotLib.ProviderDataCache
{
    public interface IProviderMemoryCache<T> : ICandleIterator 
        where T : ICandleProvider
    { 
        void Initialize(T provider);

        void Reset();
    }
}
