using RPG.Stats;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Consume", menuName = "Consume/Make New Consume")]
    public class ConsumeConfig : ScriptableObject, IItemAction
    {
        [SerializeField] public GameObject ConsumePrefab;
        [SerializeField] public float consumeValue;

        public void DoAction(GameObject go)
        {
            go.GetComponent<BaseStats>().HP += consumeValue;
        }
    }
}