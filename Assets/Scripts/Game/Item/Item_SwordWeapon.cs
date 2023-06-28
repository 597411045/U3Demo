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
            picker.GetComponent<ControllerBase>().StoreItemInBag(this);
        }

        public override void OnUse(GameObject owner)
        {
            owner.GetComponent<ControllerBase>().EquipItem(this);
        }

        public override void Attack()
        {
            throw new System.NotImplementedException();
        }
    }
}