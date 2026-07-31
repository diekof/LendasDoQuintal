using LendasDoQuintal.Core;
using UnityEngine;

namespace LendasDoQuintal.Systems
{
    public class InteractableObject : MonoBehaviour, IInteractable
    {
        [SerializeField] private string prompt = "Examinar";
        [SerializeField] private string clueId;
        [SerializeField] private string message = "Tem algo estranho aqui.";
        [SerializeField] private ClueSystem clueSystem;

        public string Prompt => prompt;

        public void Interact()
        {
            if (clueSystem != null && !string.IsNullOrWhiteSpace(clueId))
            {
                clueSystem.RegisterClue(clueId);
            }

            Debug.Log(message);
        }

        public void Configure(ClueSystem newClueSystem, string newClueId, string newMessage)
        {
            clueSystem = newClueSystem;
            clueId = newClueId;
            message = newMessage;
        }
    }
}
