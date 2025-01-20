using UnityEngine;

public class 移动速度提升 : BaseEffect
{
    public 移动速度提升()
    {
        name = "移动速度提升";
    }

    public override void ActionCallByInitial(GameObject go)
    {
        AIController goAic = go.GetComponent<AIController>();
        if (goAic != null)
        {
            goAic.characterData.移动速度speed += 1;
        }
    }
}