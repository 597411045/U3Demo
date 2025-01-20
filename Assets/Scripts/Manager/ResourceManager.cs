using System;
using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager instance;

    public ResourceManager()
    {
        instance = this;
    }

    public string ResourcePath = @"AssetBundles\StandaloneWindows\";

    public AssetBundle GetBundle(string path)
    {
        return AssetBundle.LoadFromFile(ResourcePath + path);
    }
}