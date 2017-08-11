using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeSmuME_Watch
{
    public class DSFixedPoint4
    {
        /// <summary>
        /// Converts the given value into a 20.12 fixed-point value.
        /// </summary>
        public DSFixedPoint4(float val)
        {
            val *= 4096;
            v = (int)val;
        }
        /// <summary>
        /// Creates a new DSFixedPoint4 and uses the bits of the given value as the bits of the new DSFixedPoint4.
        /// </summary>
        public DSFixedPoint4(int val)
        { WriteAsInt(val); }

        int v;

        public string ToString(string format = "")
        {
            return this.AsFloat().ToString(format);
        }

        // Implicit conversions
        public static implicit operator DSFixedPoint4(float val)
        { return new DSFixedPoint4(val); }
        public static implicit operator float(DSFixedPoint4 val)
        { return val.AsFloat(); }
        public static implicit operator DSFixedPoint4(int val)
        { return new DSFixedPoint4(val); }

        // Integer conversions
        public int ReadAsInt()
        { return v; }
        public int ToInt()
        { return v / 4096; }
        /// <summary>
        /// Writes the bits of the given value onto the internal representation of the 20.12 fixed point value.
        /// </summary>
        public void WriteAsInt(int val)
        { v = val; }

        // Get value as a float
        public float AsFloat()
        { return (float)v / 4096; }

        // Operators
        public static DSFixedPoint4 operator +(DSFixedPoint4 v1, DSFixedPoint4 v2)
        { return new DSFixedPoint4(v1.v + v2.v); }
        public static DSFixedPoint4 operator -(DSFixedPoint4 v1, DSFixedPoint4 v2)
        { return new DSFixedPoint4(v1.v - v2.v); }
        public static DSFixedPoint4 operator *(DSFixedPoint4 v1, DSFixedPoint4 v2)
        {
            long intermediate = (long)v1.v * v2.v;
            return new DSFixedPoint4((int)(intermediate >> 12));
        }
        public static DSFixedPoint4 operator /(DSFixedPoint4 v1, DSFixedPoint4 v2)
        {
            long iValue1 = (long)v1.v << 12;
            return new DSFixedPoint4((int)(iValue1 / v2.v));
        }
    }
}
