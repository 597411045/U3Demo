using System;
using Google.Protobuf.WellKnownTypes;
using RPG.Combat;
using RPG.Core;
using RPG.Stats;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class UIControl : MonoBehaviour
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
            //_canvas = this.gameObject.GetComponentInChildren<Canvas>();

            //Transform tmp;
            //this.transform.FindAlongChild("HP", out tmp, true);
            //_hpBarRT = tmp.GetComponent<RectTransform>();
            oldSizeDelta = _hpBarRT.sizeDelta;

            if (user == null) return;

            this.GetComponent<BaseStats>().OnSetEXP += () =>
            {
                expText.text = $"Exp:{user.GetComponent<BaseStats>().EXP}";
            };
        }

        private void Update()
        {
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