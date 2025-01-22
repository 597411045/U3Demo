using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class SkillData : MonoBehaviour
{
    [NonSerialized]
    public Dictionary<SkillType, BaseEffect> 效果技能Dict = new Dictionary<SkillType, BaseEffect>();

    [NonSerialized]
    public Dictionary<SkillType, BaseEffect> 主动技能Dict = new Dictionary<SkillType, BaseEffect>();

    [NonSerialized]
    public Dictionary<SkillType, BaseEffect> BuffDict = new Dictionary<SkillType, BaseEffect>();

    public List<SkillType> Config_SkillTypeList = new List<SkillType>();

    public BaseEffect curSkill;

    private BaseEffect 主动技能处理(SkillType skillType)
    {
        BaseEffect tmp = null;
        switch (skillType)
        {
            case SkillType.普通攻击:
                tmp = new 普通攻击() { parent = this };
                break;
            case SkillType.近战攻击:
                tmp = new 近战攻击() { parent = this };
                break;
            case SkillType.跳跃攻击:
                tmp = new 跳跃攻击() { parent = this };
                break;
            case SkillType.召唤近战敌人:
                tmp = new 召唤近战敌人() { parent = this };
                break;
        }

        if (tmp != null)
        {
            主动技能Dict.Add(skillType, tmp);
            tmp.ActionCallByInitial(this.gameObject);
            tmp.refType = skillType;
        }

        return tmp;
    }

    private BaseEffect 效果技能处理(SkillType skillType)
    {
        BaseEffect tmp = null;
        switch (skillType)
        {
            case SkillType.反弹墙壁:
                tmp = new 反弹墙壁() { parent = this };
                break;
            case SkillType.正向箭1:
                tmp = new 正向箭1() { parent = this };
                break;
            case SkillType.背向箭1:
                tmp = new 背向箭1() { parent = this };
                break;
            case SkillType.斜向箭1:
                tmp = new 斜向箭1() { parent = this };
                break;
            case SkillType.两侧箭1:
                tmp = new 两侧箭1() { parent = this };
                break;
            case SkillType.连续射击:
                tmp = new 连续射击() { parent = this };
                break;
            case SkillType.穿透:
                tmp = new 穿透() { parent = this };
                break;
            case SkillType.弹射:
                tmp = new 弹射() { parent = this };
                break;
            case SkillType.嗜血:
                tmp = new 嗜血() { parent = this };
                break;
        }

        if (tmp != null)
        {
            效果技能Dict.Add(skillType, tmp);
            tmp.ActionCallByInitial(this.gameObject);
            tmp.refType = skillType;
            return tmp;
        }

        return tmp;
    }

    private BaseEffect buff技能处理(SkillType skillType)
    {
        BaseEffect tmp = null;
        if (BuffDict.ContainsKey(skillType))
        {
            if (BuffDict[skillType] is BaseBuff)
            {
                ((BaseBuff)BuffDict[skillType]).WhenFreshAction();
                return BuffDict[skillType];
            }
        }

        switch (skillType)
        {
            case SkillType.C1被动:
                tmp = new C1被动() { parent = this };
                BuffDict.Add(skillType, tmp);
                break;
            case SkillType.Buff无敌:
                tmp = new Buff无敌() { parent = this };
                BuffDict.Add(skillType, tmp);
                break;
            case SkillType.Buff中毒:
                tmp = new Buff中毒() { parent = this };
                BuffDict.Add(skillType, tmp);
                break;
            case SkillType.攻击力提升:
                tmp = new 攻击力提升() { parent = this };
                break;
            case SkillType.护盾值提升:
                tmp = new 护盾值提升() { parent = this };
                break;
            case SkillType.角色生命值提升:
                tmp = new 角色生命值提升() { parent = this };
                break;
            case SkillType.闪避率提升:
                tmp = new 闪避率提升() { parent = this };
                break;
            case SkillType.暴击率提升:
                tmp = new 暴击率提升() { parent = this };
                break;
            case SkillType.暴击伤害提升:
                tmp = new 暴击伤害提升() { parent = this };
                break;
            case SkillType.变身所需能量减少:
                tmp = new 变身所需能量减少() { parent = this };
                break;
            case SkillType.移动速度提升:
                tmp = new 移动速度提升() { parent = this };
                break;
            case SkillType.美味心:
                tmp = new 美味心() { parent = this };
                break;
        }

        if (tmp != null)
        {
            tmp.ActionCallByInitial(this.gameObject);
            tmp.refType = skillType;
            return tmp;
        }

        return tmp;
    }


    public BaseEffect AddEffectToSkillDict(SkillType skillType)
    {
        BaseEffect tmp = null;
        tmp = 主动技能处理(skillType);
        if (tmp != null)
        {
            return tmp;
        }

        tmp = 效果技能处理(skillType);
        if (tmp != null)
        {
            return tmp;
        }

        tmp = buff技能处理(skillType);
        if (tmp != null)
        {
            return tmp;
        }

        return tmp;
    }

    public Dictionary<SkillType, BaseEffect> Get效果技能Dict()
    {
        return 效果技能Dict;
    }

    public Dictionary<SkillType, BaseEffect> Get主动技能Dict()
    {
        return 主动技能Dict;
    }

    private void Start()
    {
        for (int i = Config_SkillTypeList.Count - 1; i >= 0; i--)
        {
            AddEffectToSkillDict(Config_SkillTypeList[i]);
        }
    }

    private void OnDestroy()
    {
        foreach (var iter in 效果技能Dict)
        {
            iter.Value.Dispose();
        }
    }

    public string ExportData()
    {
        StringBuilder data = new StringBuilder();
        foreach (var iter in 主动技能Dict)
        {
            data.Append("<SkillData>" + iter.Key.ToString() + "<SkillData>");
        }

        foreach (var iter in 效果技能Dict)
        {
            data.Append("<SkillData>" + iter.Key.ToString() + "<SkillData>");
        }

        foreach (var iter in BuffDict)
        {
            data.Append("<SkillData>" + iter.Key.ToString() + "<SkillData>");
        }

        return data.ToString();
    }

    public void ImportData(string str)
    {
        效果技能Dict.Clear();
        主动技能Dict.Clear();
        BuffDict.Clear();
        var data = str.Split("<SkillData>");
        foreach (var iter in data)
        {
            if (iter != "")
            {
                AddEffectToSkillDict(Enum.Parse<SkillType>(iter));
            }
        }
    }
}

