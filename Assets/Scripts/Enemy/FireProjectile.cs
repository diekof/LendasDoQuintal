using LendasDoQuintal.Core;
using LendasDoQuintal.Systems;
using UnityEngine;

namespace LendasDoQuintal.Enemy
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class FireProjectile : MonoBehaviour
    {
        private static Sprite fireSprite;

        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;
        private Sprite[] fireFrames;
        private Sprite[] explosionFrames;
        private float frameTimer;
        private int frameIndex;
        private int damage = 1;
        private float despawnTime;
        private bool launched;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            AnimateFire();

            if (launched && Time.time >= despawnTime)
            {
                Explode();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.isTrigger)
            {
                return;
            }

            if (other.CompareTag("Player") && other.TryGetComponent(out Health health))
            {
                if (health.IsInvulnerable)
                {
                    return;
                }

                health.TakeDamage(damage);
                Explode();
                return;
            }

            Explode();
        }

        public void Configure(Sprite[] newFireFrames, Sprite[] newExplosionFrames)
        {
            fireFrames = newFireFrames;
            explosionFrames = newExplosionFrames;

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (spriteRenderer != null && fireFrames != null && fireFrames.Length > 0)
            {
                spriteRenderer.sprite = fireFrames[0];
            }
        }

        public void Launch(Vector2 direction, float speed, int newDamage, float lifetime)
        {
            if (rb == null)
            {
                rb = GetComponent<Rigidbody2D>();
            }

            damage = Mathf.Max(1, newDamage);
            despawnTime = Time.time + Mathf.Max(0.1f, lifetime);
            launched = true;
            rb.linearVelocity = direction.normalized * speed;
            transform.localScale = new Vector3(Mathf.Sign(direction.x), 1f, 1f);
        }

        private void AnimateFire()
        {
            if (spriteRenderer == null || fireFrames == null || fireFrames.Length == 0)
            {
                return;
            }

            frameTimer += Time.deltaTime;
            if (frameTimer < 1f / 14f)
            {
                return;
            }

            frameTimer = 0f;
            frameIndex = (frameIndex + 1) % fireFrames.Length;
            spriteRenderer.sprite = fireFrames[frameIndex];
        }

        private void Explode()
        {
            if (explosionFrames != null && explosionFrames.Length > 0)
            {
                GameObject explosion = new GameObject("ChickenFireExplosion");
                explosion.transform.position = transform.position;
                SpriteRenderer renderer = explosion.AddComponent<SpriteRenderer>();
                renderer.sortingOrder = 19;

                OneShotSpriteAnimation animation = explosion.AddComponent<OneShotSpriteAnimation>();
                animation.Configure(explosionFrames, 18f);
            }

            Destroy(gameObject);
        }

        public static Sprite CreateFireSprite()
        {
            if (fireSprite != null)
            {
                return fireSprite;
            }

            Texture2D texture = new Texture2D(16, 16, TextureFormat.RGBA32, false);
            Color32[] pixels = new Color32[16 * 16];
            Color32 clear = new Color32(0, 0, 0, 0);
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = clear;
            }

            Color32 ember = new Color32(208, 46, 30, 255);
            Color32 orange = new Color32(255, 126, 24, 255);
            Color32 yellow = new Color32(255, 220, 73, 255);
            Fill(pixels, 3, 6, 10, 5, ember);
            Fill(pixels, 5, 4, 8, 8, orange);
            Fill(pixels, 8, 6, 5, 4, yellow);
            Set(pixels, 2, 8, ember);
            Set(pixels, 4, 5, orange);
            Set(pixels, 12, 7, yellow);
            Set(pixels, 13, 8, yellow);

            texture.SetPixels32(pixels);
            texture.filterMode = FilterMode.Point;
            texture.Apply();
            fireSprite = Sprite.Create(texture, new Rect(0f, 0f, 16f, 16f), new Vector2(0.5f, 0.5f), 16f);
            return fireSprite;
        }

        private static void Fill(Color32[] pixels, int x, int y, int width, int height, Color32 color)
        {
            for (int py = y; py < y + height; py++)
            {
                for (int px = x; px < x + width; px++)
                {
                    Set(pixels, px, py, color);
                }
            }
        }

        private static void Set(Color32[] pixels, int x, int y, Color32 color)
        {
            if (x < 0 || x >= 16 || y < 0 || y >= 16)
            {
                return;
            }

            pixels[y * 16 + x] = color;
        }
    }
}
