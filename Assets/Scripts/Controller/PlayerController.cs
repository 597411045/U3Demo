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


        enum CursorType
        {
            None,
            Movement,
            Combat
        }

        [Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] private CursorMapping[] _cursorMappings;

        private void Start()
        {
            hc = this.GetComponent<HealthComponent>();
        }

        private void Awake()
        {
            UpdateManager.RegisterAction(UpdateMethod, this.gameObject.GetHashCode());
        }

        void UpdateMethod()
        {
            if (this.enabled == false) return;

            if (CanDoCombat()) return;
            if (CanSetNavDestinationToCursor()) return;
            //SetCursor(CursorType.None);
        }

        private bool CanDoCombat()
        {
            if (Input.GetMouseButtonDown(0))
            {
                SetCursor(CursorType.Combat);
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

        private void SetCursor(CursorType e)
        {
            CursorMapping m = GetCursorMapping(e);
            Cursor.SetCursor(m.texture, m.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach (var c in _cursorMappings)
            {
                if (c.type == type)
                {
                    return c;
                }
            }

            return _cursorMappings[0];
        }


        private bool CanSetNavDestinationToCursor()
        {
            if (Input.GetMouseButtonDown(0))
            {
                SetCursor(CursorType.Movement);

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