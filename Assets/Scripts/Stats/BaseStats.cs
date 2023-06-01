using System;
using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json.Linq;
using RPG.Core;
using RPG.Saving;
using RPG.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour, IJsonSaveable, IModifierProvider
    {
        [Range(1, 99)] [SerializeField] private int startingLevel = 1;

        [FormerlySerializedAs("characterClass")] [SerializeField]
        private CharacterEnum characterEnum;

        [SerializeField] private Progression progression;
        [SerializeField] public GameObject levelUpEffect = null;


        private float _exp;
        private float _hp;
        private float _maxHp;
        private float[] _explevels;

        private void Awake()
        {
            _maxHp = progression.GetData(characterEnum, ProgressionEnum.Health, startingLevel);
            _hp = _maxHp;

            _explevels = progression.GetRawData(characterEnum, ProgressionEnum.TotalExpToLevel);
            OnSetEXP += CalcuLevel;

            OnLevelUp += GenLevelUpEffect;
            OnLevelUp += RestoreHP;
        }

        public float HP
        {
            set
            {
                _hp = value;
                this.GetComponent<HealthComponent>().CheckIfDead();
                this.GetComponent<UIControl>().UpdateHpBar(HP / _maxHp);
            }
            get { return _hp; }
        }

        public float MAXHP
        {
            get { return _maxHp; }
        }

        public void GainExp(float maxHp)
        {
            EXP += maxHp;
        }

        public delegate void OnSetEXPDelegate();

        public event OnSetEXPDelegate OnSetEXP;

        public float EXP
        {
            set
            {
                _exp = value;

                OnSetEXP.Invoke();
            }
            get { return _exp; }
        }

        public float GetExpReward()
        {
            return progression.GetData(characterEnum, ProgressionEnum.Exp, Level);
        }

        public JToken CaptureAsJTokenInInterface()
        {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;
            stateDict["Exp"] = _exp;
            stateDict["HP"] = _hp;
            return state;
        }

        public void RestoreFormJToken(JToken s)
        {
            JObject state = s.ToObject<JObject>();
            IDictionary<string, JToken> stateDict = state;
            EXP = stateDict["Exp"].ToObject<float>();
            HP = stateDict["HP"].ToObject<float>();
        }


        public event Action OnLevelUp;

        public int Level
        {
            set { startingLevel = value; }
            get { return startingLevel; }
        }

        private void CalcuLevel()
        {
            bool flag = false;
            for (int i = Level; i < _explevels.Length; i++)
            {
                if (_exp >= _explevels[i])
                {
                    Level = i + 1;
                    flag = true;
                }
            }

            if (flag)
            {
                OnLevelUp.Invoke();
            }
        }

        private void GenLevelUpEffect()
        {
            Instantiate(levelUpEffect, this.transform);
        }

        private void RestoreHP()
        {
            HP = _maxHp;
        }

        public float GetAllAdditiveModifier(ProgressionEnum b)
        {
            float sum = 0;

            foreach (var c in GetComponents<IModifierProvider>())
            {
                float percent = 0;
                float unit = 0;
                foreach (var d in c.GetAdditiveModifier(b))
                {
                    unit += d;
                }

                foreach (var d in c.GetPercentageModifier(b))
                {
                    percent += d;
                }

                sum += unit * percent;
            }

            return sum;
        }

        public IEnumerable<float> GetAdditiveModifier(ProgressionEnum b)
        {
            if (b == ProgressionEnum.Damage)
            {
                yield return 1;
                yield return 2;
            }
        }

        public IEnumerable<float> GetPercentageModifier(ProgressionEnum b)
        {
            yield return 0.5f;
        }
    }
}