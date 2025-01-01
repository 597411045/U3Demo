using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeScript : MonoBehaviour
{
    public long RenderFrame;
    public long PhysicsFrame;

    private UIManagement _uiManagement;
    void Start()
    {
        _uiManagement = new UIManagement();
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
