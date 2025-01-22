using UnityEngine;

public class 背向箭1 : BaseEffect
{
    public 背向箭1()
    {
        name = "背向箭1";
    }

    public override void ActionCallByReleaseSkill(GameObject go)
    {
        AIController goAic = go.GetComponent<AIController>();
        if (goAic != null)
        {
            Bullet bulletIns = EntityManager.Instance.SpawnBullet(
                goAic.transform.position - goAic.transform.forward / 2 + Vector3.up,
                goAic.gameObject.transform.rotation);
            bulletIns.transform.forward = -bulletIns.transform.forward;
            bulletIns.caster = goAic;
            
            var dict = goAic.skillData.Get效果技能Dict();
            foreach (var iter in dict)
            {
                iter.Value.ActionCallByReleaseSkill(bulletIns.gameObject);
            }
        }
    }
}