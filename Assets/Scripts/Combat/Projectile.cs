using System;
using RPG.Core;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 1;
        [SerializeField] public bool isAutoNav;
        public Vector3 direction = Vector3.zero;
        private float aliveTime = 10;
        public float atk;
        public GameObject Target;
        [SerializeField] public GameObject colliderEffect = null;
        [SerializeField] public GameObject[] clearImmediate = null;


        [SerializeField] private UnityEvent HitEvent;
        public GameObject launcher;

        private void Awake()
        {
            UpdateManager.LocalCompute.Add(new CAction(UpdateMethod, this.GetInstanceID(), this.gameObject));
        }

        private void UpdateMethod()
        {
            if (isAutoNav == false)
            {
                if (direction != Vector3.zero)
                {
                    this.transform.rotation = Quaternion.LookRotation(direction);
                    this.transform.position += this.transform.rotation * new Vector3(0, 0, speed) * Time.deltaTime;
                }
            }

            if (isAutoNav && Target != null)
            {
                if (direction != Vector3.zero)
                {
                    if (Target.GetComponent<HealthComponent>().IsDead == false)
                    {
                        this.transform.rotation =
                            Quaternion.LookRotation(
                                (Target.transform.position + new Vector3(0, 1, 0) - this.transform.position)
                                .normalized);
                    }

                    this.transform.position += this.transform.rotation * new Vector3(0, 0, speed) * Time.deltaTime;
                }
            }

            if (aliveTime <= 0)
            {
                UpdateManager.ClearAllByGameobjectId(this.gameObject.GetInstanceID());
                Destroy(this.gameObject);
            }

            aliveTime -= Time.deltaTime;
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<HealthComponent>() != null)
            {
                other.gameObject.GetComponent<HealthComponent>().TakeDamage(atk, launcher);
                HitEvent.Invoke();
                // other.gameObject.GetComponent<Rigidbody>().AddForce(
                //     (this.transform.position -
                //      (this.transform.position + this.transform.rotation * new Vector3(0, 0, -1))).normalized
                //     * 2, ForceMode.Force);
                if (colliderEffect != null)
                {
                    Instantiate(colliderEffect, this.gameObject.transform.position, this.transform.rotation);
                }

                foreach (var c in clearImmediate)
                {
                    Destroy(c);
                }

                UpdateManager.ClearAllByGameobjectId(this.gameObject.GetInstanceID());
                Destroy(this.gameObject, 0.3f);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            speed = 0;
        }
    }
}