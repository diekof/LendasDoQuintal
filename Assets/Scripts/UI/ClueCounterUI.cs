using LendasDoQuintal.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace LendasDoQuintal.UI
{
    public class ClueCounterUI : MonoBehaviour
    {
        [SerializeField] private ClueSystem clueSystem;
        [SerializeField] private Text label;

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            if (clueSystem != null)
            {
                clueSystem.Changed -= UpdateText;
            }
        }

        private void UpdateText(int count)
        {
            if (label != null)
            {
                label.text = $"Pistas: {count}";
            }
        }

        public void Configure(ClueSystem newClueSystem, Text newLabel)
        {
            if (clueSystem != null)
            {
                clueSystem.Changed -= UpdateText;
            }

            clueSystem = newClueSystem;
            label = newLabel;
            Subscribe();
        }

        private void Subscribe()
        {
            if (clueSystem != null)
            {
                clueSystem.Changed -= UpdateText;
                clueSystem.Changed += UpdateText;
                UpdateText(clueSystem.Count);
            }
        }
    }
}
