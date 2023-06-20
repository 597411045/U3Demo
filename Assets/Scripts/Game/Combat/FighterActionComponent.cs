using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPG.Core;
using RPG.Movement;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.Serialization;

namespace RPG.Combat
{
    public class FighterActionComponent : TaskPipelineBase<FighterActionComponent>, IAction, IJsonSaveable, IModifierProvider,ILocalCompute
    {
        [SerializeField] private bool isFsmControlled;

        [SerializeField] private Transform handTransform;

        [FormerlySerializedAs("_weapon")] [SerializeField]
        public WeaponConfig weaponConfig;

        [SerializeField] public Transform weaponTR;

        public Transform target;
        public float TimeLeftToAttackAction = 0f;

        private void Awake()
        {
            weaponConfig.Spawn(this.transform, this.GetComponent<Animator>(), out weaponTR);
        }

        private void OnLocalCompute()
        {
            if (TimeLeftToAttackAction >= 0)
            {
                TimeLeftToAttackAction -= Time.deltaTime;
            }

            if (target == null) return;

            if (target.GetComponent<HealthComponent>().IsDead) return;

            if (!GetIfInRange())
            {
                this.GetComponent<NavMoveComponent>().MoveToPosition(target.position);
            }
            else
            {
                this.GetComponent<NavMoveComponent>().Cancel();
                AttackAction();
            }
        }


        private void AttackAction()
        {
            this.transform.LookAt(target.transform);
            if (TimeLeftToAttackAction <= 0)
            {
                this.GetComponent<Animator>().ResetTrigger("StopAttack");
                this.GetComponent<Animator>().SetTrigger("IfAttack");
                TimeLeftToAttackAction = weaponConfig.attackInterval;
            }
        }

        public void ActAttack()
        {
            if (TimeLeftToAttackAction <= 0)
            {
                this.GetComponent<Animator>().ResetTrigger("StopAttack");
                this.GetComponent<Animator>().SetTrigger("IfAttack");
                TimeLeftToAttackAction = weaponConfig.attackInterval;
            }
        }

        private bool GetIfInRange()
        {
            return Vector3.Distance(transform.position, target.position) < weaponConfig.weaponRange;
        }

        public bool TryMakeTargetBeAttackTarget(CombatAbleComponent cac, float speed = 0)
        {
            if (cac == null)
            {
                target = null;
                return true;
            }

            if (cac.GetComponent<HealthComponent>().IsDead) return false;

            if (speed != 0)
            {
                this.GetComponent<NavMeshAgent>().speed = speed;
            }

            this.GetComponent<ActionSchedulerComponent>().StartAction(this);
            target = cac.transform;
            return true;
        }

        public void Cancel()
        {
            this.GetComponent<Animator>().SetTrigger("StopAttack");
            target = null;
        }

        private void Hit()
        {
            if (target == null) return;
            target.GetComponent<HealthComponent>()
                .TakeDamage(this.GetComponent<BaseStats>().GetAllAdditiveModifier(ProgressionEnum.Damage),
                    this.gameObject);
            if (weaponConfig.hitAudio != null)
            {
                weaponTR.GetComponent<AudioSource>().clip = weaponConfig.hitAudio;
                weaponTR.GetComponent<AudioSource>().Play();
            }
        }

        private void Shoot()
        {
            if (target == null) return;
            GameObject go = Instantiate(weaponConfig.projectilePrefab);
            go.GetComponent<Projectile>().atk =
                this.GetComponent<BaseStats>().GetAllAdditiveModifier(ProgressionEnum.Damage);
            //go.GetComponent<Projectile>().isAutoNav = true;
            go.GetComponent<Projectile>().Target = target.gameObject;
            go.GetComponent<Projectile>().launcher = this.gameObject;

            go.transform.position = weaponTR.position;
            go.transform.position += (target.transform.position - this.transform.position).normalized * 0.5f;

            go.GetComponent<Projectile>().direction =
                (target.transform.position + new Vector3(0, 1, 0) - go.transform.position).normalized;
        }


        public void EquipItem(WeaponConfig weaponConfig)
        {
            this.weaponConfig.Drop(weaponTR);
            this.weaponConfig = weaponConfig;
            this.weaponConfig.Spawn(this.transform, this.GetComponent<Animator>(), out weaponTR);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(this.transform.position, weaponConfig.weaponRange);
        }

        public JToken CaptureAsJTokenInInterface()
        {
            if (gameObject.tag != "Player") return null;

            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;
            stateDict["weaponPrefabName"] = weaponConfig.prefabName;
            return state;
        }

        public void RestoreFormJToken(JToken state)
        {
            if (gameObject.tag != "Player") return;

            JObject s = state.ToObject<JObject>();
            IDictionary<string, JToken> stateDict = s;
            string str = stateDict["weaponPrefabName"].ToObject<string>();
            EquipItem(Resources.Load<WeaponConfig>(str));
        }

        public float GetTargetHP()
        {
            if (target == null) return 0;
            return target.GetComponent<BaseStats>().HP;
        }

        public IEnumerable<float> GetAdditiveModifier(ProgressionEnum e)
        {
            if (e == ProgressionEnum.Damage)
            {
                yield return weaponConfig.weaponDamage;
            }
        }

        public IEnumerable<float> GetPercentageModifier(ProgressionEnum b)
        {
            yield return 1f;
        }

      
        
        private void Start()
        {
            if (!isFsmControlled)
            {
                base.Start();
            }
        }
      
        void ILocalCompute.LocalCompute()
        {
            OnLocalCompute();
        }
    }
}