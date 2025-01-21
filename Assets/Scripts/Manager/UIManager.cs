using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using FairyGUI;
using FairyGUI.Utils;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public UIPanel UIPanel;

    public UIManager()
    {
        instance = this;
    }

    public void Awake()
    {
        var desc = ResourceManager.instance.GetBundle("uiresource\\desc_bundle");
        var res = ResourceManager.instance.GetBundle("uiresource\\res_bundle");

//         UIPanel.CP1_OnEnable();
//
//         List<byte[]> source = new List<byte[]>();
//         List<string> mainAssetName = new List<string>();
//         string[] names = desc.GetAllAssetNames();
//         string searchPattern = "_fui";
//         foreach (string n in names)
//         {
//             if (n.IndexOf(searchPattern) != -1)
//             {
//                 TextAsset ta = desc.LoadAsset<TextAsset>(n);
//                 if (ta != null)
//                 {
//                     source.Add(ta.bytes);
//                     mainAssetName.Add(Path.GetFileNameWithoutExtension(n));
//                 }
//             }
//         }
//
//         if (source == null)
//             throw new Exception("FairyGUI: no package found in this bundle.");
//         desc.Unload(true);
//
//         for (int i = 0; i < mainAssetName.Count; i++)
//         {
//             ByteBuffer buffer = new ByteBuffer(source[i]);
//
//             UIPackage pkg = new UIPackage();
//             pkg._resBundle = res;
//             pkg._fromBundle = true;
//             int pos = mainAssetName[i].IndexOf("_fui");
//             if (pos != -1)
//                 mainAssetName[i] = mainAssetName[i].Substring(0, pos);
//             if (!pkg.LoadPackage(buffer, mainAssetName[i]))
//                 Debug.Log("Failed LoadPackage");
//
//             UIPackage._packageInstById[pkg.id] = pkg;
//             UIPackage._packageInstByName[pkg.name] = pkg;
//             UIPackage._packageList.Add(pkg);
//         }
//
//
// #if (UNITY_5 || UNITY_5_3_OR_NEWER)
//         //Use the font names directly
//         UIConfig.defaultFont = "Microsoft YaHei";
// #else
//         //Need to put a ttf file into Resources folder. Here is the file name of the ttf file.
//         UIConfig.defaultFont = "afont";
// #endif
//         //UIPackage.AddPackage("UI/Basics");
//
//         UIConfig.verticalScrollBar = "ui://Basics/ScrollBar_VT";
//         UIConfig.horizontalScrollBar = "ui://Basics/ScrollBar_HZ";
//         UIConfig.popupMenu = "ui://Basics/PopupMenu";
//         UIConfig.buttonSound = (NAudioClip)UIPackage.GetItemAsset("Basics", "click");
//
//         UIPanel._created = true;
//
//         UIPanel._ui = (GComponent)UIPackage.CreateObject("Basics", "Main");
//         if (UIPanel._ui != null)
//         {
//             UIPanel._ui.position = UIPanel.position;
//             if (UIPanel.scale.x != 0 && UIPanel.scale.y != 0)
//                 UIPanel._ui.scale = UIPanel.scale;
//            UIPanel._ui.rotationX = UIPanel.rotation.x;
//            UIPanel._ui.rotationY = UIPanel.rotation.y;
//            UIPanel._ui.rotation =  UIPanel.rotation.z;
//             if (UIPanel.container.hitArea != null)
//             {
//                 UIPanel.UpdateHitArea();
//                 UIPanel._ui.onSizeChanged.Add(UIPanel.UpdateHitArea);
//                 UIPanel._ui.onPositionChanged.Add(UIPanel.UpdateHitArea);
//             }
//             UIPanel.container.AddChildAt(UIPanel._ui.displayObject, 0);
//         
//             UIPanel.HandleScreenSizeChanged();
//         }
//         else
//             Debug.LogError("Create failed!");
//         
//         BasicsMain basicsMain = new BasicsMain();
//         
//         Application.targetFrameRate = 60;
//         Stage.inst.onKeyDown.Add(basicsMain.OnKeyDown);
//
//         basicsMain._mainView =  UIPanel._ui;
//
//         basicsMain._backBtn =  basicsMain._mainView.GetChild("btn_Back");
//         basicsMain._backBtn.visible = false;
//         basicsMain._backBtn.onClick.Add( basicsMain.onClickBack);
//
//         basicsMain._demoContainer =  basicsMain._mainView.GetChild("container").asCom;
//         basicsMain._viewController =  basicsMain._mainView.GetController("c1");
//
//         basicsMain._demoObjects = new Dictionary<string, GComponent>();
//
//         int cnt =  basicsMain._mainView.numChildren;
//         for (int i = 0; i < cnt; i++)
//         {
//             GObject obj =  basicsMain._mainView.GetChildAt(i);
//             if (obj.group != null && obj.group.name == "btns")
//                 obj.onClick.Add( basicsMain.runDemo);
//         }
    }
}