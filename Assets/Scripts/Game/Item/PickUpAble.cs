using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Control;
using RPG.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Item
{
    public enum ItemType
    {
        Weapon,
        Consume
    }


    public class PickUpAble : MonoBehaviour, IRayCastAble
    {
        public float timer = 0;
        [FormerlySerializedAs("item")] public ItemBase_SO itemBase;

        //触发器拾取模式
        private void OnTriggerEnter(Collider collision)
        {
            if (timer >= 0) return;
            if (collision.gameObject.tag.Equals("Player"))
            {
                itemBase.OnPickup(collision.gameObject);
                //拾取后删除物体拾取预制体
                Destroy(this.gameObject);
            }
        }

        //鼠标点击拾取模式
        public bool HandleRaycaset(PlayerController p, RaycastHit h)
        {
            p.SetCursor(CursorType.PickUp);
            if (Input.GetMouseButtonDown(0))
            {
                Destroy(this.gameObject);
            }

            return true;
        }

        //拾取预制体生成2秒后才能拾取
        public void Update()
        {
            if (timer >= 0)
            {
                timer -= Time.deltaTime;
            }
        }
    }
}