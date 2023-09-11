using System;
using System.Collections;
using System.Collections.Generic;
using CS.Log;
using Google.Protobuf;
using RPG.Core;
using UnityEngine;

public class TaskPipelineManager : MonoBehaviour
{
    public static TaskPipelineManager SingleTon;

    public Dictionary<string, Action> PreActions;

    public Dictionary<string, Action> LocalActions;

    public Dictionary<string, Action> EndActions;

    public void Awake()
    {
        if (SingleTon == null)
        {
            SingleTon = this;
            PreActions = new Dictionary<string, Action>();
            LocalActions = new Dictionary<string, Action>();
            EndActions = new Dictionary<string, Action>();
        }
    }

    private void Update()
    {
        foreach (var i in PreActions)
        {
            i.Value.Invoke();
        }

        foreach (var i in LocalActions)
        {
            i.Value.Invoke();
        }

        foreach (var i in EndActions)
        {
            i.Value.Invoke();
        }
    }
   
}