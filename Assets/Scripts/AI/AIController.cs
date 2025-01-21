using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public partial class AIController : MonoBehaviour
{
    public Rigidbody rigidbody;
    public SplineAnimate splineAnimate;
    public CharacterData characterData;
    public SkillData skillData;
    public StateData stateData;

    private void Awake()
    {
        if (gameObject.tag == "Player" || gameObject.tag == "Enemy")
        {
            GameMode.Instance.AddActionToAwakeNoOrderList(M_Start);
            GameMode.Instance.AddActionToFixedUpdateOrderDic(100, M_FixedUpdate);
        }
    }

    private void M_Start()
    {
    }

    private void M_FixedUpdate()
    {
        if (stateData.IfAnimRun())
        {
            rigidbody.isKinematic = false;
        }
        else
        {
            rigidbody.isKinematic = true;
        }

        //如果死亡动画, return
        if (stateData.IfAnimDie())
        {
            return;
        }

        //如果在攻击中, return
        if (stateData.IfAnimAttack())
        {
            return;
        }

        //如果受伤动画，停止移动，停止跑步
        if (stateData.IfAnimInjured())
        {
            rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, 0);
            stateData.SetAnimRun(false);
        }


        //如果目标列表有对象
        Vector3 direction = Vector3.zero;
        Vector3 directionNormal = Vector3.zero;
        AIController target = EntityManager.Instance.GetCallerNearestEnemy(this);
        if (target != null)
        {
            //和目标的原始方向
            direction = (target.gameObject.transform.position - this.gameObject.transform.position);
            //个目标的单位方向
            directionNormal = direction.normalized;
            directionNormal.y = 0;

            //如果空闲或者攻击动画中，设置朝向
            if (stateData.IfAnimIdle())
            {
                this.gameObject.transform.forward = directionNormal;
            }


            if (stateData.ChaseAble && (stateData.IfAnimIdle() || stateData.IfAnimRun()))
            {
                if (stateData.IfAnimAttack() == false && stateData.IfAnimInjured() == false &&
                    direction.magnitude > stateData.AttackRange)
                {
                    directionNormal *= characterData.移动速度speed;
                    rigidbody.velocity = new Vector3(directionNormal.x, rigidbody.velocity.y, directionNormal.z);
                    this.gameObject.transform.forward = directionNormal;
                    stateData.SetAnimRun(true);
                }
                else
                {
                    stateData.SetAnimRun(false);
                    rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, 0);
                }
            }

            if (stateData.AttackAble && direction.magnitude < stateData.AttackRange)
            {
                if (stateData.IfAnimIdle() || stateData.IfAnimRun())
                {
                    this.gameObject.transform.forward = directionNormal;
                    StopMove();
                    CheckSkill();
                }
            }
        }
        else if (gameObject.tag != "Player")
        {
            StopMove();
        }

        CheckSatus();
    }
}