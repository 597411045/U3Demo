﻿using UnityEngine;

namespace RPG.Core
{
    public class TaskPipelineBase : MonoBehaviour
    {
        private bool IfDestroyed;

        protected void Start()
        {
            Register();
        }

        private void Register()
        {
            TaskPipelineManager.Ins.Register(this);
        }

        protected void OnDestroy()
        {
            IfDestroyed = true;
        }

        public bool GetIfDestroyed()
        {
            return IfDestroyed;
        }
    }
}