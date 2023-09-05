using RPG.Control;
using RPG.Stats;
using UnityEngine;

namespace Game.Item
{
    [CreateAssetMenu(fileName = "SwordWeapon", menuName = "Item/Make New SwordWeapon")]
    public class Item_SwordWeapon : ItemBase_Weapon
    {
        public override void OnPickup(GameObject picker)
        {
            //picker.GetComponent<ControllerBase>().StoreItemInBag(this);
            Debug.Log("Item_SwordWeapon OnPickup");
        }

        public override void OnUse(GameObject owner)
        {
            //owner.GetComponent<ControllerBase>().EquipItem(this);
            Debug.Log("Item_SwordWeapon OnUse");
        }

        public override void WeaponAttackAction()
        {
            if (hitAudio != null)
            {
                go.GetComponent<AudioSource>().clip = hitAudio;
                go.GetComponent<AudioSource>().Play();
            }

            Debug.Log("Item_SwordWeapon WeaponAttackAction");
        }
    }
}