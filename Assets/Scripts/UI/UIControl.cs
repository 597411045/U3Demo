using System;
using RPG.Combat;
using RPG.Core;
using RPG.Stats;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIControl : MonoBehaviour
    {
        [SerializeField] Text textHP;
        [SerializeField] GameObject user;
        [SerializeField] Text targetHP;
        [SerializeField] Text levelText;
        [SerializeField] Text expText;

        private void Awake()
        {
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
    }
}