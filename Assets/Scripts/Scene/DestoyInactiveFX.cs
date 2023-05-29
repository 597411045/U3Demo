using System;
using UnityEngine;

namespace RPG.Scene
{
    public class DestoyInactiveFX : MonoBehaviour
    {
        private void Update()
        {
            if (this.GetComponent<ParticleSystem>().IsAlive() == false)
            {
                Destroy(this.gameObject);
            }
        }
    }
}