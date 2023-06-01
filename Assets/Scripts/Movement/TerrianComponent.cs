using System;
using RPG.Control;
using RPG.Core;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class TerrianComponent : MonoBehaviour, IRayCastAble
    {
        NavMeshHit nmh;
        NavMeshPath nmp;
        private float SumPathLength;

        private void Awake()
        {
            nmp = new NavMeshPath();
        }

        public bool HandleRaycaset(PlayerController p, RaycastHit h)
        {
            if (NavMesh.SamplePosition(h.point, out nmh, 1, NavMesh.AllAreas))
            {
                if (NavMesh.CalculatePath(p.transform.position, h.point, NavMesh.AllAreas, nmp) == false
                    || nmp.status != NavMeshPathStatus.PathComplete)
                {
                    p.SetCursor(CursorType.Deny);
                    return false;
                }
                SumPathLength = 0;

                for (int i = 0; i < nmp.corners.Length - 1; i++)
                {
                    SumPathLength += (nmp.corners[i] - nmp.corners[i + 1]).sqrMagnitude;
                }

                if (SumPathLength > 50)
                {
                    p.SetCursor(CursorType.Deny);
                    return false;
                }

                p.SetCursor(CursorType.Movement);
                if (Input.GetMouseButtonDown(0))
                    p.GetComponent<NavMoveComponent>().StartMoveToPosition(nmh.position);
                return true;
            }
            else
            {
                p.SetCursor(CursorType.Deny);
                return false;
            }
        }
    }
}