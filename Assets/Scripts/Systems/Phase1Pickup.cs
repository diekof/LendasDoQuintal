using LendasDoQuintal.Core;
using LendasDoQuintal.Player;
using UnityEngine;

namespace LendasDoQuintal.Systems
{
    public class Phase1Pickup : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player") || !other.TryGetComponent(out Health health) || health.IsDead) return;
            health.Heal(2);
            if (other.TryGetComponent(out HeroSkillTree tree)) tree.Reward(0, 25f);
            gameObject.SetActive(false);
        }
    }
}
