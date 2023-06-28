using RPG.Control;
using RPG.Stats;
using UnityEngine;

namespace Game.Item
{
    public abstract class ItemBase_Weapon : ItemBase_SO
    {
        [SerializeField] public AnimatorOverrideController aoc = null;
        [SerializeField] public GameObject projectilePrefab = null;
        [SerializeField] public float weaponRange = 2f;
        [SerializeField] public float attackInterval = 2f;
        [SerializeField] public float weaponDamage = 10f;
        [SerializeField] public AudioClip hitAudio;
        [SerializeField] public AudioClip launchAudio;

        public abstract void Attack();
    }
}