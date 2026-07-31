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
            if (clueSystem != null)
            {
                clueSystem.Changed += UpdateText;
                UpdateText(clueSystem.Count);
            }
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
            clueSystem = newClueSystem;
            label = newLabel;
        }
    }
}
