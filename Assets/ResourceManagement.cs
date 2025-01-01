using System;
using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

public class ResourceManagement
{
    public static ResourceManagement instance;

    public ResourceManagement()
    {
        instance = this;
    }

    public string ResourcePath = @"AssetBundles\StandaloneWindows\";

    public AssetBundle GetBundle(string path)
    {
        return AssetBundle.LoadFromFile(ResourcePath + path);
    }
}