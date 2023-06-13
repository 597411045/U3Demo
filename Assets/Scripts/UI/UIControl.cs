using System;
using Google.Protobuf.WellKnownTypes;
using PRG.Network;
using RPG.Combat;
using RPG.Core;
using RPG.Stats;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class UIControl : TaskPipelineBase<UIControl>, ILocalCompute
    {
        [SerializeField] Text textHP;
        [SerializeField] GameObject user;
        [SerializeField] Text targetHP;
        [SerializeField] Text levelText;
        [SerializeField] Text expText;
        [SerializeField] GameObject damageTextPrefab;


        [SerializeField] private Transform _canvas;
        [SerializeField] private RectTransform _hpBarRT;

        private void Awake()
        {
            if (NetworkManagement.isServer) return;

            textHP = GameObject.Find("HP_Text").GetComponent<Text>();
            targetHP = GameObject.Find("TargetHP_Text").GetComponent<Text>();
            levelText = GameObject.Find("Level_Text").GetComponent<Text>();
            expText = GameObject.Find("Exp_Text").GetComponent<Text>();

            oldSizeDelta = _hpBarRT.sizeDelta;

            if (user == null) return;

            this.GetComponent<BaseStats>().OnSetEXP += () =>
            {
                expText.text = $"Exp:{user.GetComponent<BaseStats>().EXP}";
            };
        }

        public void LocalCompute()
        {
            if (NetworkManagement.isServer) return;

            if (user == null) return;
            targetHP.text =
                $"Target HP:{user.GetComponent<FighterActionComponent>().GetTargetHP()}";
            textHP.text = $"Player HP:{user.GetComponent<BaseStats>().HP}/{user.GetComponent<BaseStats>().MAXHP}";
            levelText.text = $"Level:{user.GetComponent<BaseStats>().Level}";
        }

        public void SpawnDamageText(float value)
        {
            GameObject go = Instantiate(damageTextPrefab, _canvas.transform);
            go.GetComponent<CharacterUIComponent>().timer = 1;
            go.GetComponent<Text>().text = value.ToString();
        }

        private Vector2 oldSizeDelta;

        public void UpdateHpBar(float percent)
        {
            _hpBarRT.sizeDelta = new Vector2(oldSizeDelta.x * percent, oldSizeDelta.y);
            if (percent <= 0)
            {
                _canvas.GetComponent<Canvas>().enabled = false;
            }
        }
    }
}