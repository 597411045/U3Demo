using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Control;
using RPG.Core;
using UnityEngine;

public class PickUpAble : MonoBehaviour, IRayCastAble
{
    [SerializeField] private Weapon _weapon = null;

    public float timer = 0;

    private void Update()
    {
        if (timer >= 0)
        {
            timer -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (timer >= 0) return;
        if (collision.gameObject.tag.Equals("Player") || collision.gameObject.name.Contains("Enemy"))
        {
            collision.GetComponent<FighterActionComponent>().EquipItem(_weapon);
            Destroy(this.gameObject);
        }
    }


    public bool HandleRaycaset(PlayerController p, RaycastHit h)
    {
        p.SetCursor(CursorType.PickUp);
        if (Input.GetMouseButtonDown(0))
        {
            p.GetComponent<FighterActionComponent>().EquipItem(_weapon);
            Destroy(this.gameObject);
        }

        return true;
    }
}