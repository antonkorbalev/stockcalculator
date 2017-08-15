using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortsRobotLib
{
    /// <summary>
    /// Basic candle object
    /// </summary>
    public struct Candle
    {
        public DateTime TimeStamp { get; set; }
        public float Open { get; set; }
        public float High { get; set; }
        public float Low { get; set; }
        public float Close { get; set; }
        public float Volume { get; set; }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public static bool operator ==(Candle c1, Candle c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(Candle c1, Candle c2)
        {
            return !c1.Equals(c2);
        }
    }
}
