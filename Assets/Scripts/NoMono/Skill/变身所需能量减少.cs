using UnityEngine;

public class 变身所需能量减少 : BaseEffect
{
    public 变身所需能量减少()
    {
        name = "变身所需能量减少";
    }

    public override void ActionCallByInitial(GameObject go)
    {
        AIController goAic = go.GetComponent<AIController>();
        if (goAic != null)
        {
            goAic.characterData.变身所需能量减少NeedEnergy += 10;
        }
    }
}