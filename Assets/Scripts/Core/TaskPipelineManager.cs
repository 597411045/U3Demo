using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using RPG.Core;
using UnityEngine;


// public class CAction
// {
//     public GameObject go;
//     public Action action;
//     public int guid;
//
//     public CAction(Action _action, int _guid, GameObject _go)
//     {
//         this.action = _action;
//         this.guid = _guid;
//         go = _go;
//     }
//
//     public void Invoke()
//     {
//         if (action != null)
//         {
//             action.Invoke();
//         }
//     }
// }

// public enum CActionType
// {
//     RecvMessage,
//     StatsMessage,
//     LocalCompute,
//     SyncData,
//     SendMessage,
//     CleanAction
// }


public interface IBaseTask
{
    bool GetIfDestroyed();
}

public interface IRecvCmd : IBaseTask
{
    void RecvCmd();
}

public interface ISyncStats : IBaseTask
{
    void SyncStats();
}

public interface ILocalCompute : IBaseTask
{
    void LocalCompute();
}

public interface ISyncData : IBaseTask
{
    void SyncData();
}

public interface ISendSyncObject : IBaseTask
{
    void SendSyncObject();
}

public class TaskPipelineManager : TaskPipelineBase<TaskPipelineManager>
{
    //public static TaskPipelineManager Ins;

    //计划阶段：接受指令-执行状态指令-本地模拟-执行同步指令-发送指令

    //接受指令
    private List<IRecvCmd> RecvCmd;

    //执行状态指令
    private List<ISyncStats> SyncStats;

    //本地模拟
    private List<ILocalCompute> LocalCompute;

    //执行同步指令
    private List<ISyncData> SyncData;

    //发送指令
    private List<ISendSyncObject> SendSyncObject;

    private List<Action> DoOnce;


    //清除任务
    //private List<CAction> CleanAction;


    //Test
    //private IEnumerator<bool> er;

    //Test
    private void Start()
    {
        //er = GetEnumYieldTimer();
        //er.MoveNext();
        //
        // TestData a= new TestData();
        // a.A = "Test";
        // a.B = 10;
        // var b=a.ToByteArray();
        // var c = a.ToByteString();
        // var d = a.ToString();
        //
        //
        // TestData a2;
        // a2 = TestData.Parser.ParseJson(d);
    }

    private void Awake()
    {
        base.Awake();
        // if (Ins == null)
        // {
        //     Debug.LogError(this.ToString() + " Awake");
        //     Ins = this;
        // }
        // else
        // {
        //     Debug.LogError("For Now, Only One " + this.ToString() + " Allowed");
        //     Destroy(this);
        // }

        RecvCmd = new List<IRecvCmd>();
        SyncStats = new List<ISyncStats>();
        LocalCompute = new List<ILocalCompute>();
        SyncData = new List<ISyncData>();
        SendSyncObject = new List<ISendSyncObject>();
        DoOnce = new List<Action>();
        //CleanAction = new List<CAction>();
    }

    private void Update()
    {
        //Test
        // if (er.Current)
        // {
        //     er.MoveNext();
        // }

        for (int i = 0; i < RecvCmd.Count; i++)
        {
            //Debug.LogError(1);
            if (!RecvCmd[i].GetIfDestroyed())
                RecvCmd[i].RecvCmd();
        }

        for (int i = 0; i < SyncStats.Count; i++)
        {
            //Debug.LogError(2);
            if (!SyncStats[i].GetIfDestroyed())
                SyncStats[i].SyncStats();
        }

        for (int i = 0; i < LocalCompute.Count; i++)
        {
            //Debug.LogError(3);
            if (!LocalCompute[i].GetIfDestroyed())
                LocalCompute[i].LocalCompute();
        }

        for (int i = 0; i < SyncData.Count; i++)
        {
            //Debug.LogError(4);
            if (!SyncData[i].GetIfDestroyed())
                SyncData[i].SyncData();
        }

        for (int i = 0; i < SendSyncObject.Count; i++)
        {
            //Debug.LogError(5);
            if (!SendSyncObject[i].GetIfDestroyed())
                SendSyncObject[i].SendSyncObject();
        }


        for (int i = DoOnce.Count - 1; i >= 0; i--)
        {
            DoOnce[i].Invoke();
            DoOnce.RemoveAt(i);
        }
    }

    //Test
    // IEnumerator Te()
    // {
    //     yield return new WaitForSeconds(2);
    //     Debug.Log("Done2");
    // }
    //float tTimer = 0;
    // IEnumerator<bool> GetEnumYieldTimer()
    // {
    //     yield return true;
    //
    //     while (true)
    //     {
    //         tTimer += Time.deltaTime;
    //         if (tTimer > 2)
    //         {
    //             Debug.Log("Done");
    //             yield return false;
    //         }
    //         else
    //         {
    //             yield return true;
    //         }
    //     }
    // }

    // public void RegisterAction(CActionType cat,CAction  ca)
    // {
    //     Debug.LogError(ca.go.name + "|" + ca.action.Method.DeclaringType + "|" + ca.action.Method.Name + " Registered");
    //     switch (cat)
    //     {
    //         case CActionType.RecvMessage:
    //         {
    //             RecvMessage.Add(ca);
    //             break;
    //         }
    //         case CActionType.StatsMessage:
    //         {
    //             SyncStats.Add(ca);
    //             break;
    //         }
    //         case CActionType.LocalCompute:
    //         {
    //             LocalCompute.Add(ca);
    //             break;
    //         }
    //         case CActionType.SyncData:
    //         {
    //             SyncData.Add(ca);
    //             break;
    //         }
    //         case CActionType.SendMessage:
    //         {
    //             SendMessage.Add(ca);
    //             break;
    //         }
    //         case CActionType.CleanAction:
    //         {
    //             CleanAction.Add(ca);
    //             break;
    //         }
    //     }
    // }

    public void Register<T>(T obj)
    {
        if (obj is IRecvCmd)
        {
            RecvCmd.Add(obj as IRecvCmd);
        }

        if (obj is ISyncStats)
        {
            SyncStats.Add(obj as ISyncStats);
        }

        if (obj is ILocalCompute)
        {
            LocalCompute.Add(obj as ILocalCompute);
        }

        if (obj is ISyncData)
        {
            SyncData.Add(obj as ISyncData);
        }

        if (obj is ISendSyncObject)
        {
            SendSyncObject.Add(obj as ISendSyncObject);
        }
    }

    public void Register(Action a)
    {
        DoOnce.Add(a);
    }
}