using System;
using System.Collections.Generic;
using UnityEngine;

public class PortalItem : MonoBehaviour
{
    public string SceneName;
    public BoxCollider collider;

    private void Awake()
    {
        GameMode.Instance.OnEnemyClearActions.Add(() => { collider.isTrigger = true; });
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameMode.Instance.ChangeScene(SceneName);
        }
    }
}