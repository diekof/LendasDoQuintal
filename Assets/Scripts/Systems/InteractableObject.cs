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
        [SerializeField] private ClueMessageSystem messageSystem;

        public string Prompt => prompt;

        public void Interact()
        {
            if (clueSystem != null && !string.IsNullOrWhiteSpace(clueId))
            {
                clueSystem.RegisterClue(clueId);
            }

            if (messageSystem != null)
            {
                messageSystem.Show(message);
            }

            Debug.Log(message);
        }

        public void Configure(ClueSystem newClueSystem, string newClueId, string newMessage)
        {
            clueSystem = newClueSystem;
            clueId = newClueId;
            message = newMessage;
        }

        public void Configure(ClueSystem newClueSystem, ClueMessageSystem newMessageSystem, string newClueId, string newMessage)
        {
            Configure(newClueSystem, newClueId, newMessage);
            messageSystem = newMessageSystem;
        }
    }
}
