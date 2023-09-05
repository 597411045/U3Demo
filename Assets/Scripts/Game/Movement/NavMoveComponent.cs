using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPG.Control;
using RPG.Core;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class NavMoveComponent : MonoBehaviour
    {
        //SyncObject
        public Queue<string> syncBuffer;
        private GameObject cineMachine;
        private Vector3 movingPredict;
        private Vector3 desPredict;

        private void Awake()
        {
            cineMachine = GameObject.Find("CM vcam1");
            nmp = new NavMeshPath();
            syncBuffer = new Queue<string>();
        }

        private void Start()
        {
        }

        public void LocalCompute()
        {
            UpdateAnimator();
        }

        public void ActMoveUpdating()
        {
            //WSAD
            if (Input.GetKey(KeyCode.W))
            {
                if (movingPredict.z < 2)
                    movingPredict.z += 0.5f;
            }
            else
            {
                if (movingPredict.z > 0)
                {
                    movingPredict.z -= 0.5f;
                }
            }

            if (Input.GetKey(KeyCode.S))
            {
                if (movingPredict.z > -2)
                    movingPredict.z -= 0.5f;
            }
            else
            {
                if (movingPredict.z < 0)
                {
                    movingPredict.z += 0.5f;
                }
            }

            if (Input.GetKey(KeyCode.A))
            {
                if (movingPredict.x > -2)

                    movingPredict.x -= 0.5f;
            }
            else
            {
                if (movingPredict.x < 0)
                {
                    movingPredict.x += 0.5f;
                }
            }

            if (Input.GetKey(KeyCode.D))
            {
                if (movingPredict.x < 2)

                    movingPredict.x += 0.5f;
            }
            else
            {
                if (movingPredict.x > 0)
                {
                    movingPredict.x -= 0.5f;
                }
            }

            Vector3 directZ = (this.transform.position - new Vector3(cineMachine.transform.position.x,
                this.transform.position.y, cineMachine.transform.position.z)).normalized;
            Quaternion q = Quaternion.LookRotation(directZ, Vector3.up);
            desPredict = (q * movingPredict) + this.transform.position;

            //if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) ||
            //    Input.GetKey(KeyCode.D))
            {
                this.GetComponent<NavMeshAgent>().SetDestination(desPredict);
            }
        }

        #region 普通函数

        public void StartMoveToPosition(Vector3 destination, float speed = 0)
        {
            if (speed != 0)
            {
                this.GetComponent<NavMeshAgent>().speed = speed;
            }

            MoveToPosition(destination);
        }

        public void MoveToPosition(Vector3 destination)
        {
            if (!this.GetComponent<NavMeshAgent>().enabled) return;
            this.GetComponent<NavMeshAgent>().destination = destination;
            this.GetComponent<NavMeshAgent>().isStopped = false;
        }

        public void Cancel()
        {
            if (this.GetComponent<NavMeshAgent>().enabled)
                this.GetComponent<NavMeshAgent>().isStopped = true;
        }

        private void UpdateAnimator()
        {
            // if (this.GetComponent<SyncObjectComponent>().ControllerSIID == "")
            // {
            //     Vector3 velocity = this.GetComponent<NavMeshAgent>().velocity;
            //     Vector3 localVelocity = this.transform.InverseTransformDirection(velocity);
            //     this.GetComponent<Animator>().SetFloat("ForwardSpeed", localVelocity.z);
            //     ptt.Speed = localVelocity.z;
            // }
        }

        #endregion


        #region 射线系统

        NavMeshHit nmh;
        NavMeshPath nmp;
        private float SumPathLength;

        public bool IfMoveable(PlayerController p, RaycastHit h, out Vector3 des)
        {
            des = Vector3.zero;
            if (!NavMesh.SamplePosition(h.point, out nmh, 1, NavMesh.AllAreas))
            {
                p.SetCursor(CursorType.Deny);
                return false;
            }

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

            des = nmh.position;
            return true;
        }

        #endregion

        

        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(desPredict, 1f);
            Gizmos.DrawLine(this.transform.position,
                desPredict);
        }
    }
}