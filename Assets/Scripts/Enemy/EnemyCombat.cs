using LendasDoQuintal.Core;
using UnityEngine;

namespace LendasDoQuintal.Enemy
{
    public class EnemyCombat : MonoBehaviour
    {
        [SerializeField] private int contactDamage = 1;
        [SerializeField] private float hitCooldown = 0.75f;

        private float nextHitTime;

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (Time.time < nextHitTime)
            {
                return;
            }

            if (collision.collider.CompareTag("Player") && collision.collider.TryGetComponent(out Health health))
            {
                health.TakeDamage(contactDamage);
                nextHitTime = Time.time + hitCooldown;
            }
        }
    }
}
