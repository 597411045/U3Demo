using UnityEngine;

public class 召唤近战敌人 : BaseSkill
{
    public 召唤近战敌人()
    {
        name = "召唤近战敌人";
        Config_EffectCoolDown = 5;
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
            EntityManager.Instance.SpawnMeleeEnemy(
                goAic.transform.position + goAic.transform.forward,
                goAic.gameObject.transform.rotation);

        }
    }
}