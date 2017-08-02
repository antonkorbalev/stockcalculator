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
        private float[][] _data;
        private float[] _paramets;

        public float[][] Data
        {
            get
            {
                return _data;    
            }
        }

        public float[] Parameters
        {
            get
            {
                return _paramets;
            }
            set
            {
                _paramets = value;
            }
        }

        public string Name
        {
            get
            {
                return "Basic FORTS Algorithm";
            }
        }

        public AlgResult Check(Candle candle)
        {
            return AlgResult.Hold;
        }

        public void Reset()
        {

        }
    }
}
