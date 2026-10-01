using LendasDoQuintal.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace LendasDoQuintal.UI
{
    public class ObjectiveUI : MonoBehaviour
    {
        [SerializeField] private ObjectiveSystem objectiveSystem;
        [SerializeField] private Text label;

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            if (objectiveSystem != null)
            {
                objectiveSystem.Changed -= UpdateText;
            }
        }

        private void UpdateText(string objective)
        {
            if (label != null)
            {
                label.text = objective;
            }
        }

        public void Configure(ObjectiveSystem newObjectiveSystem, Text newLabel)
        {
            if (objectiveSystem != null)
            {
                objectiveSystem.Changed -= UpdateText;
            }

            objectiveSystem = newObjectiveSystem;
            label = newLabel;
            Subscribe();
        }

        private void Subscribe()
        {
            if (objectiveSystem != null)
            {
                objectiveSystem.Changed -= UpdateText;
                objectiveSystem.Changed += UpdateText;
                UpdateText(objectiveSystem.CurrentObjective);
            }
        }
    }
}
