using UnityEngine;
using UnityEngine.Serialization;

public class AISense : MonoBehaviour
{
    public AIController selfAic = null;

    private void OnTriggerEnter(Collider other)
    {
        var ai = other.gameObject.GetComponent<AIController>();
        if (ai != null && ai.gameObject.tag != selfAic.gameObject.tag && selfAic.characterData.currentHealth >= 0 &&
            ai.gameObject.tag != "Wall" && ai.gameObject.tag != "Damage")
        {
            selfAic.targets.Add(ai);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var ai = other.gameObject.GetComponent<AIController>();
        if (ai != null)
        {
            selfAic.targets.Remove(ai);
        }
    }
}