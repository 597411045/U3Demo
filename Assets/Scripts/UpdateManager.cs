using System;
using System.Collections.Generic;
using UnityEngine;

public class UpdateManager : MonoBehaviour
{
    public static List<Action> UpdateActions = new List<Action>();
    private int i;

    private void Update()
    {
        i = 0;
        foreach (Action func in UpdateActions)
        {
            func();
            i++;
            //Debug.Log("Done " + i + " Method" + func.Method.DeclaringType+"."+func.Method.Name);
        }
    }
}