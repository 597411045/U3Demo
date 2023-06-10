using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PRG.Network;
using RPG.Control;
using RPG.Core;
using RPG.Saving;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class NavMoveComponent : MonoBehaviour, IAction, IJsonSaveable
    {
        private void Start()
        {
            UpdateManager.Ins.RegisterAction(CActionType.LocalCompute,
                new CAction(LocalCompute, this.GetInstanceID(), this.gameObject));
            nmp = new NavMeshPath();
        }

        private void LocalCompute()
        {
            UpdateAnimator();
        }

        public void StartMoveToPosition(Vector3 destination, float speed = 0)
        {
            if (speed != 0)
            {
                this.GetComponent<NavMeshAgent>().speed = speed;
            }

            this.GetComponent<ActionSchedulerComponent>().StartAction(this);
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
            if (!NetworkCenter.isServer)
            {
                Vector3 velocity = this.GetComponent<NavMeshAgent>().velocity;
                Vector3 localVelocity = this.transform.InverseTransformDirection(velocity);
                this.GetComponent<Animator>().SetFloat("ForwardSpeed", localVelocity.z);
            }
        }

        struct MoveSaveData
        {
            public JToken position;
            public JToken rotation;

            public MoveSaveData(JToken a, JToken b)
            {
                position = a;
                rotation = b;
            }

            public JToken ToToken()
            {
                JObject state = new JObject();
                IDictionary<string, JToken> stateDict = state;
                stateDict["position"] = position;
                stateDict["rotation"] = rotation;
                return state;
            }
        }

        public JToken CaptureAsJTokenInInterface()
        {
            // JObject state = new JObject();
            // IDictionary<string, JToken> stateDict = state;
            // stateDict["position"] = transform.position.ToToken();
            // stateDict["rotation"] = transform.eulerAngles.ToToken();

            return new MoveSaveData(transform.position.ToToken(), transform.eulerAngles.ToToken()).ToToken();
        }

        public void RestoreFormJToken(JToken state)
        {
            JObject s = state.ToObject<JObject>();
            IDictionary<string, JToken> stateDict = s;

            this.GetComponent<NavMeshAgent>().Warp(stateDict["position"].ToVector3());
            this.transform.eulerAngles = stateDict["rotation"].ToVector3();
        }


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
    }
}