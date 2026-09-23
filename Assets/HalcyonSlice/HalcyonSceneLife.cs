using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Halcyon.FirstWeather
{
    public sealed class HalcyonSceneLife : MonoBehaviour
    {
        readonly List<RectTransform> motes = new List<RectTransform>();
        readonly List<Vector2> origins = new List<Vector2>();
        readonly List<Graphic> lamps = new List<Graphic>();
        RectTransform boat;
        Func<bool> paused, quiet;
        string place;
        float time;
        public void Initialize(string scene, Func<bool> pause, Func<bool> reduced)
        {
            place = scene; paused = pause; quiet = reduced;
            for (int i = 0; i < 15; i++)
            {
                float x = scene == "map" ? 730 + i % 4 * 58 : 170 + (i * 173) % 1180;
                float y = scene == "map" ? 515 + i * 21 : 125 + (i * 97) % 385;
                var g = Shape("Painted light mote", x, y, scene == "map" ? 17 : 3, scene == "map" ? 1.2f : 3, new Color(1, .79f, .36f, .42f));
                motes.Add(g.rectTransform); origins.Add(new Vector2(x, -y));
            }
            Vector2[] lights = scene == "dorm" ? new[] { new Vector2(156, 251), new Vector2(1394, 277), new Vector2(771, 73) } :
                scene == "greenhouse" ? new[] { new Vector2(975, 77), new Vector2(1152, 128), new Vector2(981, 415) } :
                scene == "academy" ? new[] { new Vector2(146, 60), new Vector2(560, 279), new Vector2(1128, 247) } :
                new[] { new Vector2(302, 238), new Vector2(796, 341), new Vector2(1270, 429) };
            foreach (var p in lights) lamps.Add(Shape("Lantern warmth", p.x - 25, p.y - 32, 50, 64, new Color(1, .75f, .31f, .09f)));
            if (scene == "map")
            {
                boat = Shape("Canal boat", 850, 702, 26, 10, new Color32(50, 47, 73, 255)).rectTransform;
                var cabin = Shape("Boat lantern", 0, 0, 6, 4, new Color32(237, 188, 92, 255));
                cabin.transform.SetParent(boat, false); cabin.rectTransform.anchoredPosition = new Vector2(9, -2);
            }
        }
        AcademyInkGraphic Shape(string name, float x, float y, float w, float h, Color c)
        {
            var r = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            r.SetParent(transform, false); r.anchorMin = r.anchorMax = new Vector2(0, 1); r.pivot = new Vector2(0, 1);
            r.anchoredPosition = new Vector2(x, -y); r.sizeDelta = new Vector2(w, h);
            var g = r.gameObject.AddComponent<AcademyInkGraphic>(); g.form = AcademyInkGraphic.Form.Disc; g.color = c; g.raycastTarget = false; return g;
        }
        void Update()
        {
            if (paused == null || paused() || quiet()) return;
            time += Mathf.Min(Time.unscaledDeltaTime, .05f);
            for (int i = 0; i < motes.Count; i++)
                motes[i].anchoredPosition = origins[i] + new Vector2(Mathf.Sin(time * .3f + i) * (place == "map" ? 8 : 18), Mathf.Cos(time * .2f + i) * (place == "map" ? 1 : 7));
            for (int i = 0; i < lamps.Count; i++) lamps[i].color = new Color(1, .75f, .31f, .07f + Mathf.Sin(time * .8f + i) * .018f);
            if (boat != null) boat.anchoredPosition = new Vector2(826 + Mathf.Sin(time * .048f) * 70, -706 + Mathf.Sin(time * .096f) * 64);
        }
    }

    public sealed class HalcyonMapCamera : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler
    {
        public RectTransform world;
        public Func<bool> blocked, quiet;
        Vector2 pan, target;
        float zoom = 1, targetZoom = 1;
        public float Zoom => zoom;
        public Vector2 Pan => pan;
        public static Vector2 ClampPan(Vector2 p, float z) => new Vector2(Mathf.Clamp(p.x, -800 * (z - 1), 800 * (z - 1)), Mathf.Clamp(p.y, -450 * (z - 1), 450 * (z - 1)));
        public void ResetView() { target = Vector2.zero; targetZoom = 1; }
        public void ZoomBy(float amount) { targetZoom = Mathf.Clamp(targetZoom + amount, 1, 1.8f); target = ClampPan(target, targetZoom); }
        public void OnBeginDrag(PointerEventData e) { if (!blocked()) e.eligibleForClick = false; }
        public void OnDrag(PointerEventData e)
        {
            if (blocked()) return;
            if (targetZoom == 1) targetZoom = 1.2f;
            target = ClampPan(target + e.delta / GetComponentInParent<Canvas>().scaleFactor, targetZoom);
        }
        public void OnEndDrag(PointerEventData e) { }
        public void OnScroll(PointerEventData e)
        {
            if (blocked()) return;
            var kb = Keyboard.current;
            if (kb != null && (kb.ctrlKey.isPressed || kb.leftMetaKey.isPressed || kb.rightMetaKey.isPressed)) ZoomBy(e.scrollDelta.y * .035f);
            else { if (targetZoom == 1) targetZoom = 1.2f; target = ClampPan(target + e.scrollDelta * 22, targetZoom); }
        }
        void Update()
        {
            if (world == null || blocked == null || blocked()) return;
            var kb = Keyboard.current;
            if (kb != null)
            {
                if (kb.homeKey.wasPressedThisFrame) ResetView();
                if (kb.equalsKey.wasPressedThisFrame || kb.numpadPlusKey.wasPressedThisFrame) ZoomBy(.15f);
                if (kb.minusKey.wasPressedThisFrame || kb.numpadMinusKey.wasPressedThisFrame) ZoomBy(-.15f);
                var d = new Vector2((kb.leftArrowKey.isPressed ? 1 : 0) - (kb.rightArrowKey.isPressed ? 1 : 0), (kb.downArrowKey.isPressed ? 1 : 0) - (kb.upArrowKey.isPressed ? 1 : 0));
                if (d.sqrMagnitude > 0) { if (targetZoom == 1) targetZoom = 1.2f; target = ClampPan(target + d * Time.unscaledDeltaTime * 340, targetZoom); }
            }
            float t = quiet() ? 1 : 1 - Mathf.Exp(-Time.unscaledDeltaTime * 12);
            zoom = Mathf.Lerp(zoom, targetZoom, t); pan = ClampPan(Vector2.Lerp(pan, target, t), zoom);
            world.localScale = Vector3.one * zoom;
            world.anchoredPosition = new Vector2(800 * (1 - zoom), 450 * (zoom - 1)) + pan;
        }
    }
}
