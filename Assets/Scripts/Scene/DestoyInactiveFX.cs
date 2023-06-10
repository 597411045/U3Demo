using System;
using UnityEngine;

namespace RPG.Scene
{
    public class DestoyInactiveFX : MonoBehaviour
    {
        private void LateUpdate()
        {
            if (this.GetComponent<ParticleSystem>().IsAlive() == false)
            {
                Destroy(this.gameObject);
            }
        }
    }
}