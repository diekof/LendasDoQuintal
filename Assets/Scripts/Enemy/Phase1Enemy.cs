using LendasDoQuintal.Core;
using LendasDoQuintal.Player;
using UnityEngine;

namespace LendasDoQuintal.Enemy
{
    public enum Phase1EnemyKind { Toy, Chicken, Shadow, Whirlwind }

    [RequireComponent(typeof(Health), typeof(Rigidbody2D))]
    public class Phase1Enemy : MonoBehaviour
    {
        [SerializeField] private Phase1EnemyKind kind;
        [SerializeField] private Transform target;
        [SerializeField] private int section;
        [SerializeField] private Sprite[] walkFrames;
        [SerializeField] private Sprite[] spitFrames;
        private Rigidbody2D body;
        private Health health;
        private float nextAttack;
        private float stunUntil;
        private Vector3 origin;
        private float tellUntil;
        private bool windingUp;
        private SpriteRenderer visual;
        private int damage = 1;
        private float speedMultiplier = 1f;
        public int Section => section;
        public Phase1EnemyKind Kind => kind;
        public bool Alive => health != null && !health.IsDead;
        public void SetDifficulty(int contactDamage, float speedScale) { damage = contactDamage; speedMultiplier = speedScale; }
        public void ConfigureAnimation(Sprite[] walk, Sprite[] spit) { walkFrames = walk; spitFrames = spit; }

        public void Configure(Phase1EnemyKind type, Transform player, int area)
        { kind = type; target = player; section = area; }

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>(); health = GetComponent<Health>();
            visual = GetComponent<SpriteRenderer>(); origin = transform.position;
            health.Changed += OnDamaged; health.Died += OnDied;
        }

        private void OnDestroy()
        { if (health != null) { health.Changed -= OnDamaged; health.Died -= OnDied; } }
        private void OnDamaged(int current, int maximum) { stunUntil = Time.time + 0.25f; }
        private void OnDied()
        {
            if (target != null && target.TryGetComponent(out HeroSkillTree tree)) tree.Reward(1, 12f);
        }

        private void FixedUpdate()
        {
            if (target == null || !Alive || Time.timeScale <= 0f) return;
            Vector2 delta = target.position - transform.position;
            float left = section * Systems.Phase1Director.SectionWidth + 1f;
            float right = (section + 1) * Systems.Phase1Director.SectionWidth - 1f;
            if (transform.position.x < left || transform.position.x > right)
                body.position = new Vector2(Mathf.Clamp(transform.position.x, left, right), body.position.y);
            float direction = Mathf.Sign(delta.x);
            bool close = Mathf.Abs(delta.x) < 10f;
            if (kind == Phase1EnemyKind.Whirlwind)
            {
                body.linearVelocity = new Vector2(close ? direction * 2.4f * speedMultiplier : 0f,
                    (origin.y + Mathf.Sin(Time.time * 3f) * 0.7f - transform.position.y) * 3f);
                visual.transform.Rotate(0f, 0f, 120f * Time.fixedDeltaTime);
            }
            else
            {
                float speed = (kind == Phase1EnemyKind.Shadow ? 3.4f : 1.5f) * speedMultiplier;
                bool groundAhead = Physics2D.Raycast(transform.position + new Vector3(direction * 0.65f, -0.3f), Vector2.down, 1f, 1 << 0);
                body.linearVelocity = new Vector2(close && groundAhead && Time.time >= stunUntil && !windingUp ? direction * speed : 0f, body.linearVelocity.y);
                if (close) visual.flipX = direction < 0;
            }
            if (kind == Phase1EnemyKind.Chicken && close && Time.time >= nextAttack && !windingUp)
            { windingUp = true; tellUntil = Time.time + 0.6f; visual.color = Color.yellow; }
            if (windingUp && Time.time >= tellUntil)
            {
                windingUp = false; nextAttack = Time.time + 2.8f; visual.color = Color.white;
                GameObject shot = new GameObject("FogoDaGalinha"); shot.transform.position = transform.position + Vector3.up * 0.2f;
                var renderer = shot.AddComponent<SpriteRenderer>(); renderer.sprite = FireProjectile.CreateFireSprite(); renderer.sortingOrder = 15;
                var projectileCollider = shot.AddComponent<CircleCollider2D>(); projectileCollider.isTrigger = true; projectileCollider.radius = 0.18f;
                Physics2D.IgnoreCollision(projectileCollider, GetComponent<Collider2D>());
                var rb = shot.AddComponent<Rigidbody2D>(); rb.bodyType = RigidbodyType2D.Kinematic;
                shot.AddComponent<FireProjectile>().Launch(new Vector2(direction, 0f), 4f * speedMultiplier, damage, 2f);
            }
        }

        private void Update()
        {
            if (Time.timeScale <= 0f || visual == null) return;
            Sprite[] frames = windingUp ? spitFrames : walkFrames;
            if (frames != null && frames.Length > 0) visual.sprite = frames[Mathf.FloorToInt(Time.time * 10f) % frames.Length];
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (Time.time < nextAttack || Time.time < stunUntil || !collision.collider.CompareTag("Player")) return;
            if (collision.collider.TryGetComponent(out Health victim))
            { victim.TakeDamage(damage); victim.MakeInvulnerable(0.75f); nextAttack = Time.time + 1f; }
        }
    }
}
