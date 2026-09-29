using LendasDoQuintal.Core;
using UnityEngine;
using UnityEngine.UI;

namespace LendasDoQuintal.UI
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] private Health targetHealth;
        [SerializeField] private Slider slider;

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
            if (slider == null)
            {
                return;
            }

            slider.maxValue = max;
            slider.value = current;
        }

        public void Configure(Health newTargetHealth, Slider newSlider)
        {
            if (targetHealth != null)
            {
                targetHealth.Changed -= UpdateValue;
            }

            targetHealth = newTargetHealth;
            slider = newSlider;
            Subscribe();
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
