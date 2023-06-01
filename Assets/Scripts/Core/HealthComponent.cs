using System;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json.Linq;
using RPG.Saving;
using RPG.Stats;
using RPG.UI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace RPG.Core
{
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] private UnityEvent<float> uevent;


        private bool isDead = false;

        public bool IsDead
        {
            get { return isDead; }
        }

        public void TakeDamage(float damage, GameObject attacker)
        {
            this.GetComponent<BaseStats>().HP = Mathf.Max(0, this.GetComponent<BaseStats>().HP - damage);

            uevent.Invoke(damage);
            if (CheckIfDead())
            {
                attacker.GetComponent<BaseStats>().GainExp(
                    this.GetComponent<BaseStats>().GetExpReward()
                );
            }
        }

        public bool CheckIfDead()
        {
            if (this.GetComponent<BaseStats>().HP <= 0)
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
    }
}