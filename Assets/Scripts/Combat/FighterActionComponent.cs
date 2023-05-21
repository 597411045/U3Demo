using System;
using System.Collections;
using RPG.Core;
using RPG.Movement;
using UnityEngine;
using UnityEngine.Networking;

namespace RPG.Combat
{
    public class FighterActionComponent : MonoBehaviour, IAction
    {
        [SerializeField] private float weaponRange = 2f;
        [SerializeField] private float attackInterval = 2f;
        [SerializeField] private float weaponDamage = 10f;

        private Transform target;
        private float timeSinceLastAttackAction = 0f;

        private void Awake()
        {
            UpdateManager.UpdateActions.Add(UpdateMethod);
        }

        private void UpdateMethod()
        {
            timeSinceLastAttackAction += Time.deltaTime;


            if (target == null) return;
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
            if (timeSinceLastAttackAction > attackInterval)
            {
                this.GetComponent<Animator>().SetTrigger("IfAttack");
                timeSinceLastAttackAction = 0;
            }
        }

        private bool GetIfInRange()
        {
            return Vector3.Distance(transform.position, target.position) < weaponRange;
        }

        public void MakeTargetBeAttackTarget(CombatAbleComponent cac)
        {
            this.GetComponent<ActionSchedulerComponent>().StartAction(this);
            target = cac.transform;
        }

        public void Cancel()
        {
            target = null;
        }

        private void Hit()
        {
            target.GetComponent<HealthComponent>().TakeDamage(weaponDamage);
        }
    }
}