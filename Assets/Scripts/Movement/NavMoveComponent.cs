using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class NavMoveComponent : MonoBehaviour,IAction
    {




        private void Awake()
        {
            UpdateManager.UpdateActions.Add(UpdateMethod);
        }

        private void UpdateMethod()
        {

            UpdateAnimator();
        }

        public void StartMoveToPosition(Vector3 destination)
        {
            this.GetComponent<ActionSchedulerComponent>().StartAction(this);
            MoveToPosition(destination);
        }
        
        public void MoveToPosition(Vector3 destination)
        {
            this.GetComponent<NavMeshAgent>().destination = destination;
            this.GetComponent<NavMeshAgent>().isStopped = false;

        }

        public void Cancel()
        {
            this.GetComponent<NavMeshAgent>().isStopped = true;
        }
        
        private void UpdateAnimator()
        {
            Vector3 velocity = this.GetComponent<NavMeshAgent>().velocity;
            Vector3 localVelocity = this.transform.InverseTransformDirection(velocity);
            this.GetComponent<Animator>().SetFloat("ForwardSpeed", localVelocity.z);
        }
    }
}

