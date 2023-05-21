using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Movement
{
    public class PathPatrolComponent : MonoBehaviour
    {
        [SerializeField] private GameObject pathGroup;
        private List<Vector3> pathPoints;
        private int currPoint;

        private void Start()
        {
            currPoint = 0;
            pathPoints = new List<Vector3>();
            if (pathGroup == null) return;
            for (int i = 0; i < pathGroup.transform.childCount; i++)
            {
                pathPoints.Add(pathGroup.transform.GetChild(i).position);
            }
        }

        private void OnDrawGizmos()
        {
            if (pathPoints == null) return;
            for (int i = 0; i < pathPoints.Count; i++)
            {
                Gizmos.DrawSphere(pathPoints[i], 0.25f);
                Gizmos.DrawSphere(pathPoints[i], 0.25f);
                Gizmos.DrawLine(pathPoints[i],
                    pathPoints[(i + 1) % pathPoints.Count]);
            }
        }

        public bool TryFollowTheNextPath()
        {
            if (Vector3.Distance(this.transform.position, pathPoints[currPoint]) > 1)
            {
                this.GetComponent<NavMoveComponent>().StartMoveToPosition(pathPoints[currPoint]);
                return true;
            }
            return false;
        }

        public void PathPointNext()
        {
            currPoint = (currPoint + 1) % pathPoints.Count;
        }

        public void AddPoint(Vector3 v)
        {
            pathPoints.Add(v);
        }
    }
}