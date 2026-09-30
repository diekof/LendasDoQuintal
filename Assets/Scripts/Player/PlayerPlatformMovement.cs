using UnityEngine;
using LendasDoQuintal.Core;

namespace LendasDoQuintal.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerPlatformMovement : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float runSpeed = 7f;
        [SerializeField] private float jumpForce = 12f;

        [Header("Ground Check")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundRadius = 0.16f;
        [SerializeField] private LayerMask groundMask = ~0;
        [SerializeField] private float coyoteTime = 0.1f;
        [SerializeField] private float jumpBuffer = 0.1f;
        [SerializeField] private float minimumX = -5.5f;
        [SerializeField] private float fallRecoveryY = -8f;

        private Rigidbody2D rb;
        private Collider2D[] ownColliders;
        private readonly Collider2D[] groundHits = new Collider2D[8];
        private ContactFilter2D groundFilter;
        private float horizontalInput;
        private float coyoteCounter;
        private float jumpBufferCounter;
        private Vector3 spawnPosition;

        public bool IsGrounded { get; private set; }
        public float HorizontalInput => horizontalInput;
        public float VerticalVelocity => rb.linearVelocity.y;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            ownColliders = GetComponents<Collider2D>();
            spawnPosition = transform.position;
        }

        private void Update()
        {
            horizontalInput = InputReader.MoveX();

            IsGrounded = CheckGrounded();

            coyoteCounter = IsGrounded ? coyoteTime : coyoteCounter - Time.deltaTime;
            jumpBufferCounter = InputReader.JumpPressed() ? jumpBuffer : jumpBufferCounter - Time.deltaTime;

            if (jumpBufferCounter > 0f && coyoteCounter > 0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                jumpBufferCounter = 0f;
                coyoteCounter = 0f;
            }
        }

        private void FixedUpdate()
        {
            RecoverIfOutOfBounds();

            float speed = InputReader.RunHeld() ? runSpeed : walkSpeed;

            rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);

            if (Mathf.Abs(horizontalInput) > 0.01f)
            {
                transform.localScale = new Vector3(Mathf.Sign(horizontalInput), 1f, 1f);
            }

            ClampLeftEdge();
        }

        public void Configure(Transform newGroundCheck, LayerMask newGroundMask)
        {
            groundCheck = newGroundCheck;
            groundMask = newGroundMask;
            ConfigureGroundFilter();
        }

        private bool CheckGrounded()
        {
            if (groundCheck == null)
            {
                return false;
            }

            ConfigureGroundFilter();
            int hitCount = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundFilter, groundHits);
            for (int i = 0; i < hitCount; i++)
            {
                Collider2D hit = groundHits[i];
                if (hit != null && !IsOwnCollider(hit) && !hit.isTrigger)
                {
                    return true;
                }
            }

            return false;
        }

        private void ConfigureGroundFilter()
        {
            groundFilter.useLayerMask = true;
            groundFilter.SetLayerMask(groundMask);
            groundFilter.useTriggers = false;
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

        private void ClampLeftEdge()
        {
            if (transform.position.x >= minimumX)
            {
                return;
            }

            transform.position = new Vector3(minimumX, transform.position.y, transform.position.z);
            rb.linearVelocity = new Vector2(Mathf.Max(0f, rb.linearVelocity.x), rb.linearVelocity.y);
        }

        private void RecoverIfOutOfBounds()
        {
            if (transform.position.y > fallRecoveryY)
            {
                return;
            }

            transform.position = spawnPosition;
            rb.linearVelocity = Vector2.zero;
        }
    }
}
