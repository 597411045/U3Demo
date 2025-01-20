using System.Collections;
using UnityEngine;

public class 连续射击 : BaseEffect
{
    public 连续射击()
    {
        name = "连续射击";
    }

    public override void ActionCallByReleaseSkill(GameObject go)
    {
        AIController goAic = go.GetComponent<AIController>();
        if (goAic != null)
        {
            parent.StartCoroutine(DelayAction(go));
        }
    }

    IEnumerator DelayAction(GameObject go)
    {
        yield return new WaitForSeconds(0.2f);
        AIController goAic = go.GetComponent<AIController>();
        if (goAic != null)
        {
            goAic.skillData.curSkill.ActionCallByReleaseSkill(goAic.gameObject);

            var dict = goAic.skillData.Get效果技能Dict();
            foreach (var iter in dict)
            {
                if (iter.Key != SkillType.连续射击)
                {
                    iter.Value.ActionCallByReleaseSkill(go);
                }
            }
        }
    }
}