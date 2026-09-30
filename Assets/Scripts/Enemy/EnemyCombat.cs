using LendasDoQuintal.Core;
using UnityEngine;

namespace LendasDoQuintal.Enemy
{
    public class EnemyCombat : MonoBehaviour
    {
        [SerializeField] private int contactDamage = 1;
        [SerializeField] private float hitCooldown = 0.75f;
        [Header("Fire Attack")]
        [SerializeField] private bool useFireAttack = true;
        [SerializeField] private float fireRange = 6f;
        [SerializeField] private float fireCooldown = 1.8f;
        [SerializeField] private float fireSpeed = 5.5f;
        [SerializeField] private float fireLifetime = 2.2f;
        [SerializeField] private int fireDamage = 1;
        [SerializeField] private Transform firePoint;

        private float nextHitTime;
        private float nextFireTime;
        private EnemyPatrol patrol;

        private void Awake()
        {
            patrol = GetComponent<EnemyPatrol>();
        }

        private void Update()
        {
            if (!useFireAttack || Time.time < nextFireTime || Time.timeScale <= 0f)
            {
                return;
            }

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                return;
            }

            Vector2 toPlayer = player.transform.position - transform.position;
            int facing = FacingDirection();
            bool playerInFront = Mathf.Sign(toPlayer.x) == facing;
            bool playerInRange = Mathf.Abs(toPlayer.x) <= fireRange && Mathf.Abs(toPlayer.y) <= 1.8f;

            if (playerInFront && playerInRange)
            {
                ShootFire(facing);
            }
        }

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

        public void SetContactDamage(int value)
        {
            contactDamage = Mathf.Max(1, value);
            fireDamage = contactDamage;
        }

        private int FacingDirection()
        {
            if (patrol != null)
            {
                return patrol.Direction >= 0 ? 1 : -1;
            }

            return transform.localScale.x >= 0f ? 1 : -1;
        }

        private void ShootFire(int facing)
        {
            nextFireTime = Time.time + fireCooldown;

            Vector3 origin = firePoint != null
                ? firePoint.position
                : transform.position + new Vector3(0.55f * facing, 0.08f, 0f);

            GameObject fireball = new GameObject("ChickenFireball");
            fireball.transform.position = origin;

            SpriteRenderer renderer = fireball.AddComponent<SpriteRenderer>();
            renderer.sprite = FireProjectile.CreateFireSprite();
            renderer.sortingOrder = 18;

            CircleCollider2D collider = fireball.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.18f;

            Rigidbody2D rb = fireball.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;

            FireProjectile projectile = fireball.AddComponent<FireProjectile>();
            projectile.Launch(new Vector2(facing, 0f), fireSpeed, fireDamage, fireLifetime);
        }
    }
}
