using UnityEngine;

public class 弹射 : BaseEffect
{
    public 弹射()
    {
        name = "弹射";
    }

    public override void ActionCallByReleaseSkill(GameObject go)
    {
        Bullet bullet = go.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.ReflectOnEnemyAble = true;
        }
    }
}