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
        [SerializeField][Range(0, 6)] private float chaseSpeed = 5;

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

            // Trans_IdleToMove titm = new Trans_IdleToMove();
            //
            // Trans_MoveToIdle tmti = new Trans_MoveToIdle();
            //
            // State_Idle si = new State_Idle();
            // si.AddTransition(titm);
            //
            // State_Move sm = new State_Move();
            // sm.AddTransition(tmti);
            //
            // _SMachine = new CSMachine(si);
            // _SMachine.AddState(sm);
            
        }

        private void Awake()
        {
            UpdateManager.UpdateActions.Add(UpdateMethod);
        }

        void UpdateMethod()
        {
            if (this.enabled == false) return;

            if (hc.IsDead) return;
            //_SMachine.OnUpdate(Time.deltaTime);
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
                if (this.GetComponent<FighterActionComponent>().TryMakeTargetBeAttackTarget(cac,chaseSpeed))
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