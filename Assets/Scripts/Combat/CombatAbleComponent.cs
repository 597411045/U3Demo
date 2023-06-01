using RPG.Control;
using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    [RequireComponent(typeof(HealthComponent))]
    public class CombatAbleComponent : MonoBehaviour, IRayCastAble
    {
        public bool HandleRaycaset(PlayerController p,RaycastHit h)
        {
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