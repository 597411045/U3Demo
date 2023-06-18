using System;
using System.Collections.Generic;
using RPG.Control;
using RPG.Core;
using UnityEngine;
using UnityEngine.Playables;

namespace Cinematic
{
    public class CMTriggerComponent : MonoBehaviour
    {
        private GameObject player;

        private void Start()
        {
            player = GameObject.FindWithTag("Player");
            
            this.GetComponent<PlayableDirector>().played += DisablePlayerControl;
            this.GetComponent<PlayableDirector>().stopped += EnablePlayerControl;
        }

        

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag.Equals("Player"))
            {
                this.GetComponent<PlayableDirector>().Play();
                this.GetComponent<BoxCollider>().enabled = false;
            }
        }

        private void DisablePlayerControl(PlayableDirector p)
        {
            player.GetComponent<ActionSchedulerComponent>().CancelCurrentAction();
            player.GetComponent<PlayerController>().enabled = false;
        }

        private void EnablePlayerControl(PlayableDirector p)
        {
            player.GetComponent<PlayerController>().enabled = true;
        }
    }
}