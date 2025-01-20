using UnityEngine;

public class 美味心 : BaseEffect
{
    public 美味心()
    {
        name = "美味心";
    }

    public override void ActionCallByInitial(GameObject go)
    {
        AIController goAic = go.GetComponent<AIController>();
        if (goAic != null)
        {
            goAic.characterData.Cure(goAic, 50);
        }
    }
}