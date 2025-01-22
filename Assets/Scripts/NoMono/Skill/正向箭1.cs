using UnityEngine;

public class 正向箭1 : BaseEffect
{
    public 正向箭1()
    {
        name = "正向箭1";
    }

    public override void ActionCallByReleaseSkill(GameObject go)
    {
        AIController goAic = go.GetComponent<AIController>();
        if (goAic != null)
        {
            Bullet bulletIns1 = EntityManager.Instance.SpawnBullet(
                goAic.transform.position + goAic.transform.forward / 2 + Vector3.up + goAic.transform.right / 2,
                goAic.gameObject.transform.rotation);
            bulletIns1.caster = goAic;
            
            var dict = goAic.skillData.Get效果技能Dict();
            foreach (var iter in dict)
            {
                iter.Value.ActionCallByReleaseSkill(bulletIns1.gameObject);
            }
        }
    }
}