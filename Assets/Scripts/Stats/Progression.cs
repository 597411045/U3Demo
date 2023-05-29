using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/Make New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] private ProgressionCharacterClass _progressionCharacterClass;

        [System.Serializable]
        class ProgressionCharacterClass
        {
            [SerializeField] private CharacterClass _characterClass;
            [SerializeField] private float[] _health;
        }
    }
}