using System;
using RPG.Combat;
using RPG.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIControl : MonoBehaviour
    {
        [SerializeField] Text textHP;
        [SerializeField] GameObject user;
        [SerializeField] Text targetHP;

        public void UpdateUI()
        {
            if (user == null) return;
            textHP.text = $"Player HP:{user.GetComponent<HealthComponent>().GetHealthPercentage().ToString("F")}%";
        }

        private void Update()
        {
            if (user == null) return;
            targetHP.text = $"Target HP:{user.GetComponent<FighterActionComponent>().GetTargetHealthPercentage().ToString("F")}%";
        }
    }
}