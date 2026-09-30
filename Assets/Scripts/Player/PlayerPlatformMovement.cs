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
        [SerializeField] private float rollSpeed = 7.4f;
        [SerializeField] private float rollDuration = 0.44f;
        [SerializeField] private float rollCooldown = 0.65f;

        [Header("Ground Check")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundRadius = 0.16f;
        [SerializeField] private LayerMask groundMask = ~0;
        [SerializeField] private float coyoteTime = 0.1f;
        [SerializeField] private float jumpBuffer = 0.1f;
        [SerializeField] private float minimumX = -5.5f;
        [SerializeField] private float fallRecoveryY = -8f;
        [SerializeField, Range(0f, 1f)] private float jumpVolume = 0.48f;
        [SerializeField, Range(0f, 1f)] private float landingVolume = 0.42f;

        private Rigidbody2D rb;
        private AudioSource audioSource;
        private Collider2D[] ownColliders;
        private readonly Collider2D[] groundHits = new Collider2D[8];
        private ContactFilter2D groundFilter;
        private float horizontalInput;
        private float coyoteCounter;
        private float jumpBufferCounter;
        private float rollTimer;
        private float rollCooldownTimer;
        private float facingDirection = 1f;
        private float rollDirection = 1f;
        private Vector3 spawnPosition;
        private bool hasLeftGround;
        private static AudioClip jumpClip;
        private static AudioClip landingClip;

        public bool IsGrounded { get; private set; }
        public bool IsRolling => rollTimer > 0f;
        public float HorizontalInput => horizontalInput;
        public float VerticalVelocity => rb.linearVelocity.y;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
            ownColliders = GetComponents<Collider2D>();
            spawnPosition = transform.position;
        }

        private void Update()
        {
            horizontalInput = InputReader.MoveX();
            rollCooldownTimer = Mathf.Max(0f, rollCooldownTimer - Time.deltaTime);

            bool wasGrounded = IsGrounded;
            float verticalVelocityBeforeGroundCheck = rb.linearVelocity.y;
            IsGrounded = CheckGrounded();

            if (!IsGrounded)
            {
                hasLeftGround = true;
            }
            else if (!wasGrounded && hasLeftGround)
            {
                PlayLanding(verticalVelocityBeforeGroundCheck);
                hasLeftGround = false;
            }

            coyoteCounter = IsGrounded ? coyoteTime : coyoteCounter - Time.deltaTime;
            jumpBufferCounter = InputReader.JumpPressed() ? jumpBuffer : jumpBufferCounter - Time.deltaTime;

            if (IsRolling)
            {
                rollTimer = Mathf.Max(0f, rollTimer - Time.deltaTime);
                jumpBufferCounter = 0f;
                return;
            }

            if (InputReader.RollPressed() && IsGrounded && rollCooldownTimer <= 0f)
            {
                StartRoll();
                return;
            }

            if (jumpBufferCounter > 0f && coyoteCounter > 0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                PlayJump();
                hasLeftGround = true;
                jumpBufferCounter = 0f;
                coyoteCounter = 0f;
            }
        }

        private void FixedUpdate()
        {
            RecoverIfOutOfBounds();

            float speed = InputReader.RunHeld() ? runSpeed : walkSpeed;

            if (IsRolling)
            {
                rb.linearVelocity = new Vector2(rollDirection * rollSpeed, rb.linearVelocity.y);
                transform.localScale = new Vector3(rollDirection, 1f, 1f);
                ClampLeftEdge();
                return;
            }

            rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);

            if (Mathf.Abs(horizontalInput) > 0.01f)
            {
                facingDirection = Mathf.Sign(horizontalInput);
                transform.localScale = new Vector3(facingDirection, 1f, 1f);
            }

            ClampLeftEdge();
        }

        public void Configure(Transform newGroundCheck, LayerMask newGroundMask)
        {
            groundCheck = newGroundCheck;
            groundMask = newGroundMask;
            ConfigureGroundFilter();
        }

        private void StartRoll()
        {
            rollDirection = Mathf.Abs(horizontalInput) > 0.01f
                ? Mathf.Sign(horizontalInput)
                : facingDirection;
            rollTimer = rollDuration;
            rollCooldownTimer = rollCooldown;
            jumpBufferCounter = 0f;

            if (TryGetComponent(out Health health))
            {
                health.MakeInvulnerable(rollDuration);
            }
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

        private void PlayJump()
        {
            PlayOneShot(JumpClip(), jumpVolume);
        }

        private void PlayLanding(float landingVelocity)
        {
            float intensity = Mathf.InverseLerp(0f, -10f, landingVelocity);
            PlayOneShot(LandingClip(), landingVolume * Mathf.Lerp(0.55f, 1f, intensity));
        }

        private void PlayOneShot(AudioClip clip, float volume)
        {
            if (audioSource != null && clip != null && volume > 0f)
            {
                audioSource.PlayOneShot(clip, volume);
            }
        }

        private static AudioClip JumpClip()
        {
            return jumpClip != null
                ? jumpClip
                : jumpClip = ProceduralSfx.CreateSweep("HeroJump", 420f, 780f, 0.12f, 0.32f);
        }

        private static AudioClip LandingClip()
        {
            return landingClip != null
                ? landingClip
                : landingClip = ProceduralSfx.CreateThump("HeroLanding", 0.14f, 0.36f);
        }
    }
}
