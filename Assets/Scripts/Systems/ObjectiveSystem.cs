using System;
using UnityEngine;

namespace LendasDoQuintal.Systems
{
    public class ObjectiveSystem : MonoBehaviour
    {
        [SerializeField] private ClueSystem clueSystem;
        [SerializeField] private string initialObjective = "Descubra o que aconteceu.";
        [SerializeField] private string firstClueObjective = "Vá até o quintal.";
        [SerializeField] private string saciObjective = "Siga a pista deixada pelo Saci.";

        public event Action<string> Changed;
        public string CurrentObjective { get; private set; }

        private void Awake()
        {
            CurrentObjective = initialObjective;
        }

        private void OnEnable()
        {
            if (clueSystem != null)
            {
                clueSystem.Changed += OnCluesChanged;
            }
        }

        private void Start()
        {
            Changed?.Invoke(CurrentObjective);
        }

        private void OnDisable()
        {
            if (clueSystem != null)
            {
                clueSystem.Changed -= OnCluesChanged;
            }
        }

        private void OnCluesChanged(int count)
        {
            if (count >= 1)
            {
                SetObjective(firstClueObjective);
            }
        }

        public void SetSaciObjective()
        {
            SetObjective(saciObjective);
        }

        public void Configure(ClueSystem newClueSystem)
        {
            clueSystem = newClueSystem;
        }

        public void SetObjective(string objective)
        {
            if (string.IsNullOrWhiteSpace(objective) || objective == CurrentObjective)
            {
                return;
            }

            CurrentObjective = objective;
            Changed?.Invoke(CurrentObjective);
        }
    }
}
