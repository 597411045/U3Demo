using UnityEngine;

public class 暴击伤害提升 : BaseEffect
{
    public 暴击伤害提升()
    {
        name = "暴击伤害提升";
    }

    public override void ActionCallByInitial(GameObject go)
    {
        AIController goAic = go.GetComponent<AIController>();
        if (goAic != null)
        {
            goAic.characterData.暴击伤害criticalDamage += 10;
        }
    }
}