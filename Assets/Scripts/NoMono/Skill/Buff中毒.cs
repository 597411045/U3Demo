using System;
using System.Collections;
using UnityEngine;

public class Buff中毒 : BaseBuff
{
    private float IntervalTimer;
    private float DurationTimer;

    public Buff中毒()
    {
        name = "Buff中毒";
        Config_Duration = 3;
        Config_EffectCoolDown = 1;
        DurationTimer = 0;
        IntervalTimer = 0;
    }

    public override void ActionCallByInitial(GameObject go)
    {
        receiver = go.GetComponent<AIController>();
        if (receiver != null)
        {
            GameMode.Instance.AddActionToUpdateNoOrderListBuffer(SkillAction);
        }
    }

    public override void WhenFreshAction()
    {
        DurationTimer = 0;
    }

    private void SkillAction(float deltaTime)
    {
        if (DurationTimer < Config_Duration)
        {
            DurationTimer += deltaTime;
        }

        if (IntervalTimer < Config_EffectCoolDown)
        {
            IntervalTimer += deltaTime;
        }

        if (IntervalTimer >= Config_EffectCoolDown)
        {
            PlayerController.Instance.currentChaAic.characterData.GetDamage(caster);
            IntervalTimer = 0;
        }

        if (DurationTimer > Config_Duration)
        {
            GameMode.Instance.RemoveActionToUpdateNoOrderListBuffer(SkillAction);
            parent.BuffDict.Remove(refType);
        }
    }
}