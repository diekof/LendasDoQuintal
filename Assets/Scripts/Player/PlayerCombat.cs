using LendasDoQuintal.Core;
using LendasDoQuintal.Systems;
using UnityEngine;

namespace LendasDoQuintal.Player
{
    public class PlayerCombat : MonoBehaviour
    {
        [SerializeField] private Transform attackPoint;
        [SerializeField] private float attackRadius = 0.55f;
        [SerializeField] private int damage = 1;
        [SerializeField] private float cooldown = 0.25f;
        [SerializeField] private LayerMask enemyMask = ~0;
        [SerializeField] private Sprite[] impactSprites;
        [SerializeField] private float impactFrameRate = 18f;
        [SerializeField, Range(0f, 1f)] private float emptyPunchVolume = 0.38f;
        [SerializeField, Range(0f, 1f)] private float hitPunchVolume = 0.58f;

        private AudioSource audioSource;
        private float nextAttackTime;
        private float attackAnimationTimer;
        private static AudioClip emptyPunchClip;
        private static AudioClip hitPunchClip;

        public bool IsAttacking => attackAnimationTimer > 0f;
        public int DamageBonus { get; set; }
        public bool ItemStolen { get; set; }

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
        }

        private void Update()
        {
            attackAnimationTimer = Mathf.Max(0f, attackAnimationTimer - Time.deltaTime);

            if (Time.timeScale > 0f && !ItemStolen && InputReader.AttackPressed() && Time.time >= nextAttackTime)
            {
                Attack();
            }
        }

        private void Attack()
        {
            nextAttackTime = Time.time + cooldown;
            attackAnimationTimer = cooldown;

            if (attackPoint == null)
            {
                PlayEmptyPunch();
                return;
            }

            bool hitEnemy = false;
            Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyMask);
            foreach (Collider2D hit in hits)
            {
                if (hit.gameObject == gameObject)
                {
                    continue;
                }

                if (hit.TryGetComponent(out Health health))
                {
                    health.TakeDamage(damage + DamageBonus);
                    SpawnImpact(hit.transform.position);
                    hitEnemy = true;
                }
            }

            if (hitEnemy)
            {
                PlayHitPunch();
            }
            else
            {
                PlayEmptyPunch();
            }
        }

        private void SpawnImpact(Vector3 hitPosition)
        {
            if (impactSprites == null || impactSprites.Length == 0)
            {
                return;
            }

            Vector3 direction = transform.localScale.x >= 0f ? Vector3.right : Vector3.left;
            GameObject impact = new GameObject("PunchImpact");
            impact.transform.position = hitPosition + direction * 0.45f + Vector3.up * 0.15f;
            impact.transform.localScale = new Vector3(Mathf.Sign(transform.localScale.x), 1f, 1f);

            SpriteRenderer renderer = impact.AddComponent<SpriteRenderer>();
            renderer.sortingOrder = 20;

            OneShotSpriteAnimation animation = impact.AddComponent<OneShotSpriteAnimation>();
            animation.Configure(impactSprites, impactFrameRate);
        }

        public void Configure(Transform newAttackPoint, LayerMask newEnemyMask)
        {
            attackPoint = newAttackPoint;
            enemyMask = newEnemyMask;
        }

        public void Configure(Transform newAttackPoint, LayerMask newEnemyMask, Sprite[] newImpactSprites)
        {
            Configure(newAttackPoint, newEnemyMask);
            impactSprites = newImpactSprites;
        }

        private void PlayEmptyPunch()
        {
            PlayOneShot(EmptyPunchClip(), emptyPunchVolume);
        }

        private void PlayHitPunch()
        {
            PlayOneShot(HitPunchClip(), hitPunchVolume);
        }

        private void PlayOneShot(AudioClip clip, float volume)
        {
            if (audioSource != null && clip != null && volume > 0f)
            {
                audioSource.PlayOneShot(clip, volume);
            }
        }

        private static AudioClip EmptyPunchClip()
        {
            return emptyPunchClip != null
                ? emptyPunchClip
                : emptyPunchClip = ProceduralSfx.CreatePunch("HeroPunchEmpty", 0.09f, 0.28f, impact: false);
        }

        private static AudioClip HitPunchClip()
        {
            return hitPunchClip != null
                ? hitPunchClip
                : hitPunchClip = ProceduralSfx.CreatePunch("HeroPunchHit", 0.12f, 0.42f, impact: true);
        }
    }
}
