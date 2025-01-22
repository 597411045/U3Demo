using UnityEngine;

public class 暴击率提升 : BaseEffect
{
    public 暴击率提升()
    {
        name = "暴击率提升";
    }

    public override void ActionCallByInitial(GameObject go)
    {
        AIController goAic = go.GetComponent<AIController>();
        if (goAic != null)
        {
            goAic.characterData.暴击率criticalRate += 10;
        }
    }
}