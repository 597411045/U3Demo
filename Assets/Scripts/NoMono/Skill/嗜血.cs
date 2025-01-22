using UnityEngine;

public class 嗜血 : BaseEffect
{
    public 嗜血()
    {
        name = "嗜血";
    }

    public override void ActionCallByReleaseSkill(GameObject go)
    {
        Bullet bullet = go.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.DrainBloodAble = true;
        }
    }
}