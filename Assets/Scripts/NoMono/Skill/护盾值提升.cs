using UnityEngine;

public class 护盾值提升 : BaseEffect
{
    public 护盾值提升()
    {
        name = "护盾值提升";
    }

    public override void ActionCallByInitial(GameObject go)
    {
        AIController goAic = go.GetComponent<AIController>();
        if (goAic != null)
        {
            goAic.characterData.护盾值Shield += 10;
        }
    }
}