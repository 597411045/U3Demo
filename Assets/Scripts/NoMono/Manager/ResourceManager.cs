using System;
using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

public class ResourceManager : SingleTon<ResourceManager>
{
    private static ResourceManager instance;

    private Dictionary<string, AssetBundle> cachedBundles = new Dictionary<string, AssetBundle>();

    public static ResourceManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ResourceManager();
            }

            return instance;
        }
    }

    public string ResourcePath = @"AssetBundles\StandaloneWindows\";

    public AssetBundle GetBundle(string path)
    {
        if (cachedBundles.ContainsKey(path) == false)
        {
            AssetBundle tmp = AssetBundle.LoadFromFile(ResourcePath + path);
            if (tmp != null)
            {
                cachedBundles.Add(path, tmp);
                return tmp;
            }
        }
        else
        {
            if (cachedBundles[path] == null)
            {
                AssetBundle tmp = AssetBundle.LoadFromFile(ResourcePath + path);
                if (tmp != null)
                {
                    cachedBundles[path] = tmp;
                }
            }

            return cachedBundles[path];
        }

        return null;
    }
}