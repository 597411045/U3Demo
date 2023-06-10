using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Scene
{
    public class AutoDestroy : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (this.GetComponent<ParticleSystem>().IsAlive() == false)
            {
                Destroy(this.gameObject);
            }
        }
    }
}