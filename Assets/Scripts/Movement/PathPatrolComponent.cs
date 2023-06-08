using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Movement
{
    public class PathPatrolComponent : MonoBehaviour
    {
        [SerializeField] public GameObject pathGroup;
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

        private void OnDrawGizmosSelected()
        {
            if (pathPoints == null) return;
            for (int i = 0; i < pathPoints.Count; i++)
            {
                Gizmos.DrawSphere(pathPoints[i], 0.25f);
                Gizmos.DrawLine(pathPoints[i],
                    pathPoints[(i + 1) % pathPoints.Count]);
            }
        }

        private void OnDrawGizmos()
        {
            if (pathGroup != null) return;
            if (pathPoints != null) return;

            for (int i = 0; i < this.transform.childCount; i++)
            {
                Gizmos.DrawSphere(this.transform.GetChild(i).position, 0.25f);
                Gizmos.DrawLine(this.transform.GetChild(i).position,
                    this.transform.GetChild((i + 1) % this.transform.childCount).position);
            }
        }

        public bool TryFollowTheNextPath()
        {

            if (Vector3.Distance(this.transform.position, GetPoint()) > 1)
            {
                this.GetComponent<NavMoveComponent>().StartMoveToPosition(pathPoints[currPoint], 3);
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

        public Vector3 GetPoint()
        {
            if (pathPoints.Count == 0)
            {
                pathPoints.Add(this.transform.position);
            }
            return pathPoints[currPoint];
        }
    }
}