using UnityEngine;

public class 攻击力提升 : BaseEffect
{
    public 攻击力提升()
    {
        name = "攻击力提升";
    }

    public override void ActionCallByInitial(GameObject go)
    {
        AIController goAic = go.GetComponent<AIController>();
        if (goAic != null)
        {
            goAic.characterData.攻击力attack += 10;
        }
    }
}