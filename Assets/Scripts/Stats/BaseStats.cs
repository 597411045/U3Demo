using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPG.Saving;
using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour,IJsonSaveable
    {
        [Range(1, 99)] [SerializeField] private int startingLevel = 1;

        [FormerlySerializedAs("characterClass")] [SerializeField]
        private CharacterEnum characterEnum;

        [SerializeField] private Progression progression;

        [SerializeField] private float _exp;

        public float GetHealth()
        {
            return progression.GetData(characterEnum, ProgressionEnum.Health, startingLevel);
        }


        public void GainExp(float maxHp)
        {
            _exp += maxHp;
        }

        public float GetExpValue()
        {
            return progression.GetData(characterEnum, ProgressionEnum.Exp, startingLevel);
        }

        public JToken CaptureAsJTokenInInterface()
        {
            JObject state = new JObject();
            IDictionary<string,JToken> stateDict= state;
            stateDict["Exp"] = _exp;
            return state;
        }

        public void RestoreFormJToken(JToken s)
        {
            JObject state = s.ToObject<JObject>();
            IDictionary<string,JToken> stateDict= state;
            _exp = stateDict["Exp"].ToObject<float>();
        }
    }
}