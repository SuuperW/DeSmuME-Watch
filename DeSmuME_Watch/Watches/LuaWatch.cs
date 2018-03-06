using System;
using System.Windows.Forms;

using NLua;

namespace DeSmuME_Watch
{
    public class LuaWatch : Label, IWatchDisplay
    {
        /// <summary>
        /// A label for displaying values obtained through Lua scripts.
        /// </summary>
        /// <param name="filePath">The path to the Lua script. The script must contain a GetValue() function, which returns the desired text to be displayed.</param>
        /// <param name="lua">Obtained by calling GetLua() on a WatchManager32Bit.</param>
        public LuaWatch(string filePath, Lua.Lua lua)
        {
            NLua.Lua n = new NLua.Lua();
            n.LoadCLRPackage();
            n.DoString("import ('DeSmuME_Watch', 'DeSmuME_Watch.Lua')");

            n.DoFile(filePath);
            luaGetValue = n.GetFunction("GetValue");
            this.lua = lua;
            n["WATCH"] = lua;

            if (luaGetValue == null)
                throw new Exception("Lua script could not be loaded. Make sure it contains the function GetValue().");

            this.AutoSize = true;
            this.SetText("?");
        }

        LuaFunction luaGetValue;
        Lua.Lua lua;

        public void UpdateDisplayText()
        {
            SetText(luaGetValue.Call()[0] as string);
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
