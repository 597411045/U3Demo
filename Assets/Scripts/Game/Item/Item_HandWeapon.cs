using RPG.Control;
using RPG.Stats;
using UnityEngine;

namespace Game.Item
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Item/Make New HandWeapon")]
    public class Item_HandWeapon : ItemBase_Weapon
    {
        public override void OnPickup(GameObject picker)
        {
            //picker.GetComponent<ControllerBase>().StoreItemInBag(this);
            Debug.Log("Item_HandWeapon OnPickUp");
        }

        public override void OnUse(GameObject owner)
        {
            //owner.GetComponent<ControllerBase>().EquipItem(this);
            Debug.Log("Item_HandWeapon OnUse");

        }

        public override void WeaponAttackAction()
        {
            if (hitAudio != null)
            {
                go.GetComponent<AudioSource>().clip = hitAudio;
                go.GetComponent<AudioSource>().Play();
            }
            Debug.Log("Item_HandWeapon WeaponAttackAction");

        }
    }
}