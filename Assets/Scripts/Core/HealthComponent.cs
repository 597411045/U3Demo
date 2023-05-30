using System;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json.Linq;
using RPG.Saving;
using RPG.Stats;
using UI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace RPG.Core
{
    public class HealthComponent : MonoBehaviour, IJsonSaveable
    {
        [SerializeField] private float hp = 0;
        private float maxHp;

        private bool isDead = false;


        public float HP
        {
            set
            {
                hp = value;
                this.GetComponent<UIControl>().UpdateUI();
            }
        }

        public bool IsDead
        {
            get { return isDead; }
        }

        private void Start()
        {
            maxHp = this.GetComponent<BaseStats>().GetHealth();
            HP = maxHp;
        }

        public void TakeDamage(float damage, GameObject attacker)
        {
            HP = Mathf.Max(hp - damage, 0);

            if (CheckIfDead())
            {
                attacker.GetComponent<BaseStats>().GainExp(
                    this.GetComponent<BaseStats>().GetExpValue()
                    );
            }
        }

        bool CheckIfDead()
        {
            if (hp <= 0)
            {
                if (isDead) return true;
                isDead = true;

                UpdateManager.RemoveActionsById(this.gameObject.GetHashCode());

                this.GetComponent<Animator>().SetTrigger("IfDead");
                this.GetComponent<NavMeshAgent>().enabled = false;
                this.GetComponent<CapsuleCollider>().height = 0;
                this.GetComponent<CapsuleCollider>().radius = 0.3f;
                this.GetComponent<CapsuleCollider>().center = new Vector3(0, 0.3f, 0);

                return true;
            }

            return false;
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

        public float GetHealthPercentage()
        {
            return hp / maxHp * 100;
        }
    }
}