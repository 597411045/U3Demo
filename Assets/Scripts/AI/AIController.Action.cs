using UnityEngine;

public partial class AIController : MonoBehaviour
{
    //由动画时间调用，注意改名
    public void Fire()
    {
        skillData.curSkill.ActionCallByReleaseSkill(this.gameObject);

        var dict2 = skillData.Get效果技能Dict();

        foreach (var iter in dict2)
        {
            iter.Value.ActionCallByReleaseSkill(this.gameObject);
        }
    }

    public void Move(Vector3 direction)
    {
        stateData.AttackAble = false;
        if ((stateData.IfAnimIdle() || stateData.IfAnimRun()) && stateData.IfAnimAttack() == false)
        {
            direction = direction.normalized;
            direction *= characterData.移动速度speed;
            SetVelocity(direction);

            if (direction != Vector3.zero)
            {
                transform.forward = direction;
                stateData.SetAnimRun(true);
            }
        }
    }

    public void StopMove()
    {
        stateData.SetAnimRun(false);
        SetVelocity(new Vector3(0, rigidbody.velocity.y, 0));
    }

    public bool AIInjured(AIController caster)
    {
        //伤害流程
        if (characterData.GetDamage(caster))
        {
            if (stateData != null)
            {
                if (gameObject.tag == "Enemy")
                {
                    stateData.SetAnimInjured();
                }
            }

            return true;
        }

        return false;
    }

    public void Die()
    {
        stateData.SetAnimDie();
        var cc = this.GetComponent<CapsuleCollider>();
        rigidbody.drag = 100;
        rigidbody.excludeLayers = LayerMask.GetMask("Default");
    }

    public void CheckSatus()
    {
        if (characterData.currentHealth <= 0)
        {
            Die();
        }
    }

    public void CheckSkill()
    {
        var dict = skillData.Get主动技能Dict();
        foreach (var iter in dict)
        {
            if (iter.Value is BaseSkill skill)
            {
                if (skill.IsReady())
                {
                    skillData.curSkill = skill;
                    skill.BeginCoolDown();
                    stateData.SetAnimAttack(skill.refType);
                    return;
                }
            }
        }
    }

    public void SetVelocity(Vector3 v3)
    {
        if (rigidbody.isKinematic == false)
        {
            rigidbody.velocity = v3;
        }
    }
}