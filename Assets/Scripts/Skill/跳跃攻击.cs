using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class 跳跃攻击 : BaseSkill
{
    private float CoolDownTimer;
    private AIController owner;

    public 跳跃攻击()
    {
        name = "跳跃攻击";
        Config_EffectCoolDown = 3;
    }

    public override void ActionCallByInitial(GameObject go)
    {
        owner = go.GetComponent<AIController>();
        if (owner != null)
        {
            GameMode.Instance.AddActionToUpdateNoOrderListBuffer(CoolDown);
        }
    }

    public void CoolDown(float deltaTime)
    {
        if (CoolDownTimer > 0)
        {
            CoolDownTimer -= deltaTime;
        }
    }

    public override bool IsReady()
    {
        return CoolDownTimer <= 0;
    }

    private void EventAction()
    {
        var tmp = EntityManager.Instance.SpawnCylinderDamage(owner.transform.position);
        tmp.selfAic = owner;
        owner.splineAnimate.Completed -= EventAction;
    }

    public override void ActionCallByReleaseSkill(GameObject go)
    {
        if (owner != null)
        {
            owner.splineAnimate.Completed += EventAction;

            var target = EntityManager.Instance.GetCallerNearestEnemy(owner);
            var direction = (target.gameObject.transform.position - owner.gameObject.transform.position);
            var directionNormal = direction.normalized;

            owner.splineAnimate.Container = EntityManager.Instance.SpawnSpline();
            var sp = owner.splineAnimate.Container;
            int i = 0;
            sp.Spline.Knots = new List<BezierKnot>()
            {
                new BezierKnot()
                {
                    Position = owner.gameObject.transform.position,
                    Rotation = new quaternion(-0.6f, 0, 0, 0.7f),
                    TangentIn = new float3(0, 0, -0.5f),
                    TangentOut = new float3(0, 0, 0.5f),
                },
                new BezierKnot()
                {
                    Position = owner.transform.position +
                               directionNormal * Vector3.Distance(owner.transform.position,
                                   target.gameObject.transform.position) / 2 + Vector3.up * 3,
                    Rotation = new quaternion(0, 0, 0, 0),
                    TangentIn = new float3(0, 0, -0.3f),
                    TangentOut = new float3(0, 0, 0.3f),
                },
                new BezierKnot()
                {
                    Position = target.gameObject.transform.position - directionNormal,
                    Rotation = new quaternion(-0.6f, 0, 0, 0.7f),
                    TangentIn = new float3(0, 0, -0.5f),
                    TangentOut = new float3(0, 0, 0.5f),
                },
            };

            owner.splineAnimate.NormalizedTime = 0;
            owner.splineAnimate.Play();
            CoolDownTimer = Config_EffectCoolDown;
        }
    }
}