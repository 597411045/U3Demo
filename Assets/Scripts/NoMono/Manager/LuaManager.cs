using System;
using System.IO;
using LuaInterface;
using UnityEngine;

public class LuaManager : SingleTon<LuaManager>
{
    public void Awake()
    {
        LuaState lua = new LuaState();
        lua.Start();
        DelegateFactory.Init();

        //string fullPath = Application.dataPath + "\\fgui/lua";
        //lua.AddSearchPath(fullPath); 
        
        //lua.Require("Main");   
        

        //StreamReader file = new StreamReader(@"G:\0\Win\U3Demo\Assets\fgui\lua\Main.lua");
        
        AssetBundle ab = ResourceManager.Instance.GetBundle("uiresource\\lua");
        TextAsset ta = ab.LoadAsset<TextAsset>("Main.lua");
        lua.DoString(ta.ToString(),"Main.lua.bytes");


        //lua.GetFunction("luaDebug").Call();
        
        LuaFunction luaFunc = lua.GetFunction("luaFunc");
        
        int num = luaFunc.Invoke<int, int>(123456);
        Debugger.Log("generic call return: {0}", num);
        
        luaFunc.BeginPCall();                
        luaFunc.Push(123456);
        luaFunc.PCall();        
        num = (int)luaFunc.CheckNumber();
        luaFunc.EndPCall();
        Debugger.Log("expansion call return: {0}", num);

        Func<int, int> Func = luaFunc.ToDelegate<Func<int, int>>();
        num = Func(123456);
        Debugger.Log("Delegate call return: {0}", num);
        
        num = lua.Invoke<int, int>("luaFunc", 123456, true);
        Debugger.Log("luastate call return: {0}", num);
        
        lua.CheckTop();
        lua.Dispose();
        lua = null;
    }
}