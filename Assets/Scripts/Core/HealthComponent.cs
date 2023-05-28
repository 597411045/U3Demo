using Newtonsoft.Json.Linq;
using RPG.Saving;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace RPG.Core
{
    public class HealthComponent : MonoBehaviour, IJsonSaveable
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

            CheckIfDead();
        }

        void CheckIfDead()
        {
            if (hp <= 0)
            {
                if (isDead) return;
                isDead = true;

                UpdateManager.RemoveActionsById(this.gameObject.GetHashCode());

                this.GetComponent<Animator>().SetTrigger("IfDead");
                this.GetComponent<NavMeshAgent>().enabled = false;

            }
        }


        public JToken CaptureAsJTokenInInterface()
        {
            return JToken.FromObject(hp);
        }

        public void RestoreFormJToken(JToken state)
        {
            hp = state.ToObject<float>();
            CheckIfDead();
        }
    }
}