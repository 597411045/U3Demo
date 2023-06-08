using System;
using UnityEngine;

namespace RPG.UI
{
    public class CharacterUIComponent : MonoBehaviour
    {
        public float timer = 0f;
        public bool persist;

        private void LateUpdate()
        {
            if (Camera.main == null) return;
            this.transform.forward = Camera.main.transform.forward;

            if (persist) return;

            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }
}