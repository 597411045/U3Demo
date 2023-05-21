using UnityEngine;

namespace RPG.Core
{
    public class ActionSchedulerComponent : MonoBehaviour
    {
        private IAction currAction;

        public void StartAction(IAction action)
        {
            if (currAction == action) return;
            if (currAction != null)
            {
                currAction.Cancel();
                Debug.Log("Cancel:"+currAction);
            }

            currAction = action;
        }

        public void CancelCurrentAction()
        {
            StartAction(null);
        }
    }
}