using System;
using UnityEngine;

namespace RPG.UI
{
    public class CharacterUIComponent : MonoBehaviour
    {
        public float timer = 0f;
        public bool persist;
        public GameObject cineMachine;

        private void Awake()
        {
            cineMachine = GameObject.Find("CM vcam1");
        }

        private void LateUpdate()
        {
            if (cineMachine == null) return;
            this.transform.rotation = cineMachine.transform.rotation;

            if (persist) return;

            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }
}