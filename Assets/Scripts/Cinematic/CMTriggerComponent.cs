using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Cinematic
{
    public class CMTriggerComponent : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag.Equals("Player"))
            {
                this.GetComponent<PlayableDirector>().Play();
                this.GetComponent<BoxCollider>().enabled = false;
            }
        }
    }
}