using System;
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
        private GameObject cineMachine;
        private Vector3 movingPredict;
        private Vector3 desPredict;
        NavMeshHit nmh;
        NavMeshPath nmp;
        private float SumPathLength;

        private void Awake()
        {
            cineMachine = GameObject.Find("CM FreeLook1");
            nmp = new NavMeshPath();
        }

        public void Update()
        {
            UpdateAnimator();
            UpdateMove();
        }

        public void UpdateMove()
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

            if (!NavMesh.SamplePosition(desPredict, out nmh, 1, NavMesh.AllAreas)) return;
            if (NavMesh.CalculatePath(this.transform.position, desPredict, NavMesh.AllAreas, nmp) == false
                || nmp.status != NavMeshPathStatus.PathComplete) return;

            SumPathLength = 0;

            if (nmp.corners.Length > 2) return;

            for (int i = 0; i < nmp.corners.Length - 1; i++)
            {
                SumPathLength += (nmp.corners[i] - nmp.corners[i + 1]).sqrMagnitude;
            }

            if (SumPathLength > 50) return;

            desPredict = nmh.position;


            this.GetComponent<NavMeshAgent>().SetDestination(desPredict);
        }


        private void UpdateAnimator()
        {
            Vector3 velocity = this.GetComponent<NavMeshAgent>().velocity;
            Vector3 localVelocity = this.transform.InverseTransformDirection(velocity);
            this.GetComponent<Animator>().SetFloat("ForwardSpeed", localVelocity.z);
        }

        public void OnDrawGizmos()
        {
            Gizmos.DrawSphere(desPredict, 0.5f);
            Gizmos.DrawLine(this.transform.position,
                desPredict);
        }
    }
}