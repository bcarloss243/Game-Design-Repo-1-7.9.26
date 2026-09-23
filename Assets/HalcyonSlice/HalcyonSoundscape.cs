using UnityEngine;

namespace Halcyon.FirstWeather
{
    // Original synthesized material; no third-party music recordings.
    public sealed class HalcyonSoundscape : MonoBehaviour
    {
        AudioSource drone, staticLayer, signal, harvest;
        float harvestTime;
        void Awake()
        {
            drone = Layer("The low note", 0); staticLayer = Layer("Pressure static", 1);
            signal = Layer("The separate note", 2); harvest = Layer("Harvest relay", 3);
        }
        AudioSource Layer(string label, int kind)
        {
            const int rate = 22050, length = rate * 8;
            var samples = new float[length]; var rng = new System.Random(330 + kind); float filtered = 0;
            for (int i = 0; i < length; i++)
            {
                float t = i / (float)rate;
                filtered = filtered * .91f + ((float)rng.NextDouble() * 2 - 1) * .09f;
                samples[i] = kind == 0 ? Mathf.Sin(2 * Mathf.PI * 73 * t) * .28f + Mathf.Sin(2 * Mathf.PI * 74 * t) * .17f :
                    kind == 1 ? filtered * .75f : kind == 2 ? (Mathf.Sin(2 * Mathf.PI * 220 * t) + Mathf.Sin(2 * Mathf.PI * 330 * t)) * .12f :
                    (Mathf.Sin(2 * Mathf.PI * 98 * t) + filtered) * .2f;
            }
            var a = gameObject.AddComponent<AudioSource>();
            a.clip = AudioClip.Create(label, length, 1, rate, false); a.clip.SetData(samples, 0);
            a.loop = true; a.volume = 0; a.Play(); return a;
        }
        public void Harvest() { harvestTime = 2.2f; }
        public void Mix(SliceState state, bool muted, bool paused)
        {
            float dt = Mathf.Min(Time.unscaledDeltaTime, .05f);
            float master = muted || paused ? 0 : 1;
            float hush = state.WithLola ? .58f : 1;
            float p = state.pressure / 100f;
            drone.volume = Mathf.MoveTowards(drone.volume, master * hush * (.025f + p * .12f) * (state.medicated ? .5f : 1), dt * .15f);
            staticLayer.volume = Mathf.MoveTowards(staticLayer.volume, master * hush * Mathf.InverseLerp(40, 85, state.pressure) * .22f, dt * .17f);
            signal.volume = Mathf.MoveTowards(signal.volume, master * (!state.medicated && state.tolerance > 0 ? .026f + (1-p) * .022f : 0), dt * .1f);
            if (!paused) harvestTime = Mathf.Max(0, harvestTime - dt);
            harvest.volume = master * Mathf.Clamp01(harvestTime / 2.2f) * .045f;
        }
        void OnDestroy()
        {
            foreach (var a in new[] { drone, staticLayer, signal, harvest }) if (a != null && a.clip != null) Destroy(a.clip);
        }
    }
}
