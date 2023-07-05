using System;
using System.Collections.Generic;
using Game.Item;
using RPG.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.Control
{
    //综合的实体行为
    public class ControllerBase : TaskPipelineBase, ILocalCompute
    {
        public List<ItemBase_SO> Bag;
        public ItemBase_Weapon CurrentWeapon;
        public float AttactTimer;

        protected void Awake()
        {
            Bag = new List<ItemBase_SO>();
            AttactTimer = 0;

            if (CurrentWeapon != null)
            {
                EquipItem(CurrentWeapon);
            }
        }

        public void DropItem(ItemBase_SO item)
        {
            GenerateItemPrefabInScene(item);
            RemoveItemModel(item);
            RemoveItemInBag(item);
        }

        private void RemoveItemInBag(ItemBase_SO item)
        {
            if (!Bag.Exists((_itemInBag) => { return _itemInBag == item ? true : false; })) return;

            //移除背包和当前索引
            Bag.Remove(item);
            if (CurrentWeapon == item)
            {
                CurrentWeapon = null;
            }
        }

        private void GenerateItemPrefabInScene(ItemBase_SO item)
        {
            if (item.PrefabInScene == null) return;
            //地面生成物品拾取prefab
            GameObject go = Instantiate(item.PrefabInScene);
            go.GetComponent<PickUpAble>().timer = 2;
            go.transform.position = this.transform.position + this.transform.forward;
            go.GetComponent<Rigidbody>().AddForce(new Vector3(0, 5, 0), ForceMode.Impulse);
            item.go = go;
        }

        private void GenerateItemPrefabInTransform(ItemBase_SO item)
        {
            if (item.PrefabInBody == null) return;
            //地面生成物品拾取prefab
            //在骨骼里生成模型
            Transform slotTransform;
            Util.FindAlongChild(this.transform, item.SlotTransformName, out slotTransform, true);
            GameObject go = Instantiate(item.PrefabInBody);
            go.transform.SetParent(slotTransform);
            go.transform.localPosition = new Vector3(0, 0, 0);
            go.transform.localRotation = Quaternion.identity;
            item.go = go;
        }

        private void RemoveItemModel(ItemBase_SO item)
        {
            if (item.SlotTransformName == "") return;
            //移除骨骼中武器模型
            Transform slotTransform;
            Util.FindAlongChild(this.transform, item.SlotTransformName, out slotTransform, true);
            for (int i = 0; i < slotTransform.childCount; i++)
            {
                Transform childTransform = slotTransform.GetChild(i);
                string childTransformName = childTransform.gameObject.name;
                if (childTransformName.Contains("Slot"))
                {
                    Destroy(slotTransform.GetChild(i).gameObject);
                    break;
                }
            }
        }

        public void EquipItem(ItemBase_Weapon weapon)
        {
            if (CurrentWeapon != null)
            {
                RemoveItemModel(CurrentWeapon);
            }

            Animator anim = this.GetComponent<Animator>();
            //替换controller动画
            if (weapon.aoc != null)
            {
                anim.runtimeAnimatorController = weapon.aoc;
            }
            // else
            // {
            //     //?
            //     AnimatorOverrideController a = anim.runtimeAnimatorController as AnimatorOverrideController;
            //     if (a != null)
            //     {
            //         anim.runtimeAnimatorController = a.runtimeAnimatorController;
            //     }
            // }

            GenerateItemPrefabInTransform(weapon);
            CurrentWeapon = weapon;
        }


        public void StoreItemInBag(ItemBase_SO item)
        {
            //临时，丢弃现有武器，装备新武器
            DropItem(CurrentWeapon);
            Debug.Log("drop item");
            EquipItem((ItemBase_Weapon)item);
            Debug.Log("equip item");


            return;
            if (Bag.Exists((_itemInBag) => { return _itemInBag == item ? true : false; })) return;

            //背包中添加索引
            Bag.Add(item);
            //TODO：背包系统UI
        }

        public void RecoverHealth(float value)
        {
            throw new NotImplementedException();
        }

        public void Attack()
        {
            if (AttactTimer <= 0)
            {
                this.GetComponent<Animator>().ResetTrigger("StopAttack");
                this.GetComponent<Animator>().SetTrigger("IfAttack");
                AttactTimer = CurrentWeapon.attackInterval;
                CurrentWeapon.WeaponAttackAction();
            }


            // if (target == null) return;
            // target.GetComponent<HealthComponent>()
            //     .TakeDamage(this.GetComponent<BaseStats>().GetAllAdditiveModifier(ProgressionEnum.Damage),
            //         this.gameObject);
        }

        public void LocalCompute()
        {
            if (AttactTimer >= 0)
            {
                AttactTimer -= Time.deltaTime;
                Debug.Log("Attack cooling");
            }
        }
    }
}