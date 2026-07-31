using System;
using System.Collections.Generic;
using UnityEngine;

namespace LendasDoQuintal.Systems
{
    public class ClueSystem : MonoBehaviour
    {
        public event Action<int> Changed;

        private readonly HashSet<string> foundClues = new HashSet<string>();

        public int Count => foundClues.Count;

        public bool RegisterClue(string clueId)
        {
            if (string.IsNullOrWhiteSpace(clueId) || !foundClues.Add(clueId))
            {
                return false;
            }

            Changed?.Invoke(foundClues.Count);
            return true;
        }
    }
}
