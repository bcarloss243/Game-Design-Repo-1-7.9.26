using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Halcyon.FirstWeather
{
    // Independent scenery layers and live objects. No story time, Pressure or activity is spent here.
    public sealed class AcademyCourtyard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler
    {
        public const float MinZoom = 1f, MaxZoom = 2.05f;
        public static readonly Vector2 OverviewCenter = new Vector2(800, 450);
        static readonly Color Ink = new Color32(40, 43, 44, 255);
        static readonly Color Brass = new Color32(186, 148, 85, 255);
        static readonly Color Lamp = new Color32(255, 215, 137, 255);
        RectTransform world, background, architecture, nearLayer, minuteHand, hourHand, secondHand;
        Func<bool> isPaused, reducedMotion;
        Vector2 center = OverviewCenter, targetCenter = OverviewCenter, panVelocity;
        float zoom = 1, targetZoom = 1, zoomVelocity, elapsed;
        int clockHour;
        bool initialized, dragging;
        readonly List<AcademyInkGraphic> glows = new List<AcademyInkGraphic>();
        readonly List<AcademyInkGraphic> flags = new List<AcademyInkGraphic>();
        readonly List<RectTransform> reflections = new List<RectTransform>();
        readonly List<Vector2> reflectionOrigins = new List<Vector2>();
        readonly List<Walker> walkers = new List<Walker>();
        readonly List<RectTransform> motes = new List<RectTransform>();
        readonly List<Vector2> moteOrigins = new List<Vector2>();

        sealed class Walker
        {
            public RectTransform root;
            public RawImage sprite;
            public int row;
            public CanvasGroup alpha;
            public Vector2[] path;
            public float phase, duration, size;
        }

        public Vector2 CameraCenter => center;
        public float CameraZoom => zoom;
        public float AmbientSeconds => elapsed;
        public int MovingStudentCount => walkers.Count;

        public void Initialize(Texture2D ground, Texture2D building, Func<bool> paused, Func<bool> quiet, int hour)
        {
            isPaused = paused; reducedMotion = quiet; clockHour = hour;
            world = Node("Academy world", transform, 0, 0, 1600, 900);
            background = Node("Distant city and paving", world, 0, 0, 1600, 900);
            Artwork(background, "Enclosed court and neighboring buildings", ground, 0, 0, 1600, 900);
            Glow(background, "Late light", 1000, 50, 640, 490, new Color(1, .75f, .5f, .07f));

            architecture = Node("Architecture and attached details", world, 0, 0, 1600, 900);
            // Architecture and paving share one painted perspective; people and details remain live layers.
            // Coordinates are in the architecture's source image, keeping details attached during navigation.
            var clock = BuildingPoint(828, 220);
            hourHand = Hand("Hour hand", clock, 18, 2.4f, Ink);
            minuteHand = Hand("Minute hand", clock, 26, 1.6f, Ink);
            secondHand = Hand("Second hand", clock, 27, .8f, new Color32(137, 79, 56, 255));
            Shape(architecture, "Clock spindle", clock.x - 2, clock.y - 2, 4, 4, AcademyInkGraphic.Form.Disc, Brass);

            foreach (var p in new[] { new Vector2(493, 535), new Vector2(692, 518), new Vector2(966, 521), new Vector2(1163, 536), new Vector2(493, 375), new Vector2(697, 365), new Vector2(969, 369), new Vector2(1162, 381) })
            {
                var q = BuildingPoint(p.x, p.y);
                glows.Add(Glow(architecture, "Occupied window", q.x - 9, q.y - 18, 18, 36, new Color(1, .78f, .4f, .12f)));
            }
            foreach (var p in new[] { new Vector2(764, 484), new Vector2(899, 484) })
            {
                var q = BuildingPoint(p.x, p.y);
                glows.Add(Glow(architecture, "Entrance lantern", q.x - 16, q.y - 20, 32, 40, new Color(1, .78f, .4f, .15f)));
            }


            // Routes stay on the open forecourt and central stairs, clear of the wings.
            for (int i = 0; i < 8; i++)
            {
                bool stairs = i % 3 == 0;
                Vector2[] route = stairs
                    ? new[] { new Vector2(320, 678), new Vector2(645, 678), new Vector2(831, 623), new Vector2(831, 579) }
                    : new[] { new Vector2(300, 665 + (i % 3) * 13), new Vector2(650, 678 + (i % 2) * 15), new Vector2(1000, 664 + (i % 3) * 12), new Vector2(1310, 657 + (i % 2) * 15) };
                if (i % 2 == 1) Array.Reverse(route);
                AddStudent(route, i * .137f, 36 + i * 4, 76 + i % 3 * 3, i);
            }

            nearLayer = Node("Near water and airborne leaves", world, 0, 0, 1600, 900);
            Artwork(nearLayer, "Near reflecting channel", ground, 0, 720, 1600, 180).uvRect = new Rect(0, 0, 1, .2f);
            for (int i = 0; i < 55; i++)
            {
                float x = 100 + i * 26.1f, y = 806 + i % 7 * 6;
                var line = Shape(nearLayer, "Moving water reflection", x, y, 12 + i % 5 * 8, .9f, AcademyInkGraphic.Form.Disc,
                    i % 3 == 0 ? new Color(.92f, .75f, .58f, .24f) : new Color(.57f, .77f, .72f, .21f));
                reflections.Add(line.rectTransform); reflectionOrigins.Add(new Vector2(x, -y));
            }
            for (int i = 0; i < 14; i++)
            {
                var x = 270 + (i * 127) % 1140; var y = 510 + (i * 61) % 280;
                var leaf = Shape(nearLayer, "Windborne leaf", x, y, 3 + i % 3, 2, AcademyInkGraphic.Form.Disc, new Color(.7f, .7f, .43f, .65f));
                motes.Add(leaf.rectTransform); moteOrigins.Add(new Vector2(x, -y));
            }
            initialized = true;
            Animate(0); ApplyCamera(true);
        }

        static Vector2 BuildingPoint(float x, float y) => new Vector2(x, y);

        RectTransform Hand(string name, Vector2 at, float length, float width, Color color)
        {
            var h = Shape(architecture, name, at.x, at.y, width, length, AcademyInkGraphic.Form.Hand, color).rectTransform;
            h.pivot = new Vector2(.5f, 0);
            h.anchoredPosition = new Vector2(at.x, -at.y);
            return h;
        }

        void AddBanner(float x, float y, Color color)
        {
            Shape(architecture, "Brass banner pole", x, y - 108, 2.5f, 108, AcademyInkGraphic.Form.Quad, Brass);
            Shape(architecture, "Pole finial", x - 2, y - 114, 6.5f, 8, AcademyInkGraphic.Form.Disc, Lamp);
            var flag = Shape(architecture, "Wind-driven academy banner", x + 2, y - 103, 34, 63, AcademyInkGraphic.Form.Cloth, color);
            flag.phase = x * .017f; flags.Add(flag);
            Shape(architecture, "Pole foot", x - 5, y - 3, 13, 5, AcademyInkGraphic.Form.Disc, Ink);
        }

        void AddStudent(Vector2[] route, float phase, float duration, float height, int variant)
        {
            var w = new Walker { path = route, phase = phase, duration = duration, size = height, row = variant % 2 };
            w.root = Node("Student following forecourt route " + variant, world, 0, 0, height * .75f, height);
            w.alpha = w.root.gameObject.AddComponent<CanvasGroup>();
            w.sprite = Artwork(w.root, "Painted student walk cycle", Resources.Load<Texture2D>("HalcyonArtV3/students"), -height * .375f, -height, height * .75f, height);
            w.sprite.uvRect = new Rect(0, w.row == 0 ? .5f : 0, .25f, .5f);
            walkers.Add(w);
        }

        void Update()
        {
            if (!initialized || isPaused()) return;
            float dt = Mathf.Min(Time.unscaledDeltaTime, .05f);
            bool quiet = reducedMotion();
            TickAtmosphere(dt);
            var kb = Keyboard.current;
            if (kb != null)
            {
                var direction = new Vector2((kb.rightArrowKey.isPressed ? 1 : 0) - (kb.leftArrowKey.isPressed ? 1 : 0),
                    (kb.downArrowKey.isPressed ? 1 : 0) - (kb.upArrowKey.isPressed ? 1 : 0));
                if (direction.sqrMagnitude > 0) targetCenter = ClampCenter(targetCenter + direction.normalized * dt * 390 / targetZoom, targetZoom);
                if (kb.homeKey.wasPressedThisFrame) Overview();
                if (kb.equalsKey.wasPressedThisFrame || kb.numpadPlusKey.wasPressedThisFrame) ChangeZoom(.2f);
                if (kb.minusKey.wasPressedThisFrame || kb.numpadMinusKey.wasPressedThisFrame) ChangeZoom(-.2f);
            }
            ApplyCamera(quiet || dragging);
        }

        public void TickAtmosphere(float dt)
        {
            if (!initialized || isPaused() || reducedMotion() || float.IsNaN(dt) || float.IsInfinity(dt)) return;
            elapsed += Mathf.Clamp(dt, 0, .05f);
            Animate(elapsed);
        }

        void Animate(float t)
        {
            hourHand.localEulerAngles = new Vector3(0, 0, -(clockHour % 12 * 30 + t / 120));
            minuteHand.localEulerAngles = new Vector3(0, 0, -t / 10);
            secondHand.localEulerAngles = new Vector3(0, 0, -Mathf.Floor(t) * 6);
            for (int i = 0; i < glows.Count; i++)
            {
                float a = i < 10 ? .10f + .10f * (.5f + .5f * Mathf.Sin(t * .22f + i * 1.8f)) : .24f + .035f * Mathf.Sin(t * 2 + i);
                glows[i].color = new Color(1, .78f, .4f, a);
            }
            foreach (var f in flags) { f.phase = t * 1.7f + f.rectTransform.anchoredPosition.x * .017f; f.SetVerticesDirty(); }
            foreach (var w in walkers)
            {
                float cycle = Mathf.Repeat(t / w.duration + w.phase, 1), progress = Mathf.Clamp01(cycle / .84f);
                float segment = progress * (w.path.Length - 1); int at = Mathf.Min((int)segment, w.path.Length - 2);
                Vector2 p = Vector2.Lerp(w.path[at], w.path[at + 1], segment - at);
                float stride = cycle < .84f ? Mathf.Sin((t + w.phase * 20) * 6) : 0;
                float depth = Mathf.Lerp(.78f, 1.1f, Mathf.InverseLerp(600, 800, p.y));
                w.root.anchoredPosition = new Vector2(p.x, -p.y - Mathf.Abs(stride) * .7f);
                w.root.localScale = Vector3.one * depth;
                int frame = cycle < .84f ? Mathf.FloorToInt((t + w.phase) * 5) % 4 : 1;
                bool left = w.path[at + 1].x < w.path[at].x;
                w.sprite.uvRect = new Rect((frame + (left ? 1 : 0)) / 4f, w.row == 0 ? .5f : 0, left ? -.25f : .25f, .5f);
                // Fade at route endpoints prevents teleporting when a route restarts.
                w.alpha.alpha = Mathf.Min(Mathf.Clamp01(cycle * 25), Mathf.Clamp01((.91f - cycle) * 25));
            }
            for (int i = 0; i < reflections.Count; i++)
                reflections[i].anchoredPosition = reflectionOrigins[i] + new Vector2(Mathf.Sin(t * .45f + i) * 8, Mathf.Sin(t * .72f + i) * 1.3f);
            for (int i = 0; i < motes.Count; i++)
            {
                motes[i].anchoredPosition = moteOrigins[i] + new Vector2(Mathf.Sin(t * .13f + i) * 27, Mathf.Sin(t * .3f + i * .7f) * 9);
                motes[i].localEulerAngles = new Vector3(0, 0, t * 12 + i * 28);
            }
        }

        public static Vector2 ClampCenter(Vector2 value, float scale)
        {
            scale = Mathf.Clamp(scale, MinZoom, MaxZoom);
            float halfW = 800 / scale, halfH = 450 / scale;
            return new Vector2(Mathf.Clamp(value.x, halfW, 1600 - halfW), Mathf.Clamp(value.y, halfH, 900 - halfH));
        }

        public static Vector2 ZoomAnchor(Vector2 current, float oldZoom, float newZoom, Vector2 screenPoint)
            => current + (screenPoint - OverviewCenter) * (1 / oldZoom - 1 / newZoom);

        void ApplyCamera(bool immediate)
        {
            targetCenter = ClampCenter(targetCenter, targetZoom);
            if (immediate) { center = targetCenter; zoom = targetZoom; panVelocity = Vector2.zero; zoomVelocity = 0; }
            else
            {
                float dt = Mathf.Min(Time.unscaledDeltaTime, .05f);
                zoom = Mathf.SmoothDamp(zoom, targetZoom, ref zoomVelocity, .16f, Mathf.Infinity, dt);
                center = Vector2.SmoothDamp(center, targetCenter, ref panVelocity, .16f, Mathf.Infinity, dt);
            }
            center = ClampCenter(center, zoom);
            world.localScale = Vector3.one * zoom;
            var position = OverviewCenter - center * zoom;
            world.anchoredPosition = new Vector2(position.x, -position.y);
            // Attached building details share one layer; only depth planes receive differential movement.
            Vector2 offset = reducedMotion() ? Vector2.zero : (center - OverviewCenter) * .012f;
            background.anchoredPosition = Vector2.zero;
            nearLayer.anchoredPosition = new Vector2(-offset.x, offset.y);
        }

        public void Overview() { targetZoom = 1; targetCenter = OverviewCenter; }
        public void FocusEntrance() { targetZoom = 1.72f; targetCenter = ClampCenter(new Vector2(800, 588), targetZoom); }
        public void FocusTower() { targetZoom = 1.85f; targetCenter = ClampCenter(new Vector2(828, 220), targetZoom); }
        public void ChangeZoom(float delta) => SetZoom(targetZoom + delta, OverviewCenter);
        void SetZoom(float value, Vector2 screenPoint)
        {
            float next = Mathf.Clamp(value, MinZoom, MaxZoom);
            targetCenter = ClampCenter(ZoomAnchor(targetCenter, targetZoom, next, screenPoint), next);
            targetZoom = next;
        }
        public void OnBeginDrag(PointerEventData e) { if (!isPaused()) { dragging = true; e.eligibleForClick = false; if (targetZoom < 1.15f) targetZoom = 1.15f; } }
        public void OnEndDrag(PointerEventData e) { dragging = false; }
        public void OnDrag(PointerEventData e)
        {
            if (!dragging || isPaused()) return;
            float scale = GetComponentInParent<Canvas>().scaleFactor;
            targetCenter = ClampCenter(targetCenter + new Vector2(-e.delta.x, e.delta.y) / (Mathf.Max(.01f, scale) * targetZoom), targetZoom);
        }
        public void OnScroll(PointerEventData e)
        {
            if (isPaused()) return;
            var kb = Keyboard.current;
            if (kb != null && (kb.leftCtrlKey.isPressed || kb.rightCtrlKey.isPressed || kb.leftMetaKey.isPressed || kb.rightMetaKey.isPressed))
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, e.position, e.pressEventCamera, out var p);
                SetZoom(targetZoom + Mathf.Clamp(e.scrollDelta.y, -3, 3) * .06f, new Vector2(p.x, -p.y));
            }
            else { if (targetZoom < 1.15f) targetZoom = 1.15f; targetCenter = ClampCenter(targetCenter + new Vector2(-Mathf.Clamp(e.scrollDelta.x, -8, 8), Mathf.Clamp(e.scrollDelta.y, -8, 8)) * 26 / targetZoom, targetZoom); }
            e.Use();
        }

        static RectTransform Node(string name, Transform parent, float x, float y, float w, float h)
        {
            var r = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            r.SetParent(parent, false); r.anchorMin = r.anchorMax = r.pivot = new Vector2(0, 1);
            r.anchoredPosition = new Vector2(x, -y); r.sizeDelta = new Vector2(w, h); return r;
        }
        static RawImage Artwork(Transform parent, string name, Texture2D texture, float x, float y, float w, float h)
        {
            var image = Node(name, parent, x, y, w, h).gameObject.AddComponent<RawImage>();
            image.texture = texture; image.raycastTarget = false;
            return image;
        }
        static AcademyInkGraphic Shape(Transform p, string name, float x, float y, float w, float h, AcademyInkGraphic.Form form, Color c)
        {
            var shape = Node(name, p, x, y, w, h).gameObject.AddComponent<AcademyInkGraphic>();
            shape.form = form; shape.color = c; shape.raycastTarget = false; return shape;
        }
        static AcademyInkGraphic Glow(Transform p, string name, float x, float y, float w, float h, Color c)
            => Shape(p, name, x, y, w, h, AcademyInkGraphic.Form.Glow, c);
    }

    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class AcademyInkGraphic : MaskableGraphic
    {
        public enum Form { Quad, Disc, Glow, Hand, Coat, Cloth }
        public Form form;
        public float phase;
        protected override void Awake() { base.Awake(); useLegacyMeshGeneration = false; }
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear(); var r = rectTransform.rect;
            if (form == Form.Glow || form == Form.Disc)
            {
                vh.AddVert(r.center, color, Vector2.zero);
                Color edge = color; if (form == Form.Glow) edge.a = 0;
                for (int i = 0; i <= 32; i++)
                {
                    float a = i * Mathf.PI / 16;
                    vh.AddVert(r.center + new Vector2(Mathf.Cos(a) * r.width * .5f, Mathf.Sin(a) * r.height * .5f), edge, Vector2.zero);
                    if (i > 0) vh.AddTriangle(0, i, i + 1);
                }
                return;
            }
            if (form == Form.Cloth)
            {
                for (int i = 0; i <= 12; i++)
                {
                    float f = i / 12f, wave = Mathf.Sin(phase + f * 5) * 4 * f;
                    float x = r.xMin + r.width * f;
                    Color c = color * (1 + Mathf.Sin(phase + f * 5) * .12f); c.a = color.a;
                    vh.AddVert(new Vector3(x, r.yMax + wave), c, Vector2.zero);
                    vh.AddVert(new Vector3(x, r.yMin + wave + f * 7), c, Vector2.zero);
                    if (i > 0) { int a = (i - 1) * 2; vh.AddTriangle(a, a + 2, a + 1); vh.AddTriangle(a + 1, a + 2, a + 3); }
                }
                return;
            }
            Vector2[] points = form == Form.Hand ? new[] { new Vector2(r.center.x, r.yMax), new Vector2(r.xMax, r.yMin), new Vector2(r.xMin, r.yMin) } :
                form == Form.Coat ? new[] { new Vector2(r.xMin + r.width * .28f, r.yMax), new Vector2(r.xMax - r.width * .28f, r.yMax), new Vector2(r.xMax, r.yMin), new Vector2(r.xMin, r.yMin) } :
                new[] { new Vector2(r.xMin, r.yMax), new Vector2(r.xMax, r.yMax), new Vector2(r.xMax, r.yMin), new Vector2(r.xMin, r.yMin) };
            foreach (var point in points) vh.AddVert(point, color, Vector2.zero);
            for (int i = 1; i < points.Length - 1; i++) vh.AddTriangle(0, i, i + 1);
        }
    }
}
