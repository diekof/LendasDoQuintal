using UnityEngine;

namespace LendasDoQuintal.Systems
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class OneShotSpriteAnimation : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite[] frames;
        [SerializeField] private float frameRate = 18f;

        private float frameTimer;
        private int frameIndex;

        private void Awake()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
        }

        private void Update()
        {
            if (spriteRenderer == null || frames == null || frames.Length == 0)
            {
                Destroy(gameObject);
                return;
            }

            spriteRenderer.sprite = frames[frameIndex];
            frameTimer += Time.deltaTime;

            if (frameTimer < 1f / Mathf.Max(1f, frameRate))
            {
                return;
            }

            frameTimer = 0f;
            frameIndex++;

            if (frameIndex >= frames.Length)
            {
                Destroy(gameObject);
            }
        }

        public void Configure(Sprite[] newFrames, float newFrameRate)
        {
            frames = newFrames;
            frameRate = newFrameRate;
        }
    }
}
