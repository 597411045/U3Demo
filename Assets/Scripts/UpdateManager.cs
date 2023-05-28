using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CustomAction
{
    public bool isDone;
    public Action action;
    public int guid;

    public CustomAction(Action _action, int _guid)
    {
        this.action = _action;
        this.guid = _guid;
    }

    public CustomAction(Action _action)
    {
        this.action = _action;
    }
}

public class UpdateManager : MonoBehaviour
{
    private static List<CustomAction> UpdateActions = new List<CustomAction>();
    private static List<CustomAction> LateUpdateActions = new List<CustomAction>();

    private IEnumerator<bool> er;


    private void Start()
    {
        er = GetEnumYieldTimer();
        er.MoveNext();
    }

    private void Update()
    {
        // if (er.Current)
        // {
        //     er.MoveNext();
        // }

        for (int i = 0; i < UpdateActions.Count; i++)
        {
            UpdateActions[i].action.Invoke();
            //Debug.Log("Done "  + " Method" + func.Method.DeclaringType+"."+func.Method.Name);
        }

        foreach (CustomAction i in LateUpdateActions)
        {
            if (i.isDone != true)
            {
                i.action();
                i.isDone = true;
            }
        }

        for (int i = LateUpdateActions.Count - 1; i >= 0; i--)
        {
            if (LateUpdateActions[i].isDone) LateUpdateActions.RemoveAt(i);
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

    public static void RemoveActionsById(int _guid)
    {
        LateUpdateActions.Add(new CustomAction(() =>
        {
            for (int i = UpdateActions.Count - 1; i >= 0; i--)
            {
                if (UpdateActions[i].guid.Equals(_guid))
                {
                    UpdateActions.RemoveAt(i);
                }
                //Debug.Log("Done "  + " Method" + func.Method.DeclaringType+"."+func.Method.Name);
            }
        }));
    }

    public static void ClearAllActions()
    {
        LateUpdateActions.Add(new CustomAction(() => { UpdateActions.Clear(); }));
    }

    public static void RegisterAction(Action action, int guid)
    {
        UpdateActions.Add(new CustomAction(action, guid));
    }
}