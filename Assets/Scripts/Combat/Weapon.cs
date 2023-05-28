using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [SerializeField] private AnimatorOverrideController aoc = null;
        [SerializeField] private GameObject weaponPrefab = null;
        [SerializeField] public float weaponRange = 2f;
        [SerializeField] public float attackInterval = 2f;
        [SerializeField] public float weaponDamage = 10f;

        public void Spawn(Transform tf, Animator anim)
        {
            if (weaponPrefab != null)
            {
                tf.FindAlongChild("Hand_R", out tf);
                //weaponPrefab = Instantiate(Resources.Load<GameObject>("SwordSlot"));
                GameObject go = Instantiate(weaponPrefab);
                go.transform.SetParent(tf);
                go.transform.localPosition = new Vector3(0, 0, 0);
                go.transform.localRotation = Quaternion.identity;
            }

            if (aoc != null) anim.runtimeAnimatorController = aoc;
        }
    }
}