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

    public UIManagement _uiManagement;
    public ResourceManagement _resourceManagement;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
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
