using System;
using System.Windows.Forms;

using NLua;

namespace DeSmuME_Watch
{
    public class LuaWatch : Label, IWatchDisplay
    {
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
