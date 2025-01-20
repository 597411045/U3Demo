using System;
using System.Collections;
using UnityEngine;

public class Buff无敌 : BaseBuff
{
    private float IntervalTimer = 5;
    private float DurationTimer = 0;

    public Buff无敌()
    {
        name = "Buff无敌";
        Config_EffectCoolDown = 5;
        Config_Duration = 1;
    }

    public override void ActionCallByInitial(GameObject go)
    {
        GameMode.Instance.AddActionToUpdateNoOrderListBuffer(SkillAction);
        IntervalTimer = Config_EffectCoolDown;
        DurationTimer = Config_Duration;
    }

    private void SkillAction(float deltaTime)
    {
        if (IntervalTimer > 0 && DurationTimer <= 0)
        {
            IntervalTimer -= deltaTime;
        }

        if (IntervalTimer <= 0 && DurationTimer <= 0)
        {
            PlayerController.Instance.currentChaAic.stateData.Invincible = true;
            DurationTimer = Config_Duration;
            return;
        }

        if (DurationTimer > 0)
        {
            DurationTimer -= deltaTime;
        }

        if (DurationTimer <= 0 && IntervalTimer <= 0)
        {
            PlayerController.Instance.currentChaAic.stateData.Invincible = false;
            IntervalTimer = Config_EffectCoolDown;
        }
    }
}