using UnityEngine;

public class 召唤近战敌人 : BaseSkill
{
    private float CoolDownTimer;

    public 召唤近战敌人()
    {
        name = "召唤近战敌人";
        Config_EffectCoolDown = 5;
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
            EntityManager.Instance.SpawnMeleeEnemy(
                goAic.transform.position + goAic.transform.forward,
                goAic.gameObject.transform.rotation);

            CoolDownTimer = Config_EffectCoolDown;
        }
    }
}