using UnityEngine;
using LendasDoQuintal.Player;

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
            DropPlayer(other.gameObject);
            Invoke(nameof(EndPrototype), endDelay);
        }

        private static void DropPlayer(GameObject player)
        {
            if (player.TryGetComponent(out PlayerPlatformMovement movement))
            {
                movement.enabled = false;
            }

            if (player.TryGetComponent(out PlayerCombat combat))
            {
                combat.enabled = false;
            }

            if (player.TryGetComponent(out PlayerInteraction interaction))
            {
                interaction.enabled = false;
            }

            if (player.TryGetComponent(out Rigidbody2D rb))
            {
                rb.linearVelocity = new Vector2(1.5f, -8f);
                rb.gravityScale = 4.5f;
            }

            foreach (Collider2D collider in player.GetComponents<Collider2D>())
            {
                collider.enabled = false;
            }
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
