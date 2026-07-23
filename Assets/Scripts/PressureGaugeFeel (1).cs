using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;

// GAME FEEL PASS for the Pressure Gauge (Assignment 16.2).
// This version reads keys through the new Input System (Keyboard.current),
// so it works with Active Input Handling set to "Input System Package".
//
// HOW TO USE:
// 1. Replace the old PressureGaugeFeel.cs in Assets/Scripts/ with this file.
// 2. The PressureGauge object keeps its Pressure Gauge Feel component; no
//    re-adding needed after the script updates.
// 3. Press Play and use the test keys:
//    1 = clarity (10)   2 = manageable (30)   3 = elevated (55)   4 = crisis (85)
//    Space = +12 spike   R = random morning value
//
// WHAT IT ADDS (unchanged from v1):
// - Eased needle sweep with overshoot (Ease.OutBack), so the needle settles
//   like a real barometric instrument instead of snapping.
// - A constant tremble whose strength scales with pressure. Near clarity the
//   needle is almost still. In crisis it visibly shakes.
// - A small punch-scale on the whole gauge when the needle crosses into a
//   new zone, so state changes are felt, not just seen.
// - The needle tints toward crimson as pressure climbs.
public class PressureGaugeFeel : MonoBehaviour
{
    [Header("Wiring (auto-found if empty)")]
    public RectTransform needle;
    public Image needleImage;

    [Header("Needle sweep")]
    public float angleAtZero = 70f;     // needle angle at pressure 0 (clarity side)
    public float angleAtHundred = -70f; // needle angle at pressure 100 (crisis side)
    public float sweepDuration = 0.9f;
    public Ease sweepEase = Ease.OutBack;

    [Header("Tremble")]
    public float trembleAtZero = 0.4f;   // degrees of jitter at pressure 0
    public float trembleAtHundred = 4f;  // degrees of jitter at pressure 100
    public float trembleSpeed = 14f;

    [Header("Zone punch")]
    public float punchStrength = 0.06f;
    public float punchDuration = 0.35f;

    [Header("Needle color by pressure")]
    public Color calmColor = new Color(0.15f, 0.15f, 0.15f);
    public Color crisisColor = new Color(0.55f, 0.08f, 0.08f);

    float displayedPressure;   // the value the needle is currently showing (tweened)
    float targetPressure;
    int currentZone;
    Tween sweepTween;

    static readonly float[] zoneEdges = { 20f, 45f, 70f }; // clarity|manageable|elevated|crisis

    void Awake()
    {
        if (needle == null)
        {
            Transform t = transform.Find("Needle");
            if (t != null) needle = (RectTransform)t;
        }
        if (needle != null && needleImage == null)
            needleImage = needle.GetComponent<Image>();

        displayedPressure = 30f;
        targetPressure = 30f;
        currentZone = ZoneOf(30f);
        ApplyNeedle();
    }

    void Update()
    {
        HandleTestKeys();
        ApplyNeedle();
    }

    // Test keys so the feel pass can be demonstrated without the full game loop.
    void HandleTestKeys()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;   // no keyboard device (e.g. headless), just skip

        if (kb.digit1Key.wasPressedThisFrame) SetPressure(10f);
        if (kb.digit2Key.wasPressedThisFrame) SetPressure(30f);
        if (kb.digit3Key.wasPressedThisFrame) SetPressure(55f);
        if (kb.digit4Key.wasPressedThisFrame) SetPressure(85f);
        if (kb.spaceKey.wasPressedThisFrame) SetPressure(targetPressure + 12f);
        if (kb.rKey.wasPressedThisFrame) SetPressure(Random.Range(5f, 95f));
    }

    public void SetPressure(float value)
    {
        targetPressure = Mathf.Clamp(value, 0f, 100f);

        int newZone = ZoneOf(targetPressure);
        if (newZone != currentZone)
        {
            currentZone = newZone;
            transform.DOKill(true);
            transform.DOPunchScale(Vector3.one * punchStrength, punchDuration, 8, 0.7f);
        }

        sweepTween?.Kill();
        sweepTween = DOTween.To(() => displayedPressure, x => displayedPressure = x,
                                targetPressure, sweepDuration)
                            .SetEase(sweepEase);
    }

    void ApplyNeedle()
    {
        if (needle == null) return;

        float t = displayedPressure / 100f;
        float baseAngle = Mathf.Lerp(angleAtZero, angleAtHundred, t);

        // Perlin-driven tremble, stronger as pressure climbs.
        float amp = Mathf.Lerp(trembleAtZero, trembleAtHundred, t);
        float jitter = (Mathf.PerlinNoise(Time.time * trembleSpeed, 0.37f) - 0.5f) * 2f * amp;

        needle.localEulerAngles = new Vector3(0f, 0f, baseAngle + jitter);

        if (needleImage != null)
            needleImage.color = Color.Lerp(calmColor, crisisColor, t);
    }

    static int ZoneOf(float pressure)
    {
        for (int i = 0; i < zoneEdges.Length; i++)
            if (pressure < zoneEdges[i]) return i;
        return zoneEdges.Length;
    }
}
