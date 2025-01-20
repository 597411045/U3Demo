using UnityEngine;

public class 反弹墙壁 : BaseEffect
{
    public 反弹墙壁()
    {
        name = "反弹墙壁";
    }

    public override void ActionCallByReleaseSkill(GameObject go)
    {
        Bullet bullet = go.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.ReflectOneceOnWallAble = true;
        }
    }
}