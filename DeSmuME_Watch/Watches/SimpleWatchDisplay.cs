using System;
using System.Windows.Forms;

namespace DeSmuME_Watch
{
    /*
     * I need to have some way of having a collection of watches/structure.
     * I need to have some way of displaying values from said collection.
     */

    public class SimpleWatchDisplay : Label, IWatchDisplay
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
                displayFixedPointExact = false;
            }
        }
        public bool displayAsFixedPoint
        {
            get { return _displayAsFixedPoint; }
            set
            {
                if (value)
                {
                    if (!(watch is FixedPoint4Watch))
                        throw new Exception("Cannot display an integer watch as fixed point.");
                    _displayAsHex = false;
                }
                else
                    _digitsPastRadix = 0;
                _displayAsFixedPoint = value;
            }
        }
        public bool displayFixedPointExact;
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
            displayAsFixedPoint = false;
            displayAsHex = false;
            displayAsSigned = true;
            displayFixedPointExact = true;

            this.AutoSize = true;
            SetText(name + ": ?");
        }

        public void UpdateDisplayText()
        {
            string displayText = watchName + ": ";
            if (displayAsFixedPoint)
            {
                FixedPoint4Watch w = watch as FixedPoint4Watch;
                displayText += FormatFixedPointValues(w.getValue(), w.getDiff(), displayChange, displayFixedPointExact, digitsPastRadix);
            }
            else
            {
                displayText += FormatIntegerValues(watch.cValAsInt, watch.getDiffAsInt(), displayChange, displayAsHex, displayAsSigned);
            }

            SetText(displayText);
        }

        public static string FormatIntegerValues(int currentValue, int change, bool displayChange = true, bool asHex = false, bool asSigned = true)
        {
            string vStr;
            string cStr;
            if (asHex)
            {
                vStr = currentValue.ToString("X");
                cStr = change.ToString("X");
            }
            else if (asSigned)
            {
                vStr = currentValue.ToString();
                cStr = change.ToString();
            }
            else
            {
                vStr = Convert.ToString((uint)currentValue);
                cStr = Convert.ToString((uint)change);
            }

            string ret = vStr;
            if (displayChange)
                ret += " (" + cStr + ")";

            return ret;
        }
        public static string FormatFixedPointValues(DSFixedPoint4 currentValue, DSFixedPoint4 change, bool displayChange = true, bool exactDisplay = true, int digitsPastRadix = 0)
        {
            string vStr;
            string cStr;
            if (exactDisplay)
            {
                vStr = currentValue.ExactStringValue();
                cStr = change.ExactStringValue();
            }
            else
            {
                vStr = currentValue.ToString("N" + digitsPastRadix);
                cStr = change.ToString("N" + digitsPastRadix);
            }

            string ret = vStr;
            if (displayChange)
                ret += " (" + cStr + ")";

            return ret;
        }

        private void SetText(string text)
        {
            if (this.InvokeRequired)
                this.Invoke((Action)(() => this.Text = text));
            else
                this.Text = text;
        }
    }
}
