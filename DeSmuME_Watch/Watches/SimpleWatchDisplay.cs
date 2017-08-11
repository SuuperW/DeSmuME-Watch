using System;
using System.Windows.Forms;

namespace DeSmuME_Watch
{
    /*
     * I need to have some way of having a collection of watches/structure.
     * I need to have some way of displaying values from said collection.
     * I need to have some way of displaying calculated values.
     * 
     * 
     */

    public class SimpleWatchDisplay : Label
    {
        public IWatch32Bit watch;
        public string watchName;

        private int _digitsPastRadix;
        private bool _displayAsFixedPoint;
        private bool _displayAsSigned;

        public int digitsPastRadix
        {
            get { return _digitsPastRadix; }
            set
            {
                if (value < 0)
                    throw new Exception("Cannot display a negative number of digits.");
                if (value != 0)
                {
                    _displayAsHex = false;
                    _displayAsFixedPoint = true;
                }
                _digitsPastRadix = value;
            }
        }
        public bool displayAsFixedPoint
        {
            get { return _displayAsFixedPoint; }
            set
            {
                if (!(watch is FixedPoint4Watch))
                    throw new Exception("Cannot display an integer watch as fixed point.");
                if (value)
                    _displayAsHex = false;
                else
                    _digitsPastRadix = 0;
                _displayAsFixedPoint = value;
            }
        }
        public bool displayAsSigned
        {
            get { return _displayAsSigned; }
            set
            {
                if (value)
                    _displayAsHex = false;
                _displayAsSigned = value;
            }
        }

        private bool _displayAsHex;
        public bool displayAsHex
        {
            get { return _displayAsHex; }
            set
            {
                if (value)
                {
                    _digitsPastRadix = 0;
                    _displayAsFixedPoint = false;
                    _displayAsSigned = false;
                }
                _displayAsHex = value;
            }
        }

        public bool displayChange = true;


        public SimpleWatchDisplay(IWatch32Bit watch, string name)
        {
            this.watch = watch;
            this.watchName = name;

            digitsPastRadix = 0;
            displayAsFixedPoint = true;
            displayAsHex = false;
            displayAsSigned = true;
        }

        public void UpdateDisplayText()
        {
            string value;
            string change;
            if (displayAsHex)
            {
                value = watch.cValAsInt.ToString("X");
                change = " (" + watch.getDiffAsInt().ToString("X") + ")";
            }
            else if (displayAsFixedPoint)
            {
                FixedPoint4Watch w = watch as FixedPoint4Watch;
                value = w.getValue().ToString("N" + digitsPastRadix);
                change = " (" + w.getValue().ToString("N" + digitsPastRadix) + ")";
                // TODO: Support unsigned values?
            }
            else if (displayAsSigned)
            {
                value = Convert.ToString(watch.cValAsInt);
                change = " (" + watch.getDiffAsInt() + ")";
            }
            else
            {
                value = Convert.ToString((uint)watch.cValAsInt);
                change = " (" + (uint)watch.getDiffAsInt() + ")";
            }

            string displayText = watchName + ": " + value;
            if (displayChange)
                displayText += change;
            this.Text = displayText;
        }
    }
}
