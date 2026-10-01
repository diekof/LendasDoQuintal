using LendasDoQuintal.Core;
using UnityEngine;

namespace LendasDoQuintal.Player
{
    public class PlayerInteraction : MonoBehaviour
    {
        [SerializeField] private Transform interactionPoint;
        [SerializeField] private float radius = 0.75f;
        [SerializeField] private LayerMask interactableMask = ~0;

        private void Update()
        {
            if (Time.timeScale <= 0f || !InputReader.InteractPressed() || interactionPoint == null)
            {
                return;
            }

            Collider2D[] hits = Physics2D.OverlapCircleAll(interactionPoint.position, radius, interactableMask);
            foreach (Collider2D hit in hits)
            {
                IInteractable interactable = hit.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                    return;
                }
            }
        }

        public void Configure(Transform newInteractionPoint, LayerMask newInteractableMask)
        {
            interactionPoint = newInteractionPoint;
            interactableMask = newInteractableMask;
        }
    }
}
