using System;
using System.Collections;
using System.Collections.Generic;
using Game.Item;
using Newtonsoft.Json.Linq;
using RPG.Control;
using RPG.Core;
using RPG.Movement;
using RPG.Stats;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.Serialization;

namespace RPG.Combat
{
    public class FighterActionComponent : MonoBehaviour
    {
        [SerializeField] private bool isFsmControlled;

        public GameObject target;
        public float TimeLeftToAttackAction = 0f;
        private ControllerBase controller;

        private void Awake()
        {
            controller = this.GetComponent<ControllerBase>();
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
                this.GetComponent<NavMoveComponent>().MoveToPosition(target.transform.position);
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
                TimeLeftToAttackAction = controller.CurrentWeapon.attackInterval;
            }
        }

        public void ActAttack()
        {
           
        }

        private bool GetIfInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) <
                   controller.CurrentWeapon.weaponRange;
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

            //this.GetComponent<ActionSchedulerComponent>().StartAction(this);
            target = cac.transform.gameObject;
            return true;
        }

        public void Cancel()
        {
            this.GetComponent<Animator>().SetTrigger("StopAttack");
            target = null;
        }

        private void Hit()
        {
            
        }

        private void Shoot()
        {
            if (target == null) return;
            GameObject go = Instantiate(controller.CurrentWeapon.projectilePrefab);
            go.GetComponent<Projectile>().atk =
                this.GetComponent<BaseStats>().GetAllAdditiveModifier(ProgressionEnum.Damage);
            //go.GetComponent<Projectile>().isAutoNav = true;
            go.GetComponent<Projectile>().Target = target.gameObject;
            go.GetComponent<Projectile>().launcher = this.gameObject;

            go.transform.position = controller.CurrentWeapon.go.transform.position;
            go.transform.position += (target.transform.position - this.transform.position).normalized * 0.5f;

            go.GetComponent<Projectile>().direction =
                (target.transform.position + new Vector3(0, 1, 0) - go.transform.position).normalized;
        }


        // public void EquipItem(WeaponConfig weaponConfig)
        // {
        //     
        // }

        private void OnDrawGizmos()
        {
            if (controller == null) return;
            if (controller.CurrentWeapon == null) return;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(this.transform.position, controller.CurrentWeapon.weaponRange);
        }

        public JToken CaptureAsJTokenInInterface()
        {
            if (gameObject.tag != "Player") return null;

            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;
            stateDict["weaponPrefabName"] = controller.CurrentWeapon.PrefabInScene.name.ToString();
            return state;
        }

        public void RestoreFormJToken(JToken state)
        {
            if (gameObject.tag != "Player") return;

            JObject s = state.ToObject<JObject>();
            IDictionary<string, JToken> stateDict = s;
            string str = stateDict["weaponPrefabName"].ToObject<string>();
            controller.EquipItem(Resources.Load<ItemBase_Weapon>(str));
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
                yield return controller.CurrentWeapon.weaponDamage;
            }
        }

        public IEnumerable<float> GetPercentageModifier(ProgressionEnum b)
        {
            yield return 1f;
        }
    }
}