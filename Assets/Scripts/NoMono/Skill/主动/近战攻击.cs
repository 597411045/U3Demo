using UnityEngine;

public class 近战攻击 : BaseSkill
{
    public 近战攻击()
    {
        name = "近战攻击";
        Config_EffectCoolDown = 2;
    }

    public override void ActionCallByInitial(GameObject go)
    {
        GameMode.Instance.AddActionToUpdateNoOrderListBuffer(CoolDown);
    }

    public override void ActionCallByReleaseSkill(GameObject go)
    {
        AIController goAic = go.GetComponent<AIController>();
        if (goAic != null)
        {
            FunctionItem item = EntityManager.Instance.SpawnBoxDamage(
                goAic.transform.position + goAic.transform.forward + Vector3.up,
                goAic.gameObject.transform.rotation);
            item.selfAic = goAic;
        }
    }
}