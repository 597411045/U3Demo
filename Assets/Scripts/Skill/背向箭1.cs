using UnityEngine;

public class 斜向箭1 : BaseEffect
{
    public 斜向箭1()
    {
        name = "斜向箭1";
    }

    public override void ActionCallByReleaseSkill(GameObject go)
    {
        AIController goAic = go.GetComponent<AIController>();
        if (goAic != null)
        {
            Bullet bulletIns1 = GameMode.Instance.entityManager.SpawnBullet(
                goAic.transform.position + goAic.transform.forward / 2 + Vector3.up,
                goAic.gameObject.transform.rotation);
            bulletIns1.transform.rotation *= Quaternion.AngleAxis(45, Vector3.up);
            bulletIns1.caster = goAic;


            Bullet bulletIns2 = GameMode.Instance.entityManager.SpawnBullet(
                goAic.transform.position + goAic.transform.forward / 2 + Vector3.up,
                goAic.gameObject.transform.rotation);
            bulletIns2.transform.rotation *= Quaternion.AngleAxis(-45, Vector3.up);
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