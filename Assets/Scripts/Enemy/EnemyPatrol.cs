using UnityEngine;

namespace LendasDoQuintal.Enemy
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyPatrol : MonoBehaviour
    {
        [SerializeField] private float speed = 2f;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private Transform wallCheck;
        [SerializeField] private float checkRadius = 0.12f;
        [SerializeField] private LayerMask groundMask = ~0;

        private Rigidbody2D rb;
        private Collider2D[] ownColliders;
        private readonly Collider2D[] checkHits = new Collider2D[8];
        private ContactFilter2D groundFilter;
        private int direction = 1;

        public int Direction => direction;
        public float Speed => speed;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            ownColliders = GetComponents<Collider2D>();
            transform.localScale = new Vector3(Mathf.Sign(transform.localScale.x == 0f ? 1f : transform.localScale.x), 1f, 1f);
            ConfigureGroundFilter();
        }

        private void FixedUpdate()
        {
            rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);

            bool hasGround = groundCheck == null || HasBlockingHit(groundCheck.position);
            bool hitWall = wallCheck != null && HasBlockingHit(wallCheck.position);

            if (!hasGround || hitWall)
            {
                Flip();
            }
        }

        private void Flip()
        {
            direction *= -1;
            transform.localScale = new Vector3(direction, 1f, 1f);
        }

        private bool HasBlockingHit(Vector3 position)
        {
            ConfigureGroundFilter();
            int hitCount = Physics2D.OverlapCircle(position, checkRadius, groundFilter, checkHits);
            for (int i = 0; i < hitCount; i++)
            {
                Collider2D hit = checkHits[i];
                if (hit != null && !hit.isTrigger && !IsOwnCollider(hit))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsOwnCollider(Collider2D hit)
        {
            if (ownColliders == null)
            {
                return false;
            }

            foreach (Collider2D ownCollider in ownColliders)
            {
                if (hit == ownCollider)
                {
                    return true;
                }
            }

            return false;
        }

        public void Configure(Transform newGroundCheck, Transform newWallCheck, LayerMask newGroundMask)
        {
            groundCheck = newGroundCheck;
            wallCheck = newWallCheck;
            groundMask = newGroundMask;
            ConfigureGroundFilter();
        }

        public void SetSpeed(float value)
        {
            speed = Mathf.Max(0f, value);
        }

        private void ConfigureGroundFilter()
        {
            groundFilter.useLayerMask = true;
            groundFilter.SetLayerMask(groundMask);
            groundFilter.useTriggers = false;
        }
    }
}
