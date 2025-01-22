using UnityEngine;

public class 普通攻击 : BaseSkill
{
    public 普通攻击()
    {
        name = "普通攻击";
        Config_EffectCoolDown = 1;
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
            var dict = goAic.skillData.Get效果技能Dict();
            Bullet bulletIns1;
            if (dict.ContainsKey(SkillType.正向箭1))
            {
                bulletIns1 = EntityManager.Instance.SpawnBullet(
                    goAic.transform.position + goAic.transform.forward / 2 + Vector3.up - goAic.transform.right / 2,
                    goAic.gameObject.transform.rotation);
            }
            else
            {
                bulletIns1 = EntityManager.Instance.SpawnBullet(
                    goAic.transform.position + goAic.transform.forward / 2 + Vector3.up,
                    goAic.gameObject.transform.rotation);
            }

            bulletIns1.caster = goAic;
            foreach (var iter in dict)
            {
                iter.Value.ActionCallByReleaseSkill(bulletIns1.gameObject);
            }

        }
    }
}