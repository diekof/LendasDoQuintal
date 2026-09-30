using UnityEngine;

namespace LendasDoQuintal.Enemy
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class EnemySpriteAnimator : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private EnemyPatrol patrol;
        [SerializeField] private EnemyCombat combat;
        [SerializeField] private Sprite[] idleSprites;
        [SerializeField] private Sprite[] walkSprites;
        [SerializeField] private Sprite[] spitSprites;
        [SerializeField] private float idleFrameRate = 6f;
        [SerializeField] private float walkFrameRate = 8f;
        [SerializeField] private float spitFrameRate = 14f;

        private float frameTimer;
        private int frameIndex;
        private AnimationState currentState = AnimationState.Idle;

        private void Awake()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (patrol == null)
            {
                patrol = GetComponent<EnemyPatrol>();
            }

            if (combat == null)
            {
                combat = GetComponent<EnemyCombat>();
            }
        }

        private void Update()
        {
            if (spriteRenderer == null)
            {
                return;
            }

            if (combat != null && combat.IsSpitting && HasFrames(spitSprites))
            {
                Play(AnimationState.Spit, spitSprites, spitFrameRate);
                return;
            }

            if (patrol != null && patrol.Speed > 0.01f && HasFrames(walkSprites))
            {
                Play(AnimationState.Walk, walkSprites, walkFrameRate);
                return;
            }

            if (HasFrames(idleSprites))
            {
                Play(AnimationState.Idle, idleSprites, idleFrameRate);
            }
        }

        public void Configure(
            EnemyPatrol newPatrol,
            EnemyCombat newCombat,
            SpriteRenderer newSpriteRenderer,
            Sprite[] newIdleSprites,
            Sprite[] newWalkSprites,
            Sprite[] newSpitSprites)
        {
            patrol = newPatrol;
            combat = newCombat;
            spriteRenderer = newSpriteRenderer;
            idleSprites = newIdleSprites;
            walkSprites = newWalkSprites;
            spitSprites = newSpitSprites;
        }

        private void Play(AnimationState state, Sprite[] sprites, float frameRate)
        {
            SetState(state);

            frameTimer += Time.deltaTime;
            float frameDuration = 1f / Mathf.Max(1f, frameRate);
            if (frameTimer >= frameDuration)
            {
                frameTimer -= frameDuration;
                frameIndex = (frameIndex + 1) % sprites.Length;
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

        private enum AnimationState
        {
            Idle,
            Walk,
            Spit
        }
    }
}
