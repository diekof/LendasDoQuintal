using UnityEngine;

namespace LendasDoQuintal.Systems
{
    public class SaciEncounter : MonoBehaviour
    {
        [SerializeField] private ObjectiveSystem objectiveSystem;
        [SerializeField] private GameFlowController gameFlow;
        [SerializeField] private GameObject saciVisual;
        [SerializeField] private float endDelay = 2.5f;

        private bool triggered;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (triggered || !other.CompareTag("Player"))
            {
                return;
            }

            triggered = true;

            if (saciVisual != null)
            {
                saciVisual.SetActive(true);
            }

            objectiveSystem?.SetSaciObjective();
            Invoke(nameof(EndPrototype), endDelay);
        }

        private void EndPrototype()
        {
            gameFlow?.EndPrototype();
        }

        public void Configure(ObjectiveSystem newObjectiveSystem, GameFlowController newGameFlow, GameObject newSaciVisual)
        {
            objectiveSystem = newObjectiveSystem;
            gameFlow = newGameFlow;
            saciVisual = newSaciVisual;
        }
    }
}
