using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/Make New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] private ProgressionCharacterClass[] _progressionCharacterClass;

        [System.Serializable]
        public class ProgressionCharacterClass
        {
            [SerializeField] public CharacterEnum _characterEnum;

            [SerializeField] public ProgressionStat[] _progressionStats;
        }

        [System.Serializable]
        public class ProgressionStat
        {
            [SerializeField] public ProgressionEnum _progressionEnum;
            [SerializeField] public float[] _levels;
        }

        public float GetData(CharacterEnum e, ProgressionEnum e2, int level)
        {
            foreach (var c in _progressionCharacterClass)
            {
                if (e == c._characterEnum)
                {
                    foreach (var d in c._progressionStats)
                    {
                        if (e2 == d._progressionEnum)
                        {
                            return d._levels[(level - 1) < d._levels.Length - 1 ? (level - 1) : d._levels.Length - 1];
                        }
                    }
                }
            }

            return 0;
        }
    }
}