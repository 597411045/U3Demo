using UnityEngine;

namespace RPG.Core
{
    public class TaskPipelineBase : MonoBehaviour
    {
        private bool IfDestroyed;

        protected virtual void Start()
        {
            Register();
        }

        protected virtual void Register()
        {
            TaskPipelineManager.Ins.Register(this);
        }

        protected virtual void OnDestroy()
        {
            IfDestroyed = true;
        }

        public bool GetIfDestroyed()
        {
            return IfDestroyed;
        }
    }
}