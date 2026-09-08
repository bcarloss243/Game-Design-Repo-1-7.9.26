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
        TMP_FontAsset displayFont;
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
            if (instrumentHousing == null)
                instrumentHousing = Resources.Load<Texture2D>("HalcyonUI/BrassBarometer");
        }

        void ReleasePresentation()
        {
            if (displayFont == null) return;
            foreach (var atlas in displayFont.atlasTextures) if (atlas != null) Destroy(atlas);
            if (displayFont.material != null) Destroy(displayFont.material);
            Destroy(displayFont);
        }

        TMP_Text BookText(Transform p, string value, float x, float y, float w, float h, float size, Color color,
            bool serif = true, TextAlignmentOptions align = TextAlignmentOptions.TopLeft, float spacing = 0)
        {
            var t = Text(p, value, x, y, w, h, size, color, false, align);
            if (serif && displayFont != null) t.font = displayFont;
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
            var shadow = Ornament(p, "Soft panel shadow", x + 3, y + 5, w, h, HalcyonOrnament.Shape.CutPanel, new Color(0, 0, 0, .3f));
            shadow.inset = 7;
            var plate = Ornament(p, "Enamel panel", x, y, w, h, HalcyonOrnament.Shape.CutPanel, fill);
            plate.inset = 7;
            float corner = 8;
            Color line = new Color(warmGold.r, warmGold.g, warmGold.b, .65f);
            Hairline(p, x + corner, y, x + w - corner, y, line);
            Hairline(p, x + corner, y + h, x + w - corner, y + h, line);
            Hairline(p, x, y + corner, x, y + h - corner, line);
            Hairline(p, x + w, y + corner, x + w, y + h - corner, line);
            Hairline(p, x, y + corner, x + corner, y, line);
            Hairline(p, x + w - corner, y, x + w, y + corner, line);
            Hairline(p, x, y + h - corner, x + corner, y + h, line);
            Hairline(p, x + w - corner, y + h, x + w, y + h - corner, line);
        }

        Button MapControl(Transform p, string label, float x, float y, float w, Action action, bool primary = false)
        {
            var r = Rect(label, p, x, y, w, 45);
            EngravedFrame(r, 0, 0, w, 45, primary ? paper : new Color32(32, 42, 43, 242));
            var hit = Panel(r, 0, 0, w, 45, Color.clear); hit.raycastTarget = true;
            var b = r.gameObject.AddComponent<Button>(); b.targetGraphic = hit;
            var colors = b.colors;
            colors.normalColor = Color.clear;
            colors.highlightedColor = new Color(1, .88f, .63f, .15f);
            colors.selectedColor = new Color(1, .88f, .63f, .22f);
            colors.pressedColor = new Color(0, 0, 0, .2f);
            b.colors = colors;
            // A white target lets the selectable's tint carry the focus alpha.
            hit.color = Color.white;
            b.onClick.AddListener(() => { if (!muted) clicks.PlayOneShot(clickClip, .07f); action(); });
            BookText(r, label, 12, 5, w - 24, 34, 19, primary ? dialInk : paper, false, TextAlignmentOptions.Center);
            buttons.Add(b);
            return b;
        }

        void DrawMapPresentation()
        {
            PreparePresentation();
            Picture(screen, "map", 0, 0, 1600, 900);
            Ornament(screen, "Quiet upper edge", 0, 0, 1600, 190, HalcyonOrnament.Shape.FadeDown, new Color(.07f, .085f, .1f, .78f));
            Ornament(screen, "Quiet lower edge", 0, 632, 1600, 268, HalcyonOrnament.Shape.FadeUp, new Color(.06f, .09f, .1f, .88f));

            Color hairline = new Color(warmGold.r, warmGold.g, warmGold.b, .46f);
            Hairline(screen, 26, 26, 1574, 26, hairline);
            Hairline(screen, 26, 26, 26, 118, hairline);
            Hairline(screen, 1574, 26, 1574, 118, hairline);
            Hairline(screen, 26, 874, 26, 827, hairline);
            Hairline(screen, 1574, 874, 1574, 827, hairline);
            Hairline(screen, 340, 874, 1574, 874, hairline);
            BookText(screen, "Halcyon", 50, 41, 450, 92, 72, new Color(0, 0, 0, .5f));
            BookText(screen, "Halcyon", 48, 38, 450, 92, 72, paper);
            BookText(screen, "A C A D E M Y   D I S T R I C T", 54, 118, 435, 30, 17, warmGold, false);
            Hairline(screen, 55, 157, 125, 157, warmGold);
            Ornament(screen, "Title lozenge", 137, 152, 10, 10, HalcyonOrnament.Shape.Diamond, warmGold);
            Hairline(screen, 160, 157, 231, 157, warmGold);

            BookText(screen, TimeLabel().Replace(" / ", "  ·  "), 888, 46, 650, 33, 19, paper, false, TextAlignmentOptions.Right, 1.4f);
            MapControl(screen, "Journal  ·  J", 1110, 94, 154, () => OpenModal("journal"));
            MapControl(screen, "Settings", 1276, 94, 130, () => OpenModal("settings"));
            MapControl(screen, "Pause", 1418, 94, 136, () => OpenModal("pause"));

            bool morning = state.phase == "arrival" || state.phase == "morning2";
            bool afternoon = state.phase == "afternoon";
            bool greenhouse = state.phase == "corridor" || state.phase == "homecoming";
            MapPlace("Residence", morning ? "BEGIN HERE" : afternoon ? (state.slots == 0 ? "SLEEP" : "REST / END DAY") : "MORNING COMPLETE", 510, 334,
                morning || afternoon, true, () => {
                    if (morning) StartStory(state.day == 1 ? "morning1" : "morning2");
                    else if (state.slots == 0) StartStory("night"); else OpenModal("residence");
                });
            MapPlace("The Academy", state.phase == "class1" || state.phase == "class2" ? "CLASS IS BEGINNING" : morning ? "MORNING ROUTINE FIRST" : "CLASS COMPLETE", 804, 220,
                state.phase == "class1" || state.phase == "class2", true, () => StartStory(state.phase));
            MapPlace("Reading Room", state.studied ? "PREPARED" : afternoon ? "+12 PRESSURE  ·  1 SLOT" : "AFTER CLASS", 687, 467,
                state.CanActivity("study"), true, () => DoActivity("study"));
            MapPlace("Canal Steps", state.socialized ? "VISITED" : afternoon ? "−9 PRESSURE  ·  1 SLOT" : "AFTER CLASS", 1005, 615,
                state.CanActivity("social"), true, () => DoActivity("social"));
            MapPlace("Greenhouse", greenhouse ? (state.phase == "homecoming" ? "FINISH THE STORY" : "OPEN TO VISIT") : "UNDISCOVERED", 1253, 316,
                greenhouse, greenhouse, () => {
                    if (state.phase == "homecoming") { state.finished = true; state.view = "ending"; Save(); Render(); }
                    else StartStory("greenhouse");
                });

            DrawInstrument(screen, 27, 555, 308);
            BookText(screen, "MOLLY'S BAROMETER", 52, 852, 257, 25, 15, paper, false, TextAlignmentOptions.Center, 1.4f);
            DrawTimetable();
            BookText(screen, "SELECT AN OPEN LOCATION", 360, 816, 670, 27, 17, paper, false, TextAlignmentOptions.Left, 1.2f);
            BookText(screen, "Filled diamond: open    ·    Hollow: unavailable", 360, 847, 710, 26, 17, paper, false);
        }

        void DrawTimetable()
        {
            const float x = 1165, w = 389;
            if (!timetableOpen)
            {
                MapControl(screen, "Open timetable", x, 811, w, () => { timetableOpen = true; Render(); });
                return;
            }
            EngravedFrame(screen, x, 652, w, 204, new Color32(31, 42, 43, 248));
            BookText(screen, "Today's timetable", x + 22, 666, 288, 43, 33, paper);
            var close = MapControl(screen, "−", x + w - 62, 664, 42, () => { timetableOpen = false; Render(); });
            close.gameObject.name = "Fold timetable";
            Hairline(screen, x + 22, 718, x + w - 22, 718, new Color(warmGold.r, warmGold.g, warmGold.b, .5f));
            string task = state.phase == "arrival" || state.phase == "morning2" ? "Begin at your residence." :
                state.phase == "class1" || state.phase == "class2" ? "Your next class is at the Academy." :
                state.phase == "afternoon" ? (state.slots > 0 ? "Choose how to spend your afternoon." : "Return to your residence to sleep.") :
                state.phase == "corridor" ? "Visit the greenhouse." : "Return to the greenhouse to finish.";
            BookText(screen, task, x + 22, 731, w - 44, 65, largeText ? 27 : 25, paper);
            string status = state.phase == "afternoon" ? state.slots + " OF 2 AFTERNOON SLOTS LEFT" :
                state.phase == "arrival" || state.phase == "morning2" ? "MORNING" :
                state.phase == "corridor" || state.phase == "homecoming" ? "AFTER CLASS" : "CLASS TIME";
            BookText(screen, status, x + 22, 812, w - 44, 25, 16, warmGold, false, TextAlignmentOptions.Left, .5f);
        }

        void MapPlace(string label, string detail, float x, float y, bool available, bool discovered, Action action)
        {
            const float width = 250;
            var r = Rect("Location: " + label, screen, x - width / 2, y - 12, width, 107);
            var pinShadow = Ornament(r, "Pin shadow", 111, 2, 28, 28, HalcyonOrnament.Shape.Diamond, new Color(0, 0, 0, .65f));
            Ornament(r, "Map pin", 113, 0, 24, 24, available ? HalcyonOrnament.Shape.Diamond : HalcyonOrnament.Shape.DiamondOutline,
                available ? paper : new Color32(220, 206, 181, 255));
            if (available) Ornament(r, "Pin center", 121, 8, 8, 8, HalcyonOrnament.Shape.Diamond, dialInk);
            Hairline(r, 125, 27, 125, 37, warmGold);
            var plate = Ornament(r, "Location label", 0, 38, width, 64, HalcyonOrnament.Shape.CutPanel,
                available ? new Color32(33, 48, 46, 249) : new Color32(32, 37, 41, 235));
            plate.inset = 6;
            Hairline(r, 12, 38, width - 12, 38, available ? warmGold : new Color32(154, 143, 123, 255));
            BookText(r, discovered ? label : "?  " + label, 10, 39, width - 20, 39, 29, paper, true, TextAlignmentOptions.Center);
            BookText(r, detail, 10, 78, width - 20, 22, 14, available ? warmGold : new Color32(206, 199, 182, 255), false, TextAlignmentOptions.Center, .35f);
            var hit = Panel(r, 0, 0, width, 107, Color.white); hit.raycastTarget = available;
            var b = r.gameObject.AddComponent<Button>(); b.targetGraphic = hit;
            var colors = b.colors; colors.normalColor = Color.clear;
            colors.highlightedColor = new Color(1, .89f, .7f, .1f);
            colors.selectedColor = new Color(1, .89f, .7f, .16f);
            colors.pressedColor = new Color(.1f, .1f, .1f, .2f);
            colors.disabledColor = Color.clear; b.colors = colors; b.interactable = available;
            var focus = Ornament(r, "Location focus", -4, 34, width + 8, 74, HalcyonOrnament.Shape.CutOutline, warmGold);
            focus.inset = 8;
            focus.gameObject.SetActive(false);
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
            BookText(r, "PRESSURE", size * .27f, size * .354f, size * .46f, size * .082f, size * .057f, dialInk, false, TextAlignmentOptions.Center, .9f);
            var pointer = Ornament(r, "Live Pressure needle", c, c, size * .025f, size * .245f, HalcyonOrnament.Shape.Needle, dialInk);
            needle = pointer.rectTransform;
            needle.pivot = new Vector2(.5f, 0);
            needle.anchoredPosition = new Vector2(c, -c);
            needle.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(120, -120, displayedPressure / 100f));
            Ornament(r, "Brass spindle", c - size * .031f, c - size * .031f, size * .062f, size * .062f, HalcyonOrnament.Shape.Disc, new Color32(152, 110, 55, 255));
            Ornament(r, "Spindle light", c - size * .015f, c - size * .019f, size * .025f, size * .025f, HalcyonOrnament.Shape.Disc, warmGold);
            gaugeValue = BookText(r, state.pressure + " / 100", size * .25f, size * .56f, size * .5f, size * .12f, size * .092f, dialInk, true, TextAlignmentOptions.Center);
            BookText(r, state.Zone, size * .21f, size * .698f, size * .58f, size * .092f, size * .047f, dialInk, false, TextAlignmentOptions.Center, .5f);
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
