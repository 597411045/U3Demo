using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Control;
using RPG.Core;
using UnityEngine;
using UnityEngine.Serialization;

public enum ItemType
{
    Weapon,
    Consume
}


public class PickUpAble : TaskPipelineBase<PickUpAble>, IRayCastAble, ILocalCompute
{
    [FormerlySerializedAs("_weapon")] [SerializeField]
    private WeaponConfig weaponConfig = null;

    [SerializeField] private ConsumeConfig consumeConfig;

    public float timer = 0;

    void Awake()
    {
        
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (timer >= 0) return;
        if (collision.gameObject.tag.Equals("Player") || collision.gameObject.name.Contains("Enemy"))
        {
            weaponConfig?.DoAction(collision.gameObject);
            consumeConfig?.DoAction(collision.gameObject);
            Destroy(this.gameObject);
        }
    }

    public bool HandleRaycaset(PlayerController p, RaycastHit h)
    {
        p.SetCursor(CursorType.PickUp);
        if (Input.GetMouseButtonDown(0))
        {
            weaponConfig?.DoAction(p.gameObject);
            consumeConfig?.DoAction(p.gameObject);

            Destroy(this.gameObject);
        }

        return true;
    }

    public void LocalCompute()
    {
        if (timer >= 0)
        {
            timer -= Time.deltaTime;
        }
    }

}