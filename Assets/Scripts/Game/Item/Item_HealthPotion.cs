using RPG.Control;
using RPG.Stats;
using UnityEngine;

namespace Game.Item
{
    [CreateAssetMenu(fileName = "Potion", menuName = "Item/Make New HealthPotion")]
    public class Item_HealthPotion : ItemBase_SO
    {
        [SerializeField] public float value;

        public override void OnPickup(GameObject picker)
        {
            picker.GetComponent<ControllerBase>().StoreItemInBag(this);
        }

        public override void OnUse(GameObject owner)
        {
            owner.GetComponent<ControllerBase>().RecoverHealth(value);
        }

        
    }
}