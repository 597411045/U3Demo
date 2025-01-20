using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class StateData : MonoBehaviour
{
    public bool Invincible;
    public bool AttackAble = true;
    public bool ChaseAble = false;
    public float AttackRange = 10;
    public bool JumpAble = false;

    public AnimatorStateInfo animInfo;
    public Animator animator;
    public AIController selfAic;

    private void FixedUpdate()
    {
        if (animator != null)
        {
            animInfo = animator.GetCurrentAnimatorStateInfo(0);
        }
    }

    public bool IfAnimDie()
    {
        return animInfo.shortNameHash == Animator.StringToHash("Die");
    }

    public bool IfAnimInjured()
    {
        return animInfo.shortNameHash == Animator.StringToHash("Injured");
    }

    public bool IfAnimIdle()
    {
        return animInfo.shortNameHash == Animator.StringToHash("Idle");
    }

    public bool IfAnimAttack()
    {
        return animInfo.shortNameHash == Animator.StringToHash("Melee Attack") ||
               animInfo.shortNameHash == Animator.StringToHash("Common Attack") ||
               selfAic.splineAnimate.IsPlaying;
    }

    public bool IfAnimRun()
    {
        return animInfo.shortNameHash == Animator.StringToHash("Run");
    }

    public void SetAnimRun(bool flag)
    {
        animator.SetBool("Run", flag);
    }

    public void SetAnimAttack(SkillType iterKey)
    {
        animator.SetTrigger(iterKey.ToString());
    }

    public void SetAnimInjured()
    {
        if (IfAnimInjured() == false)
        {
            animator.SetTrigger("Injured");
        }
    }

    public void SetAnimDie()
    {
        animator.SetTrigger("Die");
    }
}