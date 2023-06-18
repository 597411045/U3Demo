using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject,IItemAction
    {
        [SerializeField] private AnimatorOverrideController aoc = null;
        [SerializeField] private GameObject weaponPrefab = null;
        [SerializeField] public GameObject dropPrefab = null;
        [SerializeField] public GameObject projectilePrefab = null;
        [SerializeField] public float weaponRange = 2f;
        [SerializeField] public float attackInterval = 2f;
        [SerializeField] public float weaponDamage = 10f;
        [SerializeField] private string transformName;
        [SerializeField] public string prefabName;
        [SerializeField] public AudioClip hitAudio;


        public void Spawn(Transform tf, Animator anim, out Transform slot)
        {
            slot = null;
            tf.FindAlongChild(transformName, out slot);

            if (weaponPrefab != null)
            {
                GameObject go = Instantiate(weaponPrefab);
                //weaponPrefab = Instantiate(Resources.Load<GameObject>("SwordSlot"));
                go.transform.SetParent(slot);
                go.transform.localPosition = new Vector3(0, 0, 0);
                go.transform.localRotation = Quaternion.identity;
                slot = go.transform;
            }

            if (aoc != null)
            {
                anim.runtimeAnimatorController = aoc;
            }
            else
            {
                AnimatorOverrideController a = anim.runtimeAnimatorController as AnimatorOverrideController;
                if (a != null)
                {
                    anim.runtimeAnimatorController = a.runtimeAnimatorController; 
                }
            }
        }

        public void Drop(Transform tf)
        {
            if (dropPrefab != null)
            {
                GameObject go = Instantiate(dropPrefab);
                go.GetComponent<PickUpAble>().timer = 2;
                go.transform.position = tf.transform.position;
                go.GetComponent<Rigidbody>().AddForce(new Vector3(0, 2, 0), ForceMode.Impulse);

                if (weaponPrefab != null)
                {
                    Destroy(tf.gameObject);
                }
            }
        }

        public void DoAction(GameObject go)
        {
            go.GetComponent<FighterActionComponent>().EquipItem(this);
        }
    }
}