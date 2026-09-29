using LendasDoQuintal.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace LendasDoQuintal.UI
{
    public class ClueMessageUI : MonoBehaviour
    {
        [SerializeField] private ClueMessageSystem messageSystem;
        [SerializeField] private GameObject panel;
        [SerializeField] private Text label;
        [SerializeField] private float visibleDuration = 4f;

        private float hideAt;

        private void OnEnable()
        {
            Subscribe();
        }

        private void Update()
        {
            if (panel != null && panel.activeSelf && Time.unscaledTime >= hideAt)
            {
                panel.SetActive(false);
            }
        }

        private void OnDisable()
        {
            if (messageSystem != null)
            {
                messageSystem.Changed -= ShowMessage;
            }
        }

        public void Configure(ClueMessageSystem newMessageSystem, GameObject newPanel, Text newLabel)
        {
            if (messageSystem != null)
            {
                messageSystem.Changed -= ShowMessage;
            }

            messageSystem = newMessageSystem;
            panel = newPanel;
            label = newLabel;
            Subscribe();

            if (panel != null)
            {
                panel.SetActive(false);
            }
        }

        private void Subscribe()
        {
            if (messageSystem != null)
            {
                messageSystem.Changed -= ShowMessage;
                messageSystem.Changed += ShowMessage;
            }
        }

        private void ShowMessage(string message)
        {
            if (label != null)
            {
                label.text = message;
            }

            if (panel != null)
            {
                panel.SetActive(true);
            }

            hideAt = Time.unscaledTime + visibleDuration;
        }
    }
}
