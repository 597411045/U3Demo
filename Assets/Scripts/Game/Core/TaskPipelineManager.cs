using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using RPG.Core;
using UnityEngine;

public class TaskPipelineManager : MonoBehaviour
{
    public static TaskPipelineManager SingleTon;

    public Dictionary<string, Action> NetRecvActions;

    public Dictionary<string, Action> LocalActions;
    
    public Dictionary<string, Action> NetSendActions;

    public TaskPipelineManager()
    {
        if (SingleTon == null)
        {
            SingleTon = this;
            NetRecvActions = new Dictionary<string, Action>();
            LocalActions = new Dictionary<string, Action>();
            NetSendActions = new Dictionary<string, Action>();
        }
    }

    private void Update()
    {

        foreach (var i in NetRecvActions)
        {
            i.Value.Invoke();
        }
        
        foreach (var i in LocalActions)
        {
            i.Value.Invoke();
        }
        
        foreach (var i in NetSendActions)
        {
            i.Value.Invoke();
        }
    }
}