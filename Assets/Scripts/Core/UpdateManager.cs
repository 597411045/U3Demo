using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
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

public class UpdateManager : MonoBehaviour
{
    public static List<CAction> LocalCompute = new List<CAction>();
    public static List<CAction> ServerAsyn = new List<CAction>();
    public static List<CAction> SendToServer = new List<CAction>();
    public static List<CAction> CleanAction = new List<CAction>();

    private IEnumerator<bool> er;

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

    private void Update()
    {
        // if (er.Current)
        // {
        //     er.MoveNext();
        // }

        for (int i = CleanAction.Count - 1; i >= 0; i--)
        {
            CleanAction[i].Invoke();
            CleanAction.RemoveAt(i);
        }

        for (int i = 0; i < LocalCompute.Count; i++)
        {
            LocalCompute[i].Invoke();
            //Debug.Log("Done "  + " Method" + func.Method.DeclaringType+"."+func.Method.Name);
        }

        for (int i = 0; i < ServerAsyn.Count; i++)
        {
            ServerAsyn[i].Invoke();
        }

        for (int i = 0; i < SendToServer.Count; i++)
        {
            SendToServer[i].Invoke();
        }

        for (int i = CleanAction.Count - 1; i >= 0; i--)
        {
            CleanAction[i].Invoke();
            CleanAction.RemoveAt(i);
        }
    }

    IEnumerator Te()
    {
        yield return new WaitForSeconds(2);
        Debug.Log("Done2");
    }

    float tTimer = 0;

    IEnumerator<bool> GetEnumYieldTimer()
    {
        yield return true;

        while (true)
        {
            tTimer += Time.deltaTime;
            if (tTimer > 2)
            {
                Debug.Log("Done");
                yield return false;
            }
            else
            {
                yield return true;
            }
        }
    }

    // public static void RemoveActionsById(int _guid)
    // {
    //     LateUpdateActions.Add(new CustomAction(() =>
    //     {
    //         for (int i = UpdateActions.Count - 1; i >= 0; i--)
    //         {
    //             if (UpdateActions[i].guid.Equals(_guid))
    //             {
    //                 UpdateActions.RemoveAt(i);
    //             }
    //             //Debug.Log("Done "  + " Method" + func.Method.DeclaringType+"."+func.Method.Name);
    //         }
    //     }));
    // }
    //
    public static void ClearAllActions()
    {
        CleanAction.Add(new CAction(() => { LocalCompute.Clear(); }, 0, null));
    }

    public static void ClearAllByGameobjectId(int goid)
    {
        CleanAction.Add(new CAction(() =>
        {
            for (int i = LocalCompute.Count - 1; i >= 0; i--)
            {
                if (LocalCompute[i].go.GetInstanceID() == goid)
                {
                    UpdateManager.LocalCompute.RemoveAt(i);
                }
            }
        }, 0, null));
    }

    public static void ClearAllByActionId(int acid)
    {
        CleanAction.Add(new CAction(() =>
        {
            for (int i = LocalCompute.Count - 1; i >= 0; i--)
            {
                if (LocalCompute[i].guid == acid)
                {
                    UpdateManager.LocalCompute.RemoveAt(i);
                }
            }
        }, 0, null));
    }

    // public static void RegisterAction(Action action, int guid)
    // {
    //     UpdateActions.Add(new CustomAction(action, guid));
    // }
}