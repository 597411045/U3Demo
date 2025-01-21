using System;
using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    private static ResourceManager instance;

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
    public void Awake()
    {
        instance = this;
    }

    public string ResourcePath = @"AssetBundles\StandaloneWindows\";

    public AssetBundle GetBundle(string path)
    {
        return AssetBundle.LoadFromFile(ResourcePath + path);
    }
}