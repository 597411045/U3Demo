using UnityEngine;

public class 近战攻击 : BaseSkill
{
    private float CoolDownTimer;

    public 近战攻击()
    {
        name = "近战攻击";
        Config_EffectCoolDown = 2;
    }

    public override void ActionCallByInitial(GameObject go)
    {
        GameMode.Instance.AddActionToUpdateNoOrderListBuffer(CoolDown);
    }

    public void CoolDown(float deltaTime)
    {
        if (CoolDownTimer > 0)
        {
            CoolDownTimer -= deltaTime;
        }
    }

    public override bool IsReady()
    {
        return CoolDownTimer <= 0;
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
            CoolDownTimer = Config_EffectCoolDown;
        }
    }
}