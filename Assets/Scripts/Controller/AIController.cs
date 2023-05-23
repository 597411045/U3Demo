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
        private Vector3 lastPlayerPosition;
        private float suspicionTimeAfterChase = 2;
        private float suspicionTimeAfterPatrol = 2;
        private Vector3 guardPosition;
        private HealthComponent hc;

        private CSMachine _SMachine;

        private void Start()
        {
            player = GameObject.FindWithTag("Player");
            guardPosition = this.transform.position;
            lastPlayerPosition = Vector3.zero;
            hc = this.GetComponent<HealthComponent>();

            _SMachine = new CSMachine();
            _SMachine.AddState(new State_Idle()).OnEnter();
            ((CTransition)_SMachine.GetState(State_Idle.CName).AddTransition(new Trans_ToMove())).
            Delegate_OnCheck += Trans_IfPlayerInRange;


            ((CState)_SMachine.AddState(new State_Move()))
            ._stateAction += State_MoveTo;
            CTransition t2 = (CTransition)_SMachine.GetState(State_Move.CName).AddTransition(new Trans_ToIdle());
            t2.Delegate_OnCheck += Trans_IfPlayerInRange;
            t2.ifReverse = true;
            
            
            
            
        }

        bool Test()
        {
            return true;
        }

        private void Awake()
        {
            UpdateManager.UpdateActions.Add(UpdateMethod);
        }

        void UpdateMethod()
        {
            if (this.enabled == false) return;
            _SMachine.OnUpdate();

            if (hc.IsDead) return;
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
                lastPlayerPosition = player.transform.position;
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
            if (lastPlayerPosition != Vector3.zero &&
                Vector3.Distance(this.transform.position, lastPlayerPosition) > 0.5f)
            {
                this.GetComponent<NavMoveComponent>().StartMoveToPosition(lastPlayerPosition, chaseSpeed);
                return true;
            }

            lastPlayerPosition = Vector3.zero;
            return false;
        }

        private bool Trans_IfPlayerInRange()
        {
            if (Vector3.Distance(this.transform.position, player.transform.position) <chaseDistance)
            {
                lastPlayerPosition = player.transform.position;
                return true;
            }

            return false;
        }

        private void State_MoveTo()
        {
            this.GetComponent<NavMoveComponent>().StartMoveToPosition(lastPlayerPosition, chaseSpeed);
        }

        private bool TryPatrol()
        {
            this.GetComponent<NavMoveComponent>().StartMoveToPosition(guardPosition);
            return true;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(this.transform.position, chaseDistance);
            Gizmos.DrawSphere(lastPlayerPosition, 0.25f);
        }
    }
}