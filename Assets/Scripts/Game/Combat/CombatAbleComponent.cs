using RPG.Control;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Combat
{
    [RequireComponent(typeof(HealthComponent))]
    public class CombatAbleComponent : MonoBehaviour, IRayCastAble
    {
        private Vector3 des;

        public bool HandleRaycaset(PlayerController p, RaycastHit h)
        {
            if (Vector3.Distance(h.point, p.transform.position) >
                p.GetComponent<FighterActionComponent>().weaponConfig.weaponRange)
            {
                p.SetCursor(CursorType.Deny);
                return false;
            }

            p.SetCursor(CursorType.Combat);
            if (Input.GetMouseButtonDown(0))
            {
                CombatAbleComponent cac = this.transform.GetComponent<CombatAbleComponent>();
                if (cac == null) return false;
                if (p.GetComponent<FighterActionComponent>().TryMakeTargetBeAttackTarget(cac))
                {
                    return true;
                }
            }

            return true;
        }
    }
}