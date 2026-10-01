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
        [SerializeField] private Sprite[] fireSprites;
        [SerializeField] private Sprite[] explosionSprites;
        [SerializeField] private float spitTellDuration = 0.28f;
        [SerializeField] private float turnToPlayerRange = 7f;

        private float nextHitTime;
        private float nextFireTime;
        private float spitTimer;
        private EnemyPatrol patrol;

        public bool IsSpitting => spitTimer > 0f;

        private void Awake()
        {
            patrol = GetComponent<EnemyPatrol>();
        }

        private void Update()
        {
            spitTimer = Mathf.Max(0f, spitTimer - Time.deltaTime);

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
            bool playerCloseEnoughToTurn = Mathf.Abs(toPlayer.x) <= turnToPlayerRange && Mathf.Abs(toPlayer.y) <= 2.2f;
            if (playerCloseEnoughToTurn && Mathf.Abs(toPlayer.x) > 0.1f && Mathf.Sign(toPlayer.x) != facing)
            {
                int newFacing = toPlayer.x >= 0f ? 1 : -1;
                patrol?.FaceDirection(newFacing);
                facing = newFacing;
            }

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

        public void SetFireProfile(float newCooldown, float newSpeed, float newRange)
        {
            fireCooldown = Mathf.Max(0.35f, newCooldown);
            fireSpeed = Mathf.Max(1f, newSpeed);
            fireRange = Mathf.Max(1f, newRange);
        }

        public void ConfigureFire(Transform newFirePoint, Sprite[] newFireSprites, Sprite[] newExplosionSprites)
        {
            firePoint = newFirePoint;
            fireSprites = newFireSprites;
            explosionSprites = newExplosionSprites;
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
            spitTimer = spitTellDuration;

            Vector3 origin = firePoint != null
                ? firePoint.position
                : transform.position + new Vector3(0.55f * facing, 0.08f, 0f);

            GameObject fireball = new GameObject("ChickenFireball");
            fireball.transform.position = origin;

            SpriteRenderer renderer = fireball.AddComponent<SpriteRenderer>();
            renderer.sprite = fireSprites != null && fireSprites.Length > 0
                ? fireSprites[0]
                : FireProjectile.CreateFireSprite();
            renderer.sortingOrder = 18;

            CircleCollider2D collider = fireball.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.18f;

            Rigidbody2D rb = fireball.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;

            FireProjectile projectile = fireball.AddComponent<FireProjectile>();
            projectile.Configure(fireSprites, explosionSprites);
            projectile.Launch(new Vector2(facing, 0f), fireSpeed, fireDamage, fireLifetime);
        }
    }
}
