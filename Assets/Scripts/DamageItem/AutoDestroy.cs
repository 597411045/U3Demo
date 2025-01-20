using System;
using UnityEngine;
using UnityEngine.Serialization;

public class AutoDestroy : MonoBehaviour
{
    public float aliveDuration = 5;

    void FixedUpdate()
    {
        if (aliveDuration > 0)
        {
            aliveDuration -= Time.fixedDeltaTime;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}