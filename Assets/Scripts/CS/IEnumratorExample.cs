using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class IEnumratorExample
{
    public void StartEnum(IEnumerator<int> er)
    {
        er = GetEnumYield();
        er.MoveNext();
        int a = er.Current;
        Debug.Log(a);
    }

    public void StartEnumWhile()
    {
        IEnumerator<int> er = GetEnum();
        while (er.MoveNext())
        {
            int a = er.Current;
            Debug.Log(a);
        }
    }

    public static IEnumerator<int> GetEnum()
    {
        List<int> list = new List<int>();
        for (int i = 0; i < 10; i++)
        {
            list.Add(i);
        }

        return list.GetEnumerator();
    }

    public static IEnumerator<int> GetEnumYield()
    {
        for (int i = 0; i < 10; i++)
        {
            yield return i;
        }
    }

    public static float tTimer = 0;

    public static IEnumerator<float> GetEnumYieldTimer()
    {
        while (true)
        {
            tTimer += Time.deltaTime;
            yield return tTimer;
        }
    }
}