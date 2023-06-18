using System.Collections.Generic;
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

        private Dictionary<CharacterEnum, Dictionary<ProgressionEnum, float[]>> dic;

        public float GetData(CharacterEnum e, ProgressionEnum e2, int level)
        {
            BuildDic();
            float[] tmp = dic[e][e2];
            return tmp[(level - 1) < tmp.Length - 1 ? (level - 1) : tmp.Length - 1];
        }
        
        public float[] GetRawData(CharacterEnum e, ProgressionEnum e2)
        {
            BuildDic();
            return  dic[e][e2];
            return  dic[e][e2];
        }

        private void BuildDic()
        {
            if (dic != null) return;
            dic = new Dictionary<CharacterEnum, Dictionary<ProgressionEnum, float[]>>();

            foreach (var c in _progressionCharacterClass)
            {
                dic[c._characterEnum] = new Dictionary<ProgressionEnum, float[]>();
                foreach (var d in c._progressionStats)
                {
                    dic[c._characterEnum][d._progressionEnum] = d._levels;
                }
            }
        }
    }
}