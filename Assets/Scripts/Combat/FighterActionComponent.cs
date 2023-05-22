using System;
using System.Collections;
using RPG.Core;
using RPG.Movement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

namespace RPG.Combat
{
    public class FighterActionComponent : MonoBehaviour, IAction
    {
        [SerializeField] private float weaponRange = 2f;
        [SerializeField] private float attackInterval = 2f;
        [SerializeField] private float weaponDamage = 10f;

        private Transform target;
        private float TimeLeftToAttackAction = 0f;

        private void Awake()
        {
            UpdateManager.UpdateActions.Add(UpdateMethod);
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
                TimeLeftToAttackAction = attackInterval;
            }
        }

        private bool GetIfInRange()
        {
            return Vector3.Distance(transform.position, target.position) < weaponRange;
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
            target.GetComponent<HealthComponent>().TakeDamage(weaponDamage);
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color=Color.yellow;
            Gizmos.DrawWireSphere(this.transform.position,weaponRange);
        }
    }
}