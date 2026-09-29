using UnityEngine;

namespace LendasDoQuintal.Player
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerSpriteAnimator : MonoBehaviour
    {
        [SerializeField] private PlayerPlatformMovement movement;
        [SerializeField] private PlayerCombat combat;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite idleSprite;
        [SerializeField] private Sprite[] walkSprites;
        [SerializeField] private Sprite[] attackSprites;
        [SerializeField] private Sprite[] jumpSprites;
        [SerializeField] private float walkFrameRate = 10f;
        [SerializeField] private float attackFrameRate = 16f;
        [SerializeField] private float jumpFrameRate = 8f;

        private float frameTimer;
        private int frameIndex;
        private AnimationState currentState = AnimationState.Idle;

        private void Awake()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (movement == null)
            {
                movement = GetComponent<PlayerPlatformMovement>();
            }

            if (combat == null)
            {
                combat = GetComponent<PlayerCombat>();
            }

            if (idleSprite == null && spriteRenderer != null)
            {
                idleSprite = spriteRenderer.sprite;
            }
        }

        private void Update()
        {
            if (spriteRenderer == null || movement == null)
            {
                return;
            }

            if (combat != null && combat.IsAttacking && HasFrames(attackSprites))
            {
                Play(AnimationState.Attack, attackSprites, attackFrameRate, false);
                return;
            }

            if (!movement.IsGrounded && HasFrames(jumpSprites))
            {
                Play(AnimationState.Jump, jumpSprites, jumpFrameRate, false);
                return;
            }

            bool walking = movement.IsGrounded &&
                Mathf.Abs(movement.HorizontalInput) > 0.05f &&
                HasFrames(walkSprites);

            if (!walking)
            {
                SetState(AnimationState.Idle);
                spriteRenderer.sprite = idleSprite;
                return;
            }

            Play(AnimationState.Walk, walkSprites, walkFrameRate, true);
        }

        private void Play(AnimationState state, Sprite[] sprites, float frameRate, bool loop)
        {
            SetState(state);

            frameTimer += Time.deltaTime;
            float frameDuration = 1f / Mathf.Max(1f, frameRate);

            if (frameTimer >= frameDuration)
            {
                frameTimer -= frameDuration;
                frameIndex++;

                if (loop)
                {
                    frameIndex %= sprites.Length;
                }
                else
                {
                    frameIndex = Mathf.Min(frameIndex, sprites.Length - 1);
                }
            }

            spriteRenderer.sprite = sprites[frameIndex];
        }

        private void SetState(AnimationState state)
        {
            if (currentState == state)
            {
                return;
            }

            currentState = state;
            frameTimer = 0f;
            frameIndex = 0;
        }

        private static bool HasFrames(Sprite[] sprites)
        {
            return sprites != null && sprites.Length > 0;
        }

        public void Configure(
            PlayerPlatformMovement newMovement,
            PlayerCombat newCombat,
            SpriteRenderer newSpriteRenderer,
            Sprite newIdleSprite,
            Sprite[] newWalkSprites,
            Sprite[] newAttackSprites,
            Sprite[] newJumpSprites)
        {
            movement = newMovement;
            combat = newCombat;
            spriteRenderer = newSpriteRenderer;
            idleSprite = newIdleSprite;
            walkSprites = newWalkSprites;
            attackSprites = newAttackSprites;
            jumpSprites = newJumpSprites;
        }

        private enum AnimationState
        {
            Idle,
            Walk,
            Attack,
            Jump
        }
    }
}
