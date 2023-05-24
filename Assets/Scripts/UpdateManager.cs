using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateManager : MonoBehaviour
{
    public static List<Action> UpdateActions = new List<Action>();

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
        
        foreach (Action func in UpdateActions)
        {
            func();
            //Debug.Log("Done "  + " Method" + func.Method.DeclaringType+"."+func.Method.Name);
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
}