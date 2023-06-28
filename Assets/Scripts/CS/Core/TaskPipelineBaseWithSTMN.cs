using UnityEngine;

namespace RPG.Core
{
    public class TaskPipelineBaseWithSTMN<T> : SingleTonWithMono<T> where T : TaskPipelineBaseWithSTMN<T>
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