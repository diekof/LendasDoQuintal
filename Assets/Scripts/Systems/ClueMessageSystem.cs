using System;
using UnityEngine;

namespace LendasDoQuintal.Systems
{
    public class ClueMessageSystem : MonoBehaviour
    {
        public event Action<string> Changed;

        public string CurrentMessage { get; private set; }

        public void Show(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            CurrentMessage = message;
            Changed?.Invoke(CurrentMessage);
        }
    }
}
