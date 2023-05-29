using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Combat;
using UnityEngine;

public class PickUpAble : MonoBehaviour
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
}