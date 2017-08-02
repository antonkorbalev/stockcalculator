using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FortsRobotLib;
using FortsRobotLib.Algorithms;

namespace BasicAlgorithms
{
    public class BasicAlgorithm : IAlgorithm
    {
        private List<float[]> _data;
        private float[] _params;
        private List<Candle> _candles;

        public float[][] Data
        {
            get
            {
                return _data.ToArray();    
            }
        }

        public float[] Parameters
        {
            get
            {
                return _params;
            }
            set
            {
                _params = value;
            }
        }

        public string Name
        {
            get
            {
                return "Basic FORTS Algorithm";
            }
        }

        public BasicAlgorithm()
        {
            Reset();
        }

        public void Reset()
        {
            _data = new List<float[]>();
            _params = new float[0];
            _candles = new List<Candle>();
        }

        public AlgResult Check(Candle candle)
        {
            return AlgResult.Hold;
        }
    }
}
