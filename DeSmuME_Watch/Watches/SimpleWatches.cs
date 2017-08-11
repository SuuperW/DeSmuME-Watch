﻿using System;

namespace DeSmuME_Watch
{
    public abstract class AbstractWatch : IWatch32Bit
    {
        // IWatch
        public uint[] offsets { get; set; }

        protected int _cValAsInt;
        public int cValAsInt { get { return _cValAsInt; } }
        public int pValAsInt { get; protected set; }
        public int getDiffAsInt() { return cValAsInt - pValAsInt; }

        // abstracct
        public abstract int type { get; }
        public abstract void updateValue(int newValue);

        public abstract object currentValue { get; }

        // Constructors
        public AbstractWatch(uint[] offsets)
        { this.offsets = offsets; }
        public AbstractWatch(uint address)
        { offsets = new uint[] { address }; }
        public AbstractWatch(uint pointer, uint offset)
        { offsets = new uint[] { pointer, offset }; }
    }

    /// <summary>
    /// Watch for an unsigned 1 byte integer.
    /// </summary>
    public class ByteWatch : AbstractWatch
    {
        #region "MPWatch"
        override public int type { get { return WatchTypes.T_BYTE; } }

        override public void updateValue(int newValue)
        {
            pValAsInt = _cValAsInt;
            _cValAsInt = newValue & 0xFF;
        }

        override public object currentValue { get { return (byte)_cValAsInt; } }
        #endregion

        // constructors, why are these required? >:(
        public ByteWatch(uint[] offsets) : base(offsets) { }
        public ByteWatch(uint address) : base(address) { }
        public ByteWatch(uint pointer, uint offset) : base(pointer, offset) { }

        // Other
        public byte cValAsByte { get { return (byte)_cValAsInt; } }
    }

    /// <summary>
    /// Watch for a signed 2 byte integer.
    /// </summary>
    public class ShortWatch : AbstractWatch
    {
        #region "MPWatch"
        override public int type { get { return WatchTypes.T_SHORT; } }

        override public void updateValue(int newValue)
        {
            pValAsInt = _cValAsInt;
            _cValAsInt = (short)newValue;
        }

        override public object currentValue { get { return (short)_cValAsInt; } }
        #endregion

        // constructors, why are these required? >:(
        public ShortWatch(uint[] offsets) : base(offsets) { }
        public ShortWatch(uint address) : base(address) { }
        public ShortWatch(uint pointer, uint offset) : base(pointer, offset) { }

        // Other
        public short cValAsShort { get { return (short)_cValAsInt; } }
    }

    /// <summary>
    /// Watch for a signed 4 byte integer.
    /// </summary>
    public class IntWatch : AbstractWatch
    {
        #region "MPWatch"
        override public int type { get { return WatchTypes.T_INT; } }

        override public void updateValue(int newValue)
        {
            pValAsInt = _cValAsInt;
            _cValAsInt = newValue;
        }

        override public object currentValue { get { return _cValAsInt; } }
        #endregion

        // constructors, why are these required? >:(
        public IntWatch(uint[] offsets) : base(offsets) { }
        public IntWatch(uint address) : base(address) { }
        public IntWatch(uint pointer, uint offset) : base(pointer, offset) { }
    }

    /// <summary>
    /// Watch for a 20.12 fixed point value.
    /// </summary>
    public class FixedPoint4Watch : AbstractWatch
    {
        #region "MPWatch"
        override public int type { get { return WatchTypes.T_FLOAT; } }

        override public void updateValue(int newValue)
        {
            pValAsInt = _cValAsInt;
            _cValAsInt = newValue;
            value.WriteAsInt(newValue);
        }

        override public object currentValue { get { return (float)value; } }
        #endregion

        // Constructors
        public FixedPoint4Watch(uint[] offsets) : base(offsets) { }
        public FixedPoint4Watch(uint address) : base(address) { }
        public FixedPoint4Watch(uint pointer, uint offset) : base(pointer, offset) { }

        // Other
        private DSFixedPoint4 value = new DSFixedPoint4(0);
        public DSFixedPoint4 getValue() { return new DSFixedPoint4(value); }

        public DSFixedPoint4 getDiff() { return value - new DSFixedPoint4(pValAsInt); }
    }
}
