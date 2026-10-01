using UnityEngine;

namespace LendasDoQuintal.Core
{
    public static class ProceduralSfx
    {
        private const int SampleRate = 44100;

        public static AudioClip CreateSweep(string name, float startFrequency, float endFrequency, float duration, float volume)
        {
            int samples = Mathf.CeilToInt(SampleRate * duration);
            float[] data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)SampleRate;
                float progress = i / (float)samples;
                float frequency = Mathf.Lerp(startFrequency, endFrequency, progress);
                float envelope = 1f - progress;
                data[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * envelope * volume;
            }

            return CreateClip(name, data);
        }

        public static AudioClip CreateThump(string name, float duration, float volume)
        {
            int samples = Mathf.CeilToInt(SampleRate * duration);
            float[] data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)SampleRate;
                float progress = i / (float)samples;
                float envelope = Mathf.Exp(-progress * 10f);
                float low = Mathf.Sin(2f * Mathf.PI * 92f * t);
                float grit = Mathf.Sin(2f * Mathf.PI * 178f * t) * 0.35f;
                data[i] = (low + grit) * envelope * volume;
            }

            return CreateClip(name, data);
        }

        public static AudioClip CreatePunch(string name, float duration, float volume, bool impact)
        {
            int samples = Mathf.CeilToInt(SampleRate * duration);
            float[] data = new float[samples];
            uint seed = impact ? 0xA341316Cu : 0xC8013EA4u;

            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)SampleRate;
                float progress = i / (float)samples;
                float envelope = Mathf.Exp(-progress * (impact ? 14f : 18f));
                float noise = NextNoise(ref seed);
                float body = Mathf.Sin(2f * Mathf.PI * (impact ? 132f : 260f) * t);
                float snap = Mathf.Sin(2f * Mathf.PI * (impact ? 760f : 520f) * t) * 0.25f;
                data[i] = (noise * 0.45f + body + snap) * envelope * volume;
            }

            return CreateClip(name, data);
        }

        private static AudioClip CreateClip(string name, float[] data)
        {
            AudioClip clip = AudioClip.Create(name, data.Length, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private static float NextNoise(ref uint seed)
        {
            seed = seed * 1664525u + 1013904223u;
            return ((seed >> 8) / 8388607.5f) - 1f;
        }
    }
}
