using UnityEngine;

namespace LendasDoQuintal.Player
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerSpriteAnimator : MonoBehaviour
    {
        [SerializeField] private PlayerPlatformMovement movement;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite idleSprite;
        [SerializeField] private Sprite[] walkSprites;
        [SerializeField] private float walkFrameRate = 10f;

        private float frameTimer;
        private int frameIndex;

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

            bool walking = movement.IsGrounded &&
                Mathf.Abs(movement.HorizontalInput) > 0.05f &&
                walkSprites != null &&
                walkSprites.Length > 0;

            if (!walking)
            {
                frameTimer = 0f;
                frameIndex = 0;
                spriteRenderer.sprite = idleSprite;
                return;
            }

            frameTimer += Time.deltaTime;
            float frameDuration = 1f / Mathf.Max(1f, walkFrameRate);

            if (frameTimer >= frameDuration)
            {
                frameTimer -= frameDuration;
                frameIndex = (frameIndex + 1) % walkSprites.Length;
                spriteRenderer.sprite = walkSprites[frameIndex];
            }
        }

        public void Configure(
            PlayerPlatformMovement newMovement,
            SpriteRenderer newSpriteRenderer,
            Sprite newIdleSprite,
            Sprite[] newWalkSprites)
        {
            movement = newMovement;
            spriteRenderer = newSpriteRenderer;
            idleSprite = newIdleSprite;
            walkSprites = newWalkSprites;
        }
    }
}
