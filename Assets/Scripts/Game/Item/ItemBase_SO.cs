using System;
using UnityEngine;

namespace Game.Item
{
    public abstract class ItemBase_SO : ScriptableObject
    {
        [SerializeField] public GameObject PrefabInBag;
        [SerializeField] public GameObject PrefabInScene;
        [SerializeField] public GameObject PrefabInBody;
        [SerializeField] public string Name;
        [SerializeField] public string SlotTransformName;
        public GameObject go;


        private string uuid;

        protected ItemBase_SO()
        {
            uuid = System.Guid.NewGuid().ToString();
        }


        public abstract void OnPickup(GameObject picker);
        public abstract void OnUse(GameObject picker);

        public static bool operator ==(ItemBase_SO one, ItemBase_SO two)
        {
            return one.uuid == two.uuid ? true : false;
        }

        public static bool operator !=(ItemBase_SO one, ItemBase_SO two)
        {
            return one.uuid != two.uuid ? true : false;
        }
    }
}