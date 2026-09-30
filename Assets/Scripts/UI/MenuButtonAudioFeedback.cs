using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LendasDoQuintal.UI
{
    [RequireComponent(typeof(Button))]
    public class MenuButtonAudioFeedback : MonoBehaviour, IPointerEnterHandler, ISelectHandler, IPointerDownHandler
    {
        private static AudioClip hoverClip;
        private static AudioClip pressClip;

        private AudioSource audioSource;
        private float nextHoverTime;

        private void Awake()
        {
            audioSource = GetComponentInParent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            audioSource.playOnAwake = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            PlayHover();
        }

        public void OnSelect(BaseEventData eventData)
        {
            PlayHover();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Play(PressClip(), 0.75f);
        }

        private void PlayHover()
        {
            if (Time.unscaledTime < nextHoverTime)
            {
                return;
            }

            nextHoverTime = Time.unscaledTime + 0.06f;
            Play(HoverClip(), 0.45f);
        }

        private void Play(AudioClip clip, float volume)
        {
            if (audioSource != null && clip != null)
            {
                audioSource.PlayOneShot(clip, volume);
            }
        }

        private static AudioClip HoverClip()
        {
            return hoverClip != null ? hoverClip : hoverClip = CreateTone("MenuHover", 720f, 0.045f);
        }

        private static AudioClip PressClip()
        {
            return pressClip != null ? pressClip : pressClip = CreateTone("MenuPress", 420f, 0.075f);
        }

        private static AudioClip CreateTone(string name, float frequency, float duration)
        {
            const int sampleRate = 44100;
            int samples = Mathf.CeilToInt(sampleRate * duration);
            float[] data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)sampleRate;
                float envelope = 1f - (i / (float)samples);
                data[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * envelope * 0.18f;
            }

            AudioClip clip = AudioClip.Create(name, samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }
    }
}
