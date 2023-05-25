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

        private SMEnemy SMachine;

        private void Start()
        {
            player = GameObject.FindWithTag("Player");
            targerPosition = Vector3.zero;
            hc = this.GetComponent<HealthComponent>();

            SMachine = new SMEnemy();
            BuildFSMFunction();
        }

        #region FSM

        private void BuildFSMFunction()
        {
            SMachine.SMove._stateAction = State_Move;
            
            SMachine.IfInChaseRange.Delegate_OnCheck += IfInChaseRange;
            SMachine.IfOutChaseRange.Delegate_OnCheck += IfOutChaseRange;
            SMachine.IfNeedWait.Delegate_OnCheck += IfNeedWait;
            SMachine.IfReachDestination.Delegate_OnCheck += IfReachDestination;
            SMachine.IfNeedGoToSomewhere.Delegate_OnCheck += IfNeedGoToSomewhere;
        }

        private void State_Move()
        {
            this.GetComponent<NavMoveComponent>().StartMoveToPosition(SMachine.moveDestination);
        }
        
        private bool IfInChaseRange()
        {
            if (Vector3.Distance(player.transform.position, this.transform.position) < chaseDistance)
            {
                SMachine.moveDestination = player.transform.position;
                this.GetComponent<NavMeshAgent>().speed = 5;
                return true;
            }
            else
            {
                return false;
            }
        }
        
        private bool IfOutChaseRange()
        {
            if (Vector3.Distance(player.transform.position, this.transform.position) > chaseDistance)
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
            if (SMachine.waitTimer<=0)
            {
                return true;
            }
            else
            {
                SMachine.waitTimer -= Time.deltaTime;
                return false;
            }
        }
        private bool IfNeedGoToSomewhere()
        {
            if (Vector3.Distance(this.transform.position,SMachine.moveDestination)>1)
            {
                SMachine.moveDestination = this.GetComponent<PathPatrolComponent>().GetPoint();
                this.GetComponent<NavMeshAgent>().speed = 2;
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool IfReachDestination()
        {
            if (Vector3.Distance(this.transform.position,SMachine.moveDestination)<1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        

        #endregion


        #region Old

        private void Awake()
        {
            UpdateManager.UpdateActions.Add(UpdateMethod);
        }

        void UpdateMethod()
        {
            if (hc.IsDead) return;
            //SMachine.OnUpdate();
            //return;
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
            Gizmos.DrawSphere(targerPosition, 0.25f);
        }

        #endregion
    }
}