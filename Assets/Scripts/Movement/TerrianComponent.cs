using System;
using RPG.Control;
using RPG.Core;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class TerrianComponent : MonoBehaviour, IRayCastAble
    {
        private Vector3 des;

        public bool HandleRaycaset(PlayerController p, RaycastHit h)
        {
            if (!p.GetComponent<NavMoveComponent>().IfMoveable(p, h, out des)) return false;

            p.SetCursor(CursorType.Movement);
            if (Input.GetMouseButtonDown(0))
            {
                p.GetComponent<NavMoveComponent>().StartMoveToPosition(des);
                return true;
            }

            return false;
        }
    }
}