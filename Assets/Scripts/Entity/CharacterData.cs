using System;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CharacterData : MonoBehaviour
{
    public AIController selfAic;

    //ID
    public int id;

    //名字
    public string characterName;

    //最大生命
    public float maxHealth;

    //当前生命
    public float currentHealth;

    //攻击
    public float 攻击力attack;

    //护盾
    public float defense;

    //闪避率
    public float 闪避率parryRate;

    //暴击率
    public float 暴击率criticalRate;

    //暴击伤害
    public float 暴击伤害criticalDamage;

    //移速
    public float 移动速度speed;

    public float 护盾值Shield;

    public float 变身所需能量减少NeedEnergy;

    public float 能量CurrentEnergy;



    //伤害流程
    public bool GetDamage(AIController caster)
    {
        if (selfAic.stateData != null && selfAic.stateData.Invincible == true)
        {
            Debug.Log("Invincible");
            return false;
        }

        var isCauseCritical = false;
        var damageResult = caster.characterData.攻击力attack;
        //闪避计算
        if (Random.Range(0f, 1f) < 闪避率parryRate)
        {
            Debug.Log("Parry");
            return false;
        }

        //暴击计算
        if (Random.Range(0f, 1f) < 暴击率criticalRate)
        {
            //打出了暴击
            Debug.Log("Critical");
            var criticalDamageAddon = caster.characterData.攻击力attack * (1 + 暴击伤害criticalDamage);
            damageResult += criticalDamageAddon;
            isCauseCritical = true;
        }

        //上血
        Damage(selfAic, damageResult, isCauseCritical);
        //玩家自行处理死亡
        return true;
    }

    public void Damage(AIController receiver, float damageResult, bool isCauseCritical)
    {
        receiver.characterData.currentHealth -= damageResult;

        if (receiver.gameObject.tag == "Player" || receiver.gameObject.tag == "Enemy")
        {
            //跳字
        }
    }

    public bool Cure(AIController receiver, float value)
    {
        //上血
        receiver.characterData.currentHealth += value;
        if (receiver.characterData.currentHealth >= receiver.characterData.maxHealth)
        {
            receiver.characterData.currentHealth = receiver.characterData.maxHealth;
        }

        //跳字

        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public string ExportData()
    {
        StringBuilder data = new StringBuilder();
        data.Append("<PropertyData>");
        data.Append("<最大血量>" + maxHealth.ToString() + "<最大血量>");
        data.Append("<现有血量>" + currentHealth.ToString() + "<现有血量>");
        data.Append("<攻击力>" + 攻击力attack.ToString() + "<攻击力>");
        data.Append("<防御力>" + defense.ToString() + "<防御力>");
        data.Append("<闪避率>" + 闪避率parryRate.ToString() + "<闪避率>");
        data.Append("<暴击率>" + 暴击率criticalRate.ToString() + "<暴击率>");
        data.Append("<暴击伤害>" + 暴击伤害criticalDamage.ToString() + "<最大血量>");
        data.Append("<移动速度>" + 移动速度speed.ToString() + "<移动速度>");
        data.Append("<PropertyData>");
        return data.ToString();
    }

    public void ImportData(string str)
    {
        var data = str.Split("<最大血量>");
        foreach (var iter in data)
        {
            if (iter != "")
            {
                float tmp = 0;
                if (float.TryParse(iter, out tmp))
                {
                    maxHealth = tmp;
                }
            }
        }

        data = str.Split("<现有血量>");
        foreach (var iter in data)
        {
            if (iter != "")
            {
                float tmp = 0;
                if (float.TryParse(iter, out tmp))
                {
                    currentHealth = tmp;
                }
            }
        }

        data = str.Split("<攻击力>");
        foreach (var iter in data)
        {
            if (iter != "")
            {
                float tmp = 0;
                if (float.TryParse(iter, out tmp))
                {
                    攻击力attack = tmp;
                }
            }
        }

        data = str.Split("<防御力>");
        foreach (var iter in data)
        {
            if (iter != "")
            {
                float tmp = 0;
                if (float.TryParse(iter, out tmp))
                {
                    defense = tmp;
                }
            }
        }

        data = str.Split("<闪避率>");
        foreach (var iter in data)
        {
            if (iter != "")
            {
                float tmp = 0;
                if (float.TryParse(iter, out tmp))
                {
                    闪避率parryRate = tmp;
                }
            }
        }

        data = str.Split("<暴击率>");
        foreach (var iter in data)
        {
            if (iter != "")
            {
                float tmp = 0;
                if (float.TryParse(iter, out tmp))
                {
                    暴击率criticalRate = tmp;
                }
            }
        }

        data = str.Split("<暴击伤害>");
        foreach (var iter in data)
        {
            if (iter != "")
            {
                float tmp = 0;
                if (float.TryParse(iter, out tmp))
                {
                    暴击伤害criticalDamage = tmp;
                }
            }
        }

        data = str.Split("<移动速度>");
        foreach (var iter in data)
        {
            if (iter != "")
            {
                float tmp = 0;
                if (float.TryParse(iter, out tmp))
                {
                    移动速度speed = tmp;
                }
            }
        }
    }
}