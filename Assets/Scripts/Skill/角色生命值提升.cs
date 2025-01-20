using UnityEngine;

public class 角色生命值提升 : BaseEffect
{
    public 角色生命值提升()
    {
        name = "角色生命值提升";
    }

    public override void ActionCallByInitial(GameObject go)
    {
        AIController goAic = go.GetComponent<AIController>();
        if (goAic != null)
        {
            goAic.characterData.maxHealth += 10;
            goAic.characterData.Cure(goAic, 10);
        }
    }
}