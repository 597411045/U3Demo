using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RPG.Core;
using RPG.Saving;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class NavMoveComponent : MonoBehaviour, IAction, IJsonSaveable
    {
        private void Awake()
        {
            UpdateManager.RegisterAction(UpdateMethod, this.gameObject.GetHashCode());
        }

        private void UpdateMethod()
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
            Vector3 velocity = this.GetComponent<NavMeshAgent>().velocity;
            Vector3 localVelocity = this.transform.InverseTransformDirection(velocity);
            this.GetComponent<Animator>().SetFloat("ForwardSpeed", localVelocity.z);
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
    }
}