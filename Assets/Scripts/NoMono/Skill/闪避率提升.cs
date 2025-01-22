using UnityEngine;

public class 闪避率提升 : BaseEffect
{
    public 闪避率提升()
    {
        name = "闪避率提升";
    }

    public override void ActionCallByInitial(GameObject go)
    {
        AIController goAic = go.GetComponent<AIController>();
        if (goAic != null)
        {
            goAic.characterData.闪避率parryRate += 10;
        }
    }
}