using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using XLua;
using UnityEngine.UI;

public class LuaManager : MonoBehaviour
{
    // public static LuaEnv luaenv = null;
    //
    // private Action luaStart;
    // private Action luaUpdate;
    // private Action luaOnDestroy;


    private void Awake()
    {
        // luaenv = new LuaEnv();
        //
        // luaenv.AddLoader(CustomLoader);
        // luaenv.DoString("require 'main'");
        // luaenv.Global.Get("start", out luaStart);
    }


    void Start()
    {
        //luaStart();
        Test();
    }

    void Update()
    {
        // if (luaenv != null)
        // {
        //     luaenv.Tick();
        // }
    }

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

    private void Test()
    {
        GameObject canvasGo = GameObject.Find("Canvas");
        GameObject buttonGO = Instantiate(Resources.Load<GameObject>("LoginPanel"));
        buttonGO.transform.SetParent(canvasGo.transform);
        buttonGO.transform.localScale = Vector3.one;
        buttonGO.transform.localPosition = Vector3.zero;
    }
}