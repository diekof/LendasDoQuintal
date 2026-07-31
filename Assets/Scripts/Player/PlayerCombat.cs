using LendasDoQuintal.Core;
using UnityEngine;

namespace LendasDoQuintal.Player
{
    public class PlayerCombat : MonoBehaviour
    {
        [SerializeField] private Transform attackPoint;
        [SerializeField] private float attackRadius = 0.55f;
        [SerializeField] private int damage = 1;
        [SerializeField] private float cooldown = 0.25f;
        [SerializeField] private LayerMask enemyMask = ~0;

        private float nextAttackTime;

        private void Update()
        {
            if (InputReader.AttackPressed() && Time.time >= nextAttackTime)
            {
                Attack();
            }
        }

        private void Attack()
        {
            nextAttackTime = Time.time + cooldown;

            if (attackPoint == null)
            {
                return;
            }

            Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyMask);
            foreach (Collider2D hit in hits)
            {
                if (hit.gameObject == gameObject)
                {
                    continue;
                }

                if (hit.TryGetComponent(out Health health))
                {
                    health.TakeDamage(damage);
                }
            }
        }

        public void Configure(Transform newAttackPoint, LayerMask newEnemyMask)
        {
            attackPoint = newAttackPoint;
            enemyMask = newEnemyMask;
        }
    }
}
