using UnityEngine;

namespace LendasDoQuintal.Systems
{
    public class GameplayMusicProximity : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private Transform target;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip musicClip;
        [SerializeField] private string musicResourcePath = "Audio/Sombrio HorizonteGamePLay";
        [SerializeField] private float farDistance = 26f;
        [SerializeField] private float nearDistance = 4f;
        [SerializeField, Range(0f, 1f)] private float minimumVolume = 0.08f;
        [SerializeField, Range(0f, 1f)] private float maximumVolume = 0.55f;
        [SerializeField] private float fadeSpeed = 1.6f;

        private void Awake()
        {
            EnsureAudioSource();
            EnsureMusicClip();
            ApplyVolume(immediate: true);
        }

        private void OnEnable()
        {
            if (player == null || target == null)
            {
                return;
            }

            EnsureAudioSource();
            EnsureMusicClip();

            if (audioSource == null || musicClip == null)
            {
                return;
            }

            audioSource.clip = musicClip;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
            audioSource.ignoreListenerPause = false;
            ApplyVolume(immediate: true);

            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }

        private void Update()
        {
            ApplyVolume(immediate: false);
        }

        private void OnDisable()
        {
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }

        public void Configure(Transform newPlayer, Transform newTarget)
        {
            player = newPlayer;
            target = newTarget;
        }

        private void EnsureAudioSource()
        {
            if (audioSource == null)
            {
                AudioSource[] sources = GetComponents<AudioSource>();
                foreach (AudioSource source in sources)
                {
                    if (source != null && source != audioSource && source.clip == musicClip)
                    {
                        audioSource = source;
                        break;
                    }
                }
            }

            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        private void EnsureMusicClip()
        {
            if (musicClip == null)
            {
                musicClip = Resources.Load<AudioClip>(musicResourcePath);
            }
        }

        private void ApplyVolume(bool immediate)
        {
            if (audioSource == null)
            {
                return;
            }

            float targetVolume = CalculateTargetVolume();
            audioSource.volume = immediate
                ? targetVolume
                : Mathf.MoveTowards(audioSource.volume, targetVolume, fadeSpeed * Time.unscaledDeltaTime);
        }

        private float CalculateTargetVolume()
        {
            if (player == null || target == null)
            {
                return minimumVolume;
            }

            float distance = Vector2.Distance(player.position, target.position);
            float t = Mathf.InverseLerp(farDistance, nearDistance, distance);
            return Mathf.Lerp(minimumVolume, maximumVolume, t);
        }
    }
}
