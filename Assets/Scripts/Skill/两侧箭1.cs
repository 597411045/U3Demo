using UnityEngine;

public class 两侧箭1 : BaseEffect
{
    public 两侧箭1()
    {
        name = "两侧箭1";
    }

    public override void ActionCallByReleaseSkill(GameObject go)
    {
        AIController goAic = go.GetComponent<AIController>();
        if (goAic != null)
        {
            Bullet bulletIns1 = EntityManager.Instance.SpawnBullet(
                goAic.transform.position + goAic.transform.right / 2 + Vector3.up,
                goAic.gameObject.transform.rotation);
            bulletIns1.transform.forward = bulletIns1.transform.right;
            bulletIns1.caster = goAic;

            Bullet bulletIns2 = EntityManager.Instance.SpawnBullet(
                goAic.transform.position - goAic.transform.right / 2 + Vector3.up,
                goAic.gameObject.transform.rotation);
            bulletIns2.transform.forward = -bulletIns2.transform.right;
            bulletIns2.caster = goAic;

            var dict = goAic.skillData.Get效果技能Dict();
            foreach (var iter in dict)
            {
                iter.Value.ActionCallByReleaseSkill(bulletIns1.gameObject);
                iter.Value.ActionCallByReleaseSkill(bulletIns2.gameObject);
            }
        }
    }
}