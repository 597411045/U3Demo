using System;
using System.Collections;
using System.Collections.Generic;
using FSM;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] private float chaseDistance = 5f;
        [SerializeField] [Range(0, 6)] private float chaseSpeed = 5;

        private GameObject player;
        private Vector3 targerPosition;
        private float suspicionTimeAfterChase = 5;
        private float suspicionTimeAfterPatrol = 2;
        private HealthComponent hc;
        private FighterActionComponent fac;

        private SMEnemy SMachine;

        private void Start()
        {
            player = GameObject.FindWithTag("Player");
            targerPosition = Vector3.zero;
            hc = this.GetComponent<HealthComponent>();
            fac = this.GetComponent<FighterActionComponent>();

            SMachine = new SMEnemy();
            BuildFSMFunction();
            SMachine.SIdle.OnEnter();

            UpdateManager.Ins.RegisterAction(CActionType.LocalCompute,
                new CAction(LocalCompute, this.GetInstanceID(), this.gameObject));
        }

        #region FSM

        private void BuildFSMFunction()
        {
            SMachine.SMove._stateAction = State_Move;
            SMachine.SAttack._stateAction = State_Attack;
            SMachine.SIdle._stateAction = State_Idle;

            SMachine.IfInChaseRange.Delegate_OnCheck += IfInChaseRange;
            SMachine.IfOutChaseRange.Delegate_OnCheck += IfOutChaseRange;
            SMachine.IfNeedWait.Delegate_OnCheck += IfNeedWait;
            SMachine.IfReachDestination.Delegate_OnCheck += IfReachDestination;
            SMachine.IfNeedGoToSomewhere.Delegate_OnCheck += IfNeedGoToSomewhere;

            SMachine.IfInAttackRange.Delegate_OnCheck += IfInAttackRange;
            SMachine.IfOutAttackRange.Delegate_OnCheck += IfOutAttackRange;
        }

        private bool IfOutAttackRange()
        {
            if (player == null) return false;
            if (Vector3.Distance(this.transform.position, player.transform.position) > fac.weaponConfig.weaponRange ||
                player.GetComponent<HealthComponent>().IsDead)
            {
                this.GetComponent<Animator>().SetTrigger("StopAttack");
                this.GetComponent<NavMeshAgent>().enabled = true;
                fac.target = null;

                if (player.GetComponent<HealthComponent>().IsDead)
                {
                    SMachine.waitTimer = 5;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IfInAttackRange()
        {
            if (player == null) return false;
            if (Vector3.Distance(this.transform.position, player.transform.position) <= fac.weaponConfig.weaponRange &&
                player.GetComponent<HealthComponent>().IsDead == false)
            {
                this.GetComponent<NavMeshAgent>().enabled = false;
                SMachine.attackTarget = player.transform;
                fac.target = player.transform;
                return true;
            }
            else
            {
                return false;
            }
        }

        private void State_Idle()
        {
            if (fac.TimeLeftToAttackAction >= 0)
            {
                fac.TimeLeftToAttackAction -= Time.deltaTime;
            }
        }

        private void State_Move()
        {
            this.GetComponent<NavMoveComponent>().StartMoveToPosition(SMachine.moveDestination);

            if (fac.TimeLeftToAttackAction >= 0)
            {
                fac.TimeLeftToAttackAction -= Time.deltaTime;
            }
        }

        private void State_Attack()
        {
            if (fac.TimeLeftToAttackAction >= 0)
            {
                fac.TimeLeftToAttackAction -= Time.deltaTime;
            }

            this.transform.LookAt(SMachine.attackTarget.transform);

            if (fac.TimeLeftToAttackAction <= 0)
            {
                this.GetComponent<Animator>().ResetTrigger("StopAttack");
                this.GetComponent<Animator>().SetTrigger("IfAttack");
                fac.TimeLeftToAttackAction = fac.weaponConfig.attackInterval;
            }
        }

        private bool IfInChaseRange()
        {
            if (player == null) return false;
            if (Vector3.Distance(player.transform.position, this.transform.position) < chaseDistance &&
                player.GetComponent<HealthComponent>().IsDead == false)
            {
                SMachine.moveDestination = player.transform.position;
                this.GetComponent<NavMeshAgent>().speed = 2;
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IfOutChaseRange()
        {
            if (player == null) return false;

            if (Vector3.Distance(player.transform.position, this.transform.position) > chaseDistance ||
                player.GetComponent<HealthComponent>().IsDead)
            {
                SMachine.waitTimer = 5;
                return true;
            }
            else
            {
                SMachine.moveDestination = player.transform.position;
                return false;
            }
        }

        private bool IfNeedWait()
        {
            if (SMachine.waitTimer >= 0)
            {
                SMachine.waitTimer -= Time.deltaTime;
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IfNeedGoToSomewhere()
        {
            SMachine.moveDestination = this.GetComponent<PathPatrolComponent>().GetPoint();
            if (Vector3.Distance(this.transform.position, SMachine.moveDestination) > 1)
            {
                this.GetComponent<NavMeshAgent>().speed = 5;
                return true;
            }
            else
            {
                this.GetComponent<PathPatrolComponent>().PathPointNext();
                return false;
            }
        }

        private bool IfReachDestination()
        {
            if (Vector3.Distance(this.transform.position, SMachine.moveDestination) < 1)
            {
                SMachine.waitTimer = 2;
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion


        #region Old

        void LocalCompute()
        {
            SMachine.OnUpdate();

            return;
            if (TryDoCombat()) return;

            if (TryChase())
            {
                ResetTimer(ref suspicionTimeAfterChase);

                return;
            }
            else
            {
                if (TimerCheck(ref suspicionTimeAfterChase))
                {
                    return;
                }
                else
                {
                }
            }

            if (this.GetComponent<PathPatrolComponent>().TryFollowTheNextPath())
            {
                ResetTimer(ref suspicionTimeAfterPatrol);
                return;
            }
            else
            {
                if (TimerCheck(ref suspicionTimeAfterPatrol))
                {
                    return;
                }
                else
                {
                    this.GetComponent<PathPatrolComponent>().PathPointNext();
                }
            }
        }

        private void ResetTimer(ref float time)
        {
            time = 0;
        }

        private bool TimerCheck(ref float time)
        {
            if (time < 2)
            {
                time += Time.deltaTime;
                return true;
            }

            return false;
        }

        private bool TryDoCombat()
        {
            if (Vector3.Distance(this.transform.position, player.transform.position) < chaseDistance)
            {
                targerPosition = player.transform.position;
                CombatAbleComponent cac = player.GetComponent<CombatAbleComponent>();
                if (this.GetComponent<FighterActionComponent>().TryMakeTargetBeAttackTarget(cac, chaseSpeed))
                {
                    return true;
                }
            }

            return false;
        }

        private bool TryChase()
        {
            if (targerPosition != Vector3.zero &&
                Vector3.Distance(this.transform.position, targerPosition) > 0.5f)
            {
                this.GetComponent<NavMoveComponent>().StartMoveToPosition(targerPosition, chaseSpeed);
                return true;
            }

            targerPosition = Vector3.zero;
            return false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(this.transform.position, chaseDistance);
            if (SMachine?.moveDestination != null)
            {
                Gizmos.DrawSphere(SMachine.moveDestination, 0.25f);
            }
        }

        #endregion
    }
}