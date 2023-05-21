using UnityEngine;

namespace RPG.Combat
{
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] private float health = 100;

        public void TakeDamage(float damage)
        {
            health = Mathf.Max(health - damage, 0);
            Debug.Log(health);
        }
    }
}