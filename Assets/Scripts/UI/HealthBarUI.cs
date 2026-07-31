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
            if (targetHealth != null)
            {
                targetHealth.Changed += UpdateValue;
                UpdateValue(targetHealth.CurrentHealth, targetHealth.MaxHealth);
            }
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
            targetHealth = newTargetHealth;
            slider = newSlider;
        }
    }
}
