using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPG.Core;
using RPG.Movement;
using RPG.Saving;
using RPG.Stats;
using UI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

namespace RPG.Combat
{
    public class FighterActionComponent : MonoBehaviour, IAction, IJsonSaveable, IModifierProvider
    {
        [SerializeField] private bool isFsmControlled;

        [SerializeField] private Transform handTransform;
        [SerializeField] public Weapon _weapon;

        public Transform target;
        public float TimeLeftToAttackAction = 0f;

        private void Awake()
        {
            _weapon.Spawn(this.transform, this.GetComponent<Animator>(), out shotPoint);
            if (isFsmControlled) return;
            UpdateManager.RegisterAction(UpdateMethod, this.gameObject.GetHashCode());
        }

        private void UpdateMethod()
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
                TimeLeftToAttackAction = _weapon.attackInterval;
            }
        }

        private bool GetIfInRange()
        {
            return Vector3.Distance(transform.position, target.position) < _weapon.weaponRange;
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
        }

        private void Shoot()
        {
            if (target == null) return;
            GameObject go = Instantiate(_weapon.projectilePrefab);
            go.GetComponent<Projectile>().atk =
                this.GetComponent<BaseStats>().GetAllAdditiveModifier(ProgressionEnum.Damage);
            //go.GetComponent<Projectile>().isAutoNav = true;
            go.GetComponent<Projectile>().Target = target.gameObject;
            go.GetComponent<Projectile>().launcher = this.gameObject;

            go.transform.position = shotPoint.position;
            go.transform.position += (target.transform.position - this.transform.position).normalized * 0.5f;

            go.GetComponent<Projectile>().direction =
                (target.transform.position + new Vector3(0, 1, 0) - go.transform.position).normalized;
        }

        private Transform shotPoint;

        public void EquipItem(Weapon weapon)
        {
            this._weapon.Drop(shotPoint);
            this._weapon = weapon;
            _weapon.Spawn(this.transform, this.GetComponent<Animator>(), out shotPoint);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(this.transform.position, _weapon.weaponRange);
        }

        public JToken CaptureAsJTokenInInterface()
        {
            if (gameObject.tag != "Player") return null;

            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;
            stateDict["weaponPrefabName"] = _weapon.prefabName;
            return state;
        }

        public void RestoreFormJToken(JToken state)
        {
            if (gameObject.tag != "Player") return;

            JObject s = state.ToObject<JObject>();
            IDictionary<string, JToken> stateDict = s;
            string str = stateDict["weaponPrefabName"].ToObject<string>();
            EquipItem(Resources.Load<Weapon>(str));
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
                yield return _weapon.weaponDamage;
            }
        }

        public IEnumerable<float> GetPercentageModifier(ProgressionEnum b)
        {
            yield return 1f;
        }
    }
}