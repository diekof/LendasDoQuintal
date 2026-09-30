using LendasDoQuintal.Core;
using UnityEngine;
using UnityEngine.UI;

namespace LendasDoQuintal.UI
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] private Health targetHealth;
        [SerializeField] private Image[] healthOrbs;
        [SerializeField] private Image specialFill;
        [SerializeField] private Color fullOrbColor = new Color32(212, 30, 48, 255);
        [SerializeField] private Color emptyOrbColor = new Color32(64, 24, 36, 180);

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            if (targetHealth != null)
            {
                targetHealth.Changed -= UpdateValue;
            }
        }

        private void UpdateValue(int current, int max)
        {
            if (healthOrbs == null)
            {
                return;
            }

            for (int i = 0; i < healthOrbs.Length; i++)
            {
                Image orb = healthOrbs[i];
                if (orb == null)
                {
                    continue;
                }

                GameObject slot = orb.transform.parent != null ? orb.transform.parent.gameObject : orb.gameObject;
                slot.SetActive(i < max);
                orb.color = i < current ? fullOrbColor : emptyOrbColor;
            }
        }

        public void Configure(Health newTargetHealth, Image[] newHealthOrbs, Image newSpecialFill)
        {
            if (targetHealth != null)
            {
                targetHealth.Changed -= UpdateValue;
            }

            targetHealth = newTargetHealth;
            healthOrbs = newHealthOrbs;
            specialFill = newSpecialFill;
            SetSpecialValue(0f);
            Subscribe();
        }

        public void SetSpecialValue(float normalizedValue)
        {
            if (specialFill != null)
            {
                specialFill.fillAmount = Mathf.Clamp01(normalizedValue);
            }
        }

        private void Subscribe()
        {
            if (targetHealth != null)
            {
                targetHealth.Changed -= UpdateValue;
                targetHealth.Changed += UpdateValue;
                UpdateValue(targetHealth.CurrentHealth, targetHealth.MaxHealth);
            }
        }
    }
}
