using RPG.Combat;
using RPG.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Item
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Item/Old Weapon", order = 0)]
    public class WeaponConfig : ItemBase_SO
    {
        [SerializeField] public AnimatorOverrideController aoc = null;
        [SerializeField] public GameObject projectilePrefab = null;
        [SerializeField] public float weaponRange = 2f;
        [SerializeField] public float attackInterval = 2f;
        [SerializeField] public float weaponDamage = 10f;

        [FormerlySerializedAs("transformName")] [SerializeField]
        public string SlotName;

        [SerializeField] public AudioClip hitAudio;
        [SerializeField] public AudioClip launchAudio;
        public override void OnPickup(GameObject picker)
        {
            throw new System.NotImplementedException();
        }

        public override void OnUse(GameObject picker)
        {
            throw new System.NotImplementedException();
        }
    }
}