using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace RPG.Core
{
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] private float hp = 100;

        private bool isDead = false;

        public bool IsDead
        {
            get { return isDead; }
        }

        public void TakeDamage(float damage)
        {
            hp = Mathf.Max(hp - damage, 0);
            if (hp == 0)
            {
                DeadAction();
            }
        }

        void DeadAction()
        {
            if (isDead) return;
            isDead = true;
            this.GetComponent<Animator>().SetTrigger("IfDead");
            this.GetComponent<ActionSchedulerComponent>().CancelCurrentAction();
            this.GetComponent<NavMeshAgent>().enabled = false;
        }
    }
}