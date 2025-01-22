using System;
using System.Collections;
using UnityEngine;

public class C1被动 : BaseEffect
{
    private AIController owner;
    private float Config_Interval = 3;
    private float Interval = 3;
    public int level = 1;


    public C1被动()
    {
        name = "C1被动";
    }

    public override void ActionCallByInitial(GameObject go)
    {
        owner = go.GetComponent<AIController>();
        if (owner != null)
        {
            GameMode.Instance.AddActionToUpdateNoOrderListBuffer(SkillAction);
            Interval = Config_Interval;
        }
    }

    private void SkillAction(float deltaTime)
    {
        if (PlayerController.Instance.currentChaAic == owner)
        {
            if (Interval > 0)
            {
                Interval -= deltaTime;
            }

            if (Interval <= 0)
            {
                Bullet bullet = EntityManager.Instance.SpawnBullet(
                    owner.transform.position +
                    owner.transform.forward / 2 + Vector3.up,
                    owner.gameObject.transform.rotation);
                bullet.caster = owner;
                bullet.SetDirToNearestTarget(owner);
                bullet.transform.position += bullet.transform.forward / 2;
                Interval = Config_Interval;
            }
        }
    }
}