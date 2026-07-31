using System;
using UnityEngine;

namespace LendasDoQuintal.Core
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 5;
        [SerializeField] private bool destroyOnDeath = true;

        public event Action<int, int> Changed;
        public event Action Died;

        public int CurrentHealth { get; private set; }
        public int MaxHealth => maxHealth;
        public bool IsDead => CurrentHealth <= 0;

        private void Awake()
        {
            CurrentHealth = maxHealth;
        }

        public void TakeDamage(int amount)
        {
            if (amount <= 0 || IsDead)
            {
                return;
            }

            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
            Changed?.Invoke(CurrentHealth, maxHealth);

            if (CurrentHealth == 0)
            {
                Died?.Invoke();

                if (destroyOnDeath)
                {
                    Destroy(gameObject);
                }
            }
        }

        public void Heal(int amount)
        {
            if (amount <= 0 || IsDead)
            {
                return;
            }

            CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
            Changed?.Invoke(CurrentHealth, maxHealth);
        }

        public void Restore()
        {
            CurrentHealth = maxHealth;
            Changed?.Invoke(CurrentHealth, maxHealth);
        }
    }
}
