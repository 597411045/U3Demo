using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using RPG.Core;
using UnityEngine;


public class CAction
{
    public GameObject go;
    public Action action;
    public int guid;

    public CAction(Action _action, int _guid, GameObject _go)
    {
        this.action = _action;
        this.guid = _guid;
        go = _go;
    }

    public void Invoke()
    {
        if (action != null)
        {
            action.Invoke();
        }
    }
}

public enum CActionType
{
    RecvMessage,
    StatsMessage,
    LocalCompute,
    SyncData,
    SendMessage,
    CleanAction
}

public class UpdateManager : MonoBehaviour
{
    public static UpdateManager Ins;

    //计划阶段：接受指令-执行状态指令-本地模拟-执行同步指令-发送指令

    //接受指令
    private List<CAction> RecvMessage;

    //执行状态指令
    private List<CAction> StatsMessage;

    //本地模拟
    private List<CAction> LocalCompute;

    //执行同步指令
    private List<CAction> SyncData;

    //发送指令
    private List<CAction> SendMessage;

    //清除任务
    private List<CAction> CleanAction;


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
        if (Ins == null)
        {
            Debug.LogError(this.ToString() + " Awake");
            Ins = this;
        }
        else
        {
            Debug.LogError("For Now, Only One " + this.ToString() + " Allowed");
            Destroy(this);
        }

        RecvMessage = new List<CAction>();
        StatsMessage = new List<CAction>();
        LocalCompute = new List<CAction>();
        SyncData = new List<CAction>();
        SendMessage = new List<CAction>();
        CleanAction = new List<CAction>();
    }

    private void Update()
    {
        //Test
        // if (er.Current)
        // {
        //     er.MoveNext();
        // }

        //防止新删除任务的Action还未执行，开始轮询前再检查删除Action的Task
        for (int i = CleanAction.Count - 1; i >= 0; i--)
        {
            //Debug.LogError(0);
            CleanAction[i].Invoke();
            CleanAction.RemoveAt(i);
        }

        for (int i = 0; i < RecvMessage.Count; i++)
        {
            //Debug.LogError(1);
            RecvMessage[i].Invoke();
        }

        for (int i = 0; i < StatsMessage.Count; i++)
        {
            //Debug.LogError(2);
            StatsMessage[i].Invoke();
        }

        for (int i = 0; i < LocalCompute.Count; i++)
        {
            //Debug.LogError(3);
            LocalCompute[i].Invoke();
        }

        for (int i = 0; i < SyncData.Count; i++)
        {
            //Debug.LogError(4);
            SyncData[i].Invoke();
        }

        for (int i = 0; i < SendMessage.Count; i++)
        {
            //Debug.LogError(5);
            SendMessage[i].Invoke();
        }

        for (int i = CleanAction.Count - 1; i >= 0; i--)
        {
            //Debug.LogError(6);
            CleanAction[i].Invoke();
            CleanAction.RemoveAt(i);
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

    public void RegisterAction(CActionType cat, CAction ca)
    {
        Debug.LogError(ca.go.name + "|" + ca.action.Method.DeclaringType + "|" + ca.action.Method.Name + " Registered");
        switch (cat)
        {
            case CActionType.RecvMessage:
            {
                RecvMessage.Add(ca);
                break;
            }
            case CActionType.StatsMessage:
            {
                StatsMessage.Add(ca);
                break;
            }
            case CActionType.LocalCompute:
            {
                LocalCompute.Add(ca);
                break;
            }
            case CActionType.SyncData:
            {
                SyncData.Add(ca);
                break;
            }
            case CActionType.SendMessage:
            {
                SendMessage.Add(ca);
                break;
            }
            case CActionType.CleanAction:
            {
                CleanAction.Add(ca);
                break;
            }
        }
    }

    public void ClearAllLocalCompute()
    {
        Debug.LogError("ClearAllLocalCompute");

        CleanAction.Add(new CAction(() => { LocalCompute.Clear(); }, 0, null));
    }

    public void ClearLocalComputelByGameobjectId(int goid)
    {
        CleanAction.Add(new CAction(() =>
        {
            for (int i = LocalCompute.Count - 1; i >= 0; i--)
            {
                if (LocalCompute[i].go.GetInstanceID() == goid)
                {
                    //Debug.LogError(LocalCompute[i].action.Method.DeclaringType + "|" +LocalCompute[i].action.Method.Name + " Clear");
                    LocalCompute.RemoveAt(i);
                }
            }
        }, 0, null));
    }

    public void ClearLocalComputeByActionId(int acid)
    {
        CleanAction.Add(new CAction(() =>
        {
            for (int i = LocalCompute.Count - 1; i >= 0; i--)
            {
                if (LocalCompute[i].guid == acid)
                {
                    LocalCompute.RemoveAt(i);
                }
            }
        }, 0, null));
    }

    private void OnDestroy()
    {
        ClearAllLocalCompute();
    }
}