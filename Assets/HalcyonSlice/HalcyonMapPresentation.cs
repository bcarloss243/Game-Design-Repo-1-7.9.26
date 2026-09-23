using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Halcyon.FirstWeather
{
    // Presentation is separate from story text and progression. Existing saves remain valid.
    public partial class HalcyonSlice
    {
        TMP_FontAsset displayFont, titleFont;
        Texture2D instrumentHousing;
        bool timetableOpen = true;
        readonly Color paper = new Color32(235, 220, 190, 255);
        readonly Color dialInk = new Color32(64, 57, 49, 255);
        readonly Color warmGold = new Color32(224, 191, 135, 255);

        void PreparePresentation()
        {
            if (displayFont == null)
            {
                var source = Resources.Load<Font>("HalcyonFonts/CormorantGaramond-Semibold");
                if (source != null)
                {
                    displayFont = TMP_FontAsset.CreateFontAsset(source);
                    displayFont.name = "Halcyon Cormorant Display (runtime)";
                    displayFont.fallbackFontAssetTable = new System.Collections.Generic.List<TMP_FontAsset> { font };
                }
            }
            if (titleFont == null)
            {
                var source = Resources.Load<Font>("HalcyonFonts/CinzelDecorative-Regular");
                if (source != null) { titleFont = TMP_FontAsset.CreateFontAsset(source); titleFont.fallbackFontAssetTable = new System.Collections.Generic.List<TMP_FontAsset> { displayFont, font }; }
            }
            if (instrumentHousing == null)
                instrumentHousing = Resources.Load<Texture2D>("HalcyonUI/BrassBarometer");
        }

        void ReleasePresentation()
        {
            if (paintedFrame != null) Destroy(paintedFrame);
            if (titleFont != null) { foreach (var atlas in titleFont.atlasTextures) if (atlas != null) Destroy(atlas); Destroy(titleFont.material); Destroy(titleFont); }
            if (displayFont == null) return;
            foreach (var atlas in displayFont.atlasTextures) if (atlas != null) Destroy(atlas);
            if (displayFont.material != null) Destroy(displayFont.material);
            Destroy(displayFont);
        }

        TMP_Text BookText(Transform p, string value, float x, float y, float w, float h, float size, Color color,
            bool serif = true, TextAlignmentOptions align = TextAlignmentOptions.TopLeft, float spacing = 0)
        {
            var t = Text(p, value, x, y, w, h, size, color, false, align);
            t.font = serif && displayFont != null ? displayFont : font;
            t.characterSpacing = spacing;
            t.lineSpacing = 0;
            return t;
        }

        HalcyonOrnament Ornament(Transform p, string name, float x, float y, float w, float h,
            HalcyonOrnament.Shape shape, Color color)
        {
            var graphic = Rect(name, p, x, y, w, h).gameObject.AddComponent<HalcyonOrnament>();
            graphic.shape = shape;
            graphic.color = color;
            graphic.raycastTarget = false;
            return graphic;
        }

        void Hairline(Transform p, float x1, float y1, float x2, float y2, Color c, float width = 1)
        {
            var length = Vector2.Distance(new Vector2(x1, y1), new Vector2(x2, y2));
            var r = Panel(p, x1, y1, length, width, c).rectTransform;
            r.pivot = new Vector2(0, .5f);
            r.localEulerAngles = new Vector3(0, 0, -Mathf.Atan2(y2 - y1, x2 - x1) * Mathf.Rad2Deg);
        }

        void EngravedFrame(Transform p, float x, float y, float w, float h, Color fill)
        {
            Stationery(p, "Academy paper", x, y, w, h);
        }

        Button MapControl(Transform p, string label, float x, float y, float w, Action action, bool primary = false)
        {
            return StationeryButton(p, label, x, y, w, 45, action, true, primary, 25, true);
        }

        RectTransform mapLayer;
        HalcyonMapCamera mapCamera;

        void DrawMapPresentation()
        {
            PreparePresentation();
            var viewport = Rect("Explorable district", screen, 0, 0, 1600, 900);
            viewport.gameObject.AddComponent<RectMask2D>();
            var hit = viewport.gameObject.AddComponent<Image>(); hit.color = Color.clear;
            mapLayer = Rect("District geography and markers", viewport, 0, 0, 1600, 900);
            Picture(mapLayer, "map", 0, 0, 1600, 900); AddSceneLife(mapLayer, "map");
            mapCamera = viewport.gameObject.AddComponent<HalcyonMapCamera>();
            mapCamera.world = mapLayer; mapCamera.blocked = () => modal || title; mapCamera.quiet = () => quietMotion;
            Ornament(screen, "Quiet upper edge", 0, 0, 1600, 190, HalcyonOrnament.Shape.FadeDown, new Color(.07f, .085f, .1f, .78f));
            Ornament(screen, "Quiet lower edge", 0, 632, 1600, 268, HalcyonOrnament.Shape.FadeUp, new Color(.06f, .09f, .1f, .88f));

            RunningTitle(screen, "Halcyon", "Academy district", 43, 39, 460);
            BookText(screen, TimeLabel().Replace(" / ", "  ·  "), 1080, 36, 470, 35, 23, cream, true, TextAlignmentOptions.Right);
            InkLink(screen, "Tracks", 955, 86, 132, () => OpenModal("tracks"));
            InkLink(screen, "Journal", 1110, 86, 154, () => OpenModal("journal"));
            InkLink(screen, "Settings", 1276, 86, 130, () => OpenModal("settings"));
            InkLink(screen, "Pause", 1418, 86, 136, () => OpenModal("pause"));

            bool morning = state.phase == "arrival" || state.phase == "morning2";
            bool afternoon = state.phase == "afternoon";
            bool greenhouse = state.phase == "corridor" || state.phase == "homecoming";
            MapPlace("Residence", morning ? "BEGIN HERE" : afternoon ? (state.slots == 0 ? "EVENING AT HOME" : "REST / EVENING") : "MORNING COMPLETE", 290, 264,
                morning || afternoon, true, () => {
                    if (morning) StartStory(state.day == 1 ? "morning1" : "morning2");
                    else if (state.slots == 0) BeginEvening(); else OpenModal("residence");
                });
            MapPlace("The Academy", state.phase == "class1" || state.phase == "class2" ? "CLASS IS BEGINNING" : "EXPLORE THE GROUNDS", 804, 220,
                true, true, OpenAcademyExterior);
            MapPlace("Reading Room", state.studied ? "PREPARED" : afternoon ? "PREPARE  ·  +" + state.ActivityDelta("study") + "  ·  1 SLOT" : "AFTER CLASS", 390, 452,
                state.CanActivity("study"), true, () => DoActivity("study"));
            MapPlace("Canal Steps", state.socialized ? "VISITED" : afternoon ? "MEET JULES  ·  −7  ·  1 SLOT" : "AFTER CLASS", 1005, 615,
                state.CanActivity("social"), true, () => DoActivity("social"));
            MapPlace("Greenhouse", greenhouse ? (state.phase == "homecoming" ? "READ LOLA’S MESSAGE" : "FIND SOMEWHERE QUIET") : "UNDISCOVERED", 1253, 360,
                greenhouse, greenhouse, () => {
                    if (state.phase == "homecoming") { state.finished = true; state.view = "ending"; Save(); Render(); }
                    else StartStory("greenhouse");
                });

            DrawInstrument(screen, 27, 555, 308);
            BookText(screen, "Duvernay · Halcyon", 52, 850, 257, 33, 23, cream, true, TextAlignmentOptions.Center);
            DrawTimetable();
            InkLink(screen, "−", 360, 752, 52, () => mapCamera.ZoomBy(-.15f));
            InkLink(screen, "+", 422, 752, 52, () => mapCamera.ZoomBy(.15f));
            InkLink(screen, "Recenter", 496, 752, 154, () => mapCamera.ResetView());
            BookText(screen, "Drag or scroll to explore", 674, 760, 450, 31, 23, cream);
            BookText(screen, "Where will you go?", 360, 816, 670, 36, 29, cream);
            BookText(screen, "Filled marker · open    /    Outline · unavailable", 360, 852, 710, 30, 21, cream);
        }

        void DrawTimetable()
        {
            const float x = 1165, w = 389;
            if (!timetableOpen)
            {
                MapControl(screen, "Today's timetable", x, 811, w, () => { timetableOpen = true; Render(); });
                return;
            }
            Stationery(screen, "Folded timetable", x, 652, w, 218);
            BookText(screen, "Today's timetable", x + 22, 667, 288, 46, 33, plum);
            MapControl(screen, "−", x + w - 59, 664, 42, () => { timetableOpen = false; Render(); }).gameObject.name = "Fold timetable";
            Hairline(screen, x + 22, 719, x + w - 22, 719, ruleInk);
            string task = state.phase == "arrival" || state.phase == "morning2" ? "Begin at your residence." :
                state.phase == "class1" || state.phase == "class2" ? "Your next class is at the Academy." :
                state.phase == "afternoon" ? (state.slots > 0 ? "Choose how to spend your afternoon." : "Return home for the evening.") :
                state.phase == "corridor" ? "Visit the greenhouse." : "A message from Lola is waiting.";
            BookText(screen, task, x + 22, 732, w - 44, 82, largeText ? 29 : 27, printedInk);
            string status = state.phase == "afternoon" ? state.slots + " of 2 afternoon slots left" :
                state.phase == "arrival" || state.phase == "morning2" ? "Morning" :
                state.phase == "corridor" || state.phase == "homecoming" ? "After class" : "Class time";
            BookText(screen, status, x + 22, 827, w - 44, 28, 21, fadedInk);
        }

        void MapPlace(string label, string detail, float x, float y, bool available, bool discovered, Action action)
        {
            const float width = 250;
            var r = Rect("Location: " + label, mapLayer, x - width / 2, y - 12, width, 114);
            Ornament(r, "Pin shadow", 111, 2, 28, 28, HalcyonOrnament.Shape.Diamond, new Color(.12f, .06f, .1f, .7f));
            Ornament(r, "Map pin", 113, 0, 24, 24, available ? HalcyonOrnament.Shape.Diamond : HalcyonOrnament.Shape.DiamondOutline, cream);
            if (available) Ornament(r, "Pin center", 121, 8, 8, 8, HalcyonOrnament.Shape.Diamond, plum);
            Hairline(r, 125, 27, 125, 39, brass);
            Stationery(r, "Printed location ticket", 0, 38, width, 73, !available);
            BookText(r, discovered ? label : "?  " + label, 10, 40, width - 20, 38, 30, available ? plum : cream, true, TextAlignmentOptions.Center);
            BookText(r, detail.ToLowerInvariant(), 8, 79, width - 16, 29, 20, available ? fadedInk : cream, true, TextAlignmentOptions.Center);
            var hit = Panel(r, 0, 0, width, 114, Color.white); hit.raycastTarget = available;
            var b = r.gameObject.AddComponent<Button>(); b.targetGraphic = hit;
            var colors = b.colors; colors.normalColor = Color.clear;
            colors.highlightedColor = new Color(.35f, .16f, .23f, .10f);
            colors.selectedColor = colors.highlightedColor; colors.pressedColor = new Color(.15f, .07f, .12f, .24f);
            colors.disabledColor = Color.clear; b.colors = colors; b.interactable = available;
            var focus = Ornament(r, "Location focus", 3, 41, width - 6, 67, HalcyonOrnament.Shape.CutOutline, available ? plum : cream);
            focus.inset = 0; focus.gameObject.SetActive(false);
            r.gameObject.AddComponent<HalcyonMapFocus>().outline = focus.gameObject;
            b.onClick.AddListener(() => { if (!muted) clicks.PlayOneShot(clickClip, .07f); action(); });
            buttons.Add(b);
        }

        void DrawInstrument(Transform parent, float x, float y, float size)
        {
            PreparePresentation();
            var r = Rect("Molly's brass Pressure instrument", parent, x, y, size, size);
            Ornament(r, "Instrument shadow", size * .01f, size * .025f, size, size, HalcyonOrnament.Shape.Disc, new Color(0, 0, 0, .25f));
            if (instrumentHousing != null)
            {
                var image = Rect("Painted brass and enamel", r, 0, 0, size, size).gameObject.AddComponent<RawImage>();
                image.texture = instrumentHousing; image.raycastTarget = false;
            }
            else Ornament(r, "Enamel fallback", 0, 0, size, size, HalcyonOrnament.Shape.Disc, paper);
            float c = size / 2, radius = size * .316f;
            for (int i = 0; i <= 50; i++)
            {
                float angle = Mathf.Lerp(-120, 120, i / 50f) * Mathf.Deg2Rad;
                float length = size * (i % 5 == 0 ? .044f : .02f);
                Hairline(r, c + Mathf.Sin(angle) * radius, c - Mathf.Cos(angle) * radius,
                    c + Mathf.Sin(angle) * (radius - length), c - Mathf.Cos(angle) * (radius - length), dialInk, i % 5 == 0 ? 1.35f : .8f);
                if (i % 5 == 0)
                {
                    float inner = radius - size * .074f;
                    BookText(r, (i * 2).ToString(), c + Mathf.Sin(angle) * inner - size * .055f,
                        c - Mathf.Cos(angle) * inner - size * .035f, size * .11f, size * .072f, size * .041f, dialInk, false, TextAlignmentOptions.Center);
                }
            }
            // A narrow colored band complements the written zone; values still use the same rules.
            for (int i = 0; i < 100; i++)
            {
                float a = Mathf.Lerp(-120, 120, i / 100f) * Mathf.Deg2Rad;
                float b = Mathf.Lerp(-120, 120, (i + 1) / 100f) * Mathf.Deg2Rad;
                float outer = size * .331f;
                Color zone = i < 20 ? new Color32(89, 125, 125, 255) : i < 45 ? new Color32(117, 136, 97, 255) : i < 70 ? new Color32(185, 139, 67, 255) : new Color32(154, 88, 77, 255);
                Hairline(r, c + Mathf.Sin(a) * outer, c - Mathf.Cos(a) * outer,
                    c + Mathf.Sin(b) * outer, c - Mathf.Cos(b) * outer, zone, size * .011f);
            }
            DrawPortrait(r, "molly", size * .355f, size * .325f, size * .29f, size * .29f, true);
            var pointer = Ornament(r, "Live Pressure needle", c, c, size * .019f, size * .258f, HalcyonOrnament.Shape.Needle, dialInk);
            needle = pointer.rectTransform; needle.pivot = new Vector2(.5f, 0); needle.anchoredPosition = new Vector2(c, -c);
            Ornament(r, "Needle pivot", c - size * .017f, c - size * .017f, size * .034f, size * .034f, HalcyonOrnament.Shape.Disc, warmGold);
            BookText(r, state.Zone, size * .21f, size * .625f, size * .58f, size * .09f, size * .055f, dialInk, false, TextAlignmentOptions.Center);
            BookText(r, "HALCYON", size * .30f, size * .734f, size * .4f, size * .064f, size * .05f, dialInk, true, TextAlignmentOptions.Center);
            PrinterFlower(r, size * .46f, size * .80f, dialInk, size * .075f);
            for (int vent = 0; vent < 3; vent++) Hairline(r, size * (.46f + vent * .04f), size * .11f, size * (.46f + vent * .04f), size * .16f, dialInk, size * .007f);
            Hairline(r, size * .24f, size * .94f, size * .76f, size * .94f, dialInk, size * .018f);
            gaugeFeel = r.gameObject.AddComponent<PressureGaugeFeel>();
            gaugeFeel.needle = needle; gaugeFeel.needleGraphic = pointer;
            gaugeFeel.angleAtZero = 120; gaugeFeel.angleAtHundred = -120;
            gaugeFeel.reducedMotion = quietMotion; gaugeFeel.paused = modal;
            gaugeFeel.Initialize(displayedPressure); gaugeFeel.SetPressure(state.pressure);

        }
    }

    public sealed class HalcyonMapFocus : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {
        public GameObject outline;
        bool hovered, selected;
        void Refresh() { if (outline != null) outline.SetActive((hovered || selected) && GetComponent<Button>().interactable); }
        public void OnPointerEnter(PointerEventData e) { hovered = true; Refresh(); }
        public void OnPointerExit(PointerEventData e) { hovered = false; Refresh(); }
        public void OnSelect(BaseEventData e) { selected = true; Refresh(); }
        public void OnDeselect(BaseEventData e) { selected = false; Refresh(); }
    }

    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class HalcyonOrnament : MaskableGraphic
    {
        public enum Shape { Disc, Diamond, DiamondOutline, Needle, CutPanel, CutOutline, FadeDown, FadeUp }
        public Shape shape;
        public float inset = 6;

        protected override void Awake()
        {
            base.Awake();
            useLegacyMeshGeneration = false;
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            var r = rectTransform.rect;
            if (shape == Shape.FadeDown || shape == Shape.FadeUp)
            {
                Color clear = color; clear.a = 0;
                vh.AddVert(new Vector3(r.xMin, r.yMin), shape == Shape.FadeDown ? clear : color, Vector2.zero);
                vh.AddVert(new Vector3(r.xMin, r.yMax), shape == Shape.FadeDown ? color : clear, Vector2.zero);
                vh.AddVert(new Vector3(r.xMax, r.yMax), shape == Shape.FadeDown ? color : clear, Vector2.zero);
                vh.AddVert(new Vector3(r.xMax, r.yMin), shape == Shape.FadeDown ? clear : color, Vector2.zero);
                vh.AddTriangle(0, 1, 2); vh.AddTriangle(0, 2, 3); return;
            }
            Vector2[] points;
            if (shape == Shape.Disc)
            {
                points = new Vector2[64];
                for (int i = 0; i < points.Length; i++)
                {
                    float a = i * Mathf.PI * 2 / points.Length;
                    points[i] = r.center + new Vector2(Mathf.Cos(a) * r.width / 2, Mathf.Sin(a) * r.height / 2);
                }
            }
            else if (shape == Shape.Diamond || shape == Shape.DiamondOutline)
                points = new[] { new Vector2(r.center.x, r.yMax), new Vector2(r.xMax, r.center.y), new Vector2(r.center.x, r.yMin), new Vector2(r.xMin, r.center.y) };
            else if (shape == Shape.Needle)
                points = new[] { new Vector2(r.center.x, r.yMax), new Vector2(r.xMax, r.yMin), new Vector2(r.center.x, r.yMin + r.height * .08f), new Vector2(r.xMin, r.yMin) };
            else points = new[] {
                new Vector2(r.xMin + inset, r.yMax), new Vector2(r.xMax - inset, r.yMax),
                new Vector2(r.xMax, r.yMax - inset), new Vector2(r.xMax, r.yMin + inset),
                new Vector2(r.xMax - inset, r.yMin), new Vector2(r.xMin + inset, r.yMin),
                new Vector2(r.xMin, r.yMin + inset), new Vector2(r.xMin, r.yMax - inset)
            };
            bool outline = shape == Shape.DiamondOutline || shape == Shape.CutOutline;
            if (outline)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    Vector2 a = points[i], b = points[(i + 1) % points.Length];
                    Vector2 n = new Vector2(-(b - a).y, (b - a).x).normalized * .8f;
                    int at = vh.currentVertCount;
                    vh.AddVert(a - n, color, Vector2.zero); vh.AddVert(a + n, color, Vector2.zero);
                    vh.AddVert(b + n, color, Vector2.zero); vh.AddVert(b - n, color, Vector2.zero);
                    vh.AddTriangle(at, at + 1, at + 2); vh.AddTriangle(at, at + 2, at + 3);
                }
            }
            else
            {
                foreach (var p in points) vh.AddVert(p, color, Vector2.zero);
                for (int i = 1; i < points.Length - 1; i++) vh.AddTriangle(0, i, i + 1);
            }
        }
    }
}
