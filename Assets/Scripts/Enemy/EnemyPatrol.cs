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
        private int direction = 1;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);

            bool hasGround = groundCheck == null ||
                Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundMask);
            bool hitWall = wallCheck != null &&
                Physics2D.OverlapCircle(wallCheck.position, checkRadius, groundMask);

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

        public void Configure(Transform newGroundCheck, Transform newWallCheck, LayerMask newGroundMask)
        {
            groundCheck = newGroundCheck;
            wallCheck = newWallCheck;
            groundMask = newGroundMask;
        }
    }
}
