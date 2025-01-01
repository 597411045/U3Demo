using System;
using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

public class GameModeScript : MonoBehaviour
{   
    public static GameModeScript instance;
    
    public long RenderFrame;
    public long PhysicsFrame;

    public UIPanel UIPanel;
    
    private UIManagement _uiManagement;
    private ResourceManagement _resourceManagement;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        _resourceManagement = new ResourceManagement();
        
        _uiManagement = new UIManagement();
        _uiManagement.UIPanel = UIPanel;
        _uiManagement.CusAwake();
        
    }

    // Update is called once per frame
    void Update()
    {
        RenderFrame++;
    }

    private void FixedUpdate()
    {
        PhysicsFrame++;
    }
}
