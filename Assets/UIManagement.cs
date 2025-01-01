using System;
using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

public class UIManagement
{
    public static UIManagement instance;
    public UIPanel UIPanel;

    public UIManagement()
    {
        instance = this;
    }

    public void CusAwake()
    {
        UIPanel.CP1_OnEnable(ResourceManagement.instance.GetBundle("uiresource\\desc_bundle"),
            ResourceManagement.instance.GetBundle("uiresource\\res_bundle"));
        
#if (UNITY_5 || UNITY_5_3_OR_NEWER)
        //Use the font names directly
        UIConfig.defaultFont = "Microsoft YaHei";
#else
        //Need to put a ttf file into Resources folder. Here is the file name of the ttf file.
        UIConfig.defaultFont = "afont";
#endif
        //UIPackage.AddPackage("UI/Basics");

        UIConfig.verticalScrollBar = "ui://Basics/ScrollBar_VT";
        UIConfig.horizontalScrollBar = "ui://Basics/ScrollBar_HZ";
        UIConfig.popupMenu = "ui://Basics/PopupMenu";
        UIConfig.buttonSound = (NAudioClip)UIPackage.GetItemAsset("Basics", "click");
    }
}