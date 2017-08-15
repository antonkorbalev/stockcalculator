using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortsRobotLib.Algorithms
{
    public class AlgorithmBase
    {
        protected List<Candle> _candles;
        protected float[] _params;
        protected List<float[]> _data;

        public float[] Parameters
        {
            get
            {
                return _params;
            }
            private set
            {
                _params = value;
            }
        }
        public float[][] Data
        {
            get
            {
                return _data.ToArray();
            }
        }

        public virtual string Name
        {
            get
            {
                return "Basic FORTS Algorithm";
            }
        }

        public void Initialize(float[] parameters)
        {
            if (_params == null)
                _params = parameters;
        }
    }
}
