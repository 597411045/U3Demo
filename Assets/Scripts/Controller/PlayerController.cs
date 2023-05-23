using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        private HealthComponent hc;

        private void Start()
        {
            hc = this.GetComponent<HealthComponent>();
        }

        private void Awake()
        {
            UpdateManager.UpdateActions.Add(UpdateMethod);
        }

        void UpdateMethod()
        {
            if (this.enabled == false) return;
            if (hc.IsDead) return;
            if (CanDoCombat()) return;
            if (CanSetNavDestinationToCursor()) return;
        }

        private bool CanDoCombat()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit[] raycastHits = Physics.RaycastAll(GerRayFromCursor());
                foreach (RaycastHit hit in raycastHits)
                {
                    CombatAbleComponent cac = hit.transform.GetComponent<CombatAbleComponent>();
                    if (cac == null) continue;
                    if (this.GetComponent<FighterActionComponent>().TryMakeTargetBeAttackTarget(cac))
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        private bool CanSetNavDestinationToCursor()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit raycastHit;
                bool ifHit = Physics.Raycast(GerRayFromCursor(), out raycastHit);
                if (ifHit)
                {
                    this.GetComponent<NavMoveComponent>().StartMoveToPosition(raycastHit.point);
                    return true;
                }
            }

            return false;
        }

        private Ray GerRayFromCursor()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}