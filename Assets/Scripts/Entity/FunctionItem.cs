using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class FunctionItem : MonoBehaviour
{
    public bool 可破坏;
    public bool 在内每秒伤害;
    public bool 在内持续中毒;
    public bool DoOnce = false;
    public float Interval;
    public float Config_Interval = 1;

    public AIController selfAic;
    public Collider collider;

    [NonSerialized] public List<AIController> otherAiList = new List<AIController>();
    [NonSerialized] public List<AIController> aisReadyToMove = new List<AIController>();

    // Start is called before the first frame update
    void Start()
    {
        if (在内每秒伤害 || 在内持续中毒)
        {
            collider.isTrigger = true;
        }

        Interval = Config_Interval;
    }

    // Update is called once per frame

    public void ItemInjured(AIController caster)
    {
        if (可破坏)
        {
            selfAic.AIInjured(caster);
            if (selfAic.characterData.currentHealth <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void Update()
    {
        if (Interval > 0)
        {
            Interval -= Time.deltaTime;
        }

        if (Interval <= 0)
        {
            if (otherAiList.Count > 0)
            {
                for (int i = otherAiList.Count - 1; i >= 0; i--)
                {
                    if (在内每秒伤害)
                    {
                        otherAiList[i].AIInjured(selfAic);
                    }
                    else if (在内持续中毒)
                    {
                        var buff = otherAiList[i].skillData.AddEffectToSkillDict(SkillType.Buff中毒) as BaseBuff;
                        buff.caster = selfAic;
                    }
                }
            }

            Interval = Config_Interval;
        }


        if (aisReadyToMove.Count > 0)
        {
            foreach (var iter in aisReadyToMove)
            {
                if (otherAiList.Contains(iter))
                {
                    otherAiList.Remove(iter);
                }
            }

            aisReadyToMove.Clear();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != selfAic.tag)
        {
            AIController otherAic = other.GetComponent<AIController>();
            {
                if (otherAic != null)
                {
                    if (在内每秒伤害 || 在内持续中毒)
                    {
                        otherAiList.Add(otherAic);
                    }
                    else if (DoOnce == true)
                    {
                        otherAic.AIInjured(selfAic);
                        DoOnce = false;
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != selfAic.tag)
        {
            AIController otherAic = other.GetComponent<AIController>();
            {
                if (otherAic != null)
                {
                    aisReadyToMove.Add(otherAic);
                }
            }
        }
    }
}