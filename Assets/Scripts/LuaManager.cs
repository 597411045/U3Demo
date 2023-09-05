using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;




[CSharpCallLua]
[LuaCallCSharp]

public class CSObject
{
    public int hp;
    public int p2;
}




[CSharpCallLua]
[LuaCallCSharp]
public class LuaManager : MonoBehaviour
{
    public static LuaEnv luaenv = null;

    private Action luaStart;
    private Action luaUpdate;
    private Action luaOnDestroy;


    private void Awake()
    {
        luaenv = new LuaEnv();
        
        luaenv.AddLoader(CustomLoader);
        luaenv.DoString("require 'main'");
        

        luaenv.Global.Set("CSHARP", this);
        luaenv.Global.Set("TestValue",1234);
        

        ;
        //luaenv.DoString(File.ReadAllText(Application.streamingAssetsPath + "/" + "main" + ".lua.txt")

        //LuaFunction luaAwake = luaenv.Global.Get<LuaFunction>("awake");
        //Action<Player> luaAwake2 = luaenv.Global.Get<Action<Player>>("awake");
        //luaenv.Global.Set<string,List<Enemy>>("Enemys",GameController.enemys);
        luaenv.Global.Get("start", out luaStart);

    }


    void Start()
    {
        luaStart();
        
        var max = luaenv.Global.GetInPath<LuaMax>("math.max");
        Debug.Log("max:" + max(32, 12));
    }

    void Update()
    {
        if (luaenv != null)
        {
            luaenv.Tick();
        }
    }

    [XLua.CSharpCallLua]
    public delegate double LuaMax(double a, double b);

    //public void LuaEnv.AddLoader(CustomLoader loader)
    public byte[] CustomLoader(ref string filepath)
    {

        if (filepath == "emmy_core")
        {
            return null;
        }
        string path = Application.streamingAssetsPath + "/" + filepath + ".lua.txt";
        return System.Text.Encoding.UTF8.GetBytes(File.ReadAllText(path));
    }

    private void OnDestroy()
    {
        //luaenv.Dispose();
    }
}