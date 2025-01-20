using UnityEngine;

public class 穿透 : BaseEffect
{
    public 穿透()
    {
        name = "穿透";
    }

    public override void ActionCallByReleaseSkill(GameObject go)
    {
        Bullet bullet = go.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.PenetrateAble = true;
        }
    }
}