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

        private Rigidbody2D rb;
        private float horizontalInput;
        private float coyoteCounter;
        private float jumpBufferCounter;

        public bool IsGrounded { get; private set; }
        public float HorizontalInput => horizontalInput;
        public float VerticalVelocity => rb.linearVelocity.y;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            horizontalInput = InputReader.MoveX();

            IsGrounded = groundCheck != null &&
                Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundMask);

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
            float speed = InputReader.RunHeld() ? runSpeed : walkSpeed;

            rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);

            if (Mathf.Abs(horizontalInput) > 0.01f)
            {
                transform.localScale = new Vector3(Mathf.Sign(horizontalInput), 1f, 1f);
            }
        }

        public void Configure(Transform newGroundCheck, LayerMask newGroundMask)
        {
            groundCheck = newGroundCheck;
            groundMask = newGroundMask;
        }
    }
}