public enum SkillType
{
    普通攻击 = 0,
    攻击力提升 = 1,
    护盾值提升 = 2,
    角色生命值提升 = 3,
    闪避率提升 = 4,
    暴击率提升 = 5,
    暴击伤害提升 = 6,
    变身所需能量减少 = 7,
    移动速度提升 = 8,
    美味心 = 9,
    反弹墙壁 = 10,
    正向箭1 = 11,
    背向箭1 = 12,
    斜向箭1 = 13,
    两侧箭1 = 14,
    连续射击 = 15,
    穿透 = 16,
    嗜血 = 17,
    弹射 = 18,
    Buff无敌 = 19,
    C1被动 = 20,
    Buff中毒 = 21,
    近战攻击 = 22,
    跳跃攻击 = 23,
    召唤近战敌人 = 24,
}

[Serializable]
public class BaseEffect
{
    public SkillData parent;
    public SkillType refType;
    public string name;
    public float Config_EffectCoolDown;


    public virtual void ActionCallByReleaseSkill(GameObject go)
    {
    }

    public virtual void ActionCallByInitial(GameObject go)
    {
    }

    public virtual void Dispose()
    {
    }
}

public class BaseSkill : BaseEffect
{
    protected float CoolDownTimer;

    public virtual bool IsReady()
    {
        return CoolDownTimer <= 0;
    }

    public void BeginCoolDown()
    {
        CoolDownTimer = Config_EffectCoolDown;
    }
    
    protected void CoolDown(float deltaTime)
    {
        if (CoolDownTimer > 0)
        {
            CoolDownTimer -= deltaTime;
        }
    }
}

public class BaseBuff : BaseEffect
{
    public AIController caster;
    public AIController receiver;
    public float Config_Duration;

    public virtual void WhenFreshAction()
    {
    }
}