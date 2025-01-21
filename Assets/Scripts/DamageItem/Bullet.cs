using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Bullet : MonoBehaviour
{
    public float speed = 5;
    public float aliveDuration = 5;

    public bool ReflectOneceOnWallAble;
    public bool ReflectOnEnemyAble;
    public int ReflectOnEnemyAimit = 2;
    public bool PenetrateAble;
    public bool DrainBloodAble;

    private Rigidbody rigidBody;

    public AIController caster;
    [NonSerialized] public List<AIController> ignoreList = new List<AIController>();

    private void Start()
    {
        rigidBody = this.gameObject.gameObject.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        rigidBody.velocity = this.transform.forward * speed;

        if (aliveDuration > 0)
        {
            aliveDuration -= Time.fixedDeltaTime;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (caster.gameObject != other.gameObject)
        {
            if (other.gameObject.tag == "Wall")
            {
                var data = other.gameObject.GetComponent<FunctionItem>();
                if (data != null)
                {
                    data.ItemInjured(caster);
                }
            }

            if (ReflectOneceOnWallAble && other.gameObject.tag == "Wall")
            {
                if (Physics.Raycast(this.transform.position, this.transform.forward, out RaycastHit hit, 100))
                {
                    this.transform.forward = Vector3.Reflect(this.transform.forward, hit.normal);
                    ReflectOneceOnWallAble = false;
                }

                return;
            }

            if (ReflectOneceOnWallAble == false && other.gameObject.tag == "Wall")
            {
                Destroy(this.gameObject);
                return;
            }

            var otherAic = other.gameObject.GetComponent<AIController>();
            if (otherAic != null && caster.tag != otherAic.tag && ignoreList.Contains(otherAic) == false)
            {
                if (otherAic.AIInjured(caster) && DrainBloodAble)
                {
                    caster.characterData.Cure(caster, 10);
                }

                if (ReflectOnEnemyAble)
                {
                    ignoreList.Add(otherAic);
                    SetDirToNearestTarget(otherAic);
                    ReflectOnEnemyAimit--;
                    if (ReflectOnEnemyAimit <= 0)
                    {
                        ReflectOnEnemyAble = false;
                    }
                }

                if (PenetrateAble == false && ReflectOnEnemyAble == false)
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }

    public void SetDirToNearestTarget(AIController lastTarget)
    {
        var target = EntityManager.Instance.GetCallerNearestAlly(lastTarget);
        if (target != null)
        {
            this.transform.forward = target.transform.position - this.transform.position;
        }
    }
}