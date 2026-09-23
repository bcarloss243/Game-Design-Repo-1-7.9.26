using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Halcyon.FirstWeather
{
    public partial class HalcyonSlice
    {
        readonly Color plum = new Color32(242, 192, 109, 255);
        readonly Color printedInk = new Color32(247, 236, 216, 255);
        readonly Color fadedInk = new Color32(201, 194, 214, 255);
        readonly Color vellum = new Color32(31, 29, 60, 255);
        readonly Color ruleInk = new Color32(169, 130, 76, 255);

        // Shared with the editor's actual-font layout measurements.
        public const float StoryTextSize = 30, LargerStoryTextSize = 33;
        public const float StoryTextWidth = 1200, StoryTextHeight = 205;
        public const float ChoiceTextSize = 24;

        #if UNITY_EDITOR
        public void PreviewStationery(string sample, bool bigger = false, int page = 0, string dialog = "")
        {
            PreviewMapInterface("afternoon", bigger);
            if (sample == "title") { title = true; Render(); }
            else if (sample == "evening") { state.phase = "evening"; state.view = "evening"; Render(); }
            else if (sample == "academy") { state.phase = "class1"; OpenAcademyExterior(); }
            else if (sample == "garden" || sample == "ending")
            {
                state.view = sample; state.day = 2; state.phase = "corridor"; state.solved = true;
                state.puzzleRound = page; Render();
            }
            else if (sample != "map")
            {
                state.view = "story"; state.story = sample; state.page = page; Render();
            }
            if (!string.IsNullOrEmpty(dialog)) OpenModal(dialog);
        }
        #endif

        Sprite paintedFrame;
        RectTransform Stationery(Transform p, string name, float x, float y, float w, float h, bool dark = false, bool border = true)
        {
            var texture = Resources.Load<Texture2D>("HalcyonArtV3/ui-frame");
            if (paintedFrame == null && texture != null)
                paintedFrame = Sprite.Create(texture, new UnityEngine.Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f), 100, 0, SpriteMeshType.FullRect, new Vector4(245, 215, 245, 215));
            Panel(p, x + 2, y + 5, w, h, new Color(.04f, .03f, .09f, .4f));
            var r = Rect(name, p, x, y, w, h);
            if (h >= 90 && paintedFrame != null)
            {
                var image = r.gameObject.AddComponent<Image>(); image.sprite = paintedFrame;
                image.type = Image.Type.Sliced; image.pixelsPerUnitMultiplier = 5;
                image.raycastTarget = false;
            }
            else
            {
                var image = r.gameObject.AddComponent<RawImage>(); image.texture = texture;
                image.uvRect = new UnityEngine.Rect(.3f, .3f, .4f, .4f);
                image.color = dark ? new Color(1.35f, 1.25f, 1.13f) : Color.white; image.raycastTarget = false;
                Hairline(r, 0, h - 1, w, h - 1, ruleInk);
            }
            return r;
        }

        TMP_Text Heading(Transform p, string value, float x, float y, float w, float h, float size, Color color)
        {
            var t = BookText(p, value, x, y, w, h, size, color);
            if (titleFont != null) t.font = titleFont;
            return t;
        }

        void PrinterFlower(Transform p, float x, float y, Color c, float size = 30)
        {
            // A small botanical printer's mark, echoing the wrought-iron flowers in the artwork.
            for (int i = 0; i < 4; i++)
            {
                var petal = Ornament(p, "Printer's petal", x + size / 2, y + size / 2,
                    size * .2f, size * .46f, HalcyonOrnament.Shape.Disc, c).rectTransform;
                petal.pivot = new Vector2(.5f, 0);
                petal.anchoredPosition = new Vector2(x + size / 2, -(y + size / 2));
                petal.localEulerAngles = new Vector3(0, 0, 45 + i * 90);
            }
            Ornament(p, "Printer's center", x + size * .43f, y + size * .43f,
                size * .14f, size * .14f, HalcyonOrnament.Shape.Disc, c);
        }

        void RunningTitle(Transform p, string titleText, string subtitle, float x, float y, float w)
        {
            Heading(p, titleText, x + 1, y + 2, w, 78, 46, new Color(.12f, .07f, .10f, .7f));
            Heading(p, titleText, x, y, w, 78, 46, cream);
            BookText(p, subtitle, x + 3, y + 78, w, 31, 23, cream);
            Hairline(p, x + 3, y + 119, x + 95, y + 119, brass);
            PrinterFlower(p, x + 105, y + 108, brass, 22);
        }

        Button InkLink(Transform p, string label, float x, float y, float w, Action action, bool onPaper = false)
        {
            var r = Rect(label, p, x, y, w, 45);
            var hit = r.gameObject.AddComponent<Image>(); hit.color = Color.white;
            var b = r.gameObject.AddComponent<Button>(); b.targetGraphic = hit;
            var colors = b.colors; colors.normalColor = Color.clear;
            colors.highlightedColor = colors.selectedColor = onPaper ? new Color(.35f, .16f, .23f, .1f) : new Color(1, .86f, .63f, .12f);
            colors.pressedColor = new Color(.15f, .07f, .12f, .24f); b.colors = colors;
            BookText(r, label, 4, 3, w - 8, 36, 25, onPaper ? plum : cream, true, TextAlignmentOptions.Center);
            Hairline(r, 10, 43, w - 10, 43, onPaper ? ruleInk : brass);
            var focus = Ornament(r, "Link focus", 2, 2, w - 4, 41, HalcyonOrnament.Shape.CutOutline, onPaper ? plum : cream);
            focus.inset = 0; focus.gameObject.SetActive(false);
            r.gameObject.AddComponent<HalcyonMapFocus>().outline = focus.gameObject;
            b.onClick.AddListener(() => { if (!muted) clicks.PlayOneShot(clickClip, .07f); action(); });
            buttons.Add(b); return b;
        }

        Button StationeryButton(Transform p, string label, float x, float y, float w, float h,
            Action action, bool enabled, bool primary, float size, bool centered = false)
        {
            var r = Stationery(p, label, x, y, w, h, primary);
            var hit = Panel(r, 0, 0, w, h, Color.white); hit.raycastTarget = true;
            var b = r.gameObject.AddComponent<Button>(); b.targetGraphic = hit; b.interactable = enabled;
            var colors = b.colors;
            colors.normalColor = Color.clear;
            colors.highlightedColor = primary ? new Color(1, .86f, .63f, .12f) : new Color(.35f, .16f, .23f, .10f);
            colors.selectedColor = colors.highlightedColor;
            colors.pressedColor = new Color(.15f, .07f, .12f, .24f);
            colors.disabledColor = new Color(.45f, .38f, .29f, .14f);
            b.colors = colors;
            var caption = BookText(r, label, centered ? 8 : 19, 3, w - (centered ? 16 : 38), h - 6,
                size, primary ? cream : enabled ? plum : fadedInk, true,
                centered ? TextAlignmentOptions.Center : TextAlignmentOptions.MidlineLeft);
            caption.lineSpacing = 0;
            var focus = Ornament(r, "Keyboard and pointer focus", 3, 3, w - 6, h - 6,
                HalcyonOrnament.Shape.CutOutline, primary ? cream : plum);
            focus.inset = 0; focus.gameObject.SetActive(false);
            r.gameObject.AddComponent<HalcyonMapFocus>().outline = focus.gameObject;
            b.onClick.AddListener(() => { if (!muted) clicks.PlayOneShot(clickClip, .07f); action(); });
            buttons.Add(b); return b;
        }
    }

    // Static vertex color and tiny fibers give the paper a material without bitmap assets
    // or per-frame noise. At most 2,500 vertices per sheet, rebuilt only on UI changes.
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class HalcyonPaperGraphic : MaskableGraphic
    {
        protected override void Awake() { base.Awake(); useLegacyMeshGeneration = false; }
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear(); var r = rectTransform.rect;
            int columns = Mathf.Clamp(Mathf.CeilToInt(r.width / 55), 2, 30);
            int rows = Mathf.Clamp(Mathf.CeilToInt(r.height / 55), 2, 18);
            var rng = new System.Random(1709);
            for (int y = 0; y <= rows; y++) for (int x = 0; x <= columns; x++)
            {
                float shade = 1 - (float)rng.NextDouble() * .026f - Mathf.Pow(Mathf.Abs(x / (float)columns - .5f) * 2, 4) * .025f;
                vh.AddVert(new Vector3(Mathf.Lerp(r.xMin, r.xMax, x / (float)columns), Mathf.Lerp(r.yMin, r.yMax, y / (float)rows)),
                    new Color(color.r * shade, color.g * shade, color.b * shade, color.a), Vector2.zero);
            }
            for (int y = 0; y < rows; y++) for (int x = 0; x < columns; x++)
            {
                int i = y * (columns + 1) + x;
                vh.AddTriangle(i, i + columns + 1, i + 1);
                vh.AddTriangle(i + 1, i + columns + 1, i + columns + 2);
            }
            int fibers = Mathf.Clamp((int)(r.width * r.height / 1800), 3, 250);
            for (int i = 0; i < fibers; i++)
            {
                float x = Mathf.Lerp(r.xMin + 1, r.xMax - 5, (float)rng.NextDouble());
                float y = Mathf.Lerp(r.yMin + 1, r.yMax - 1, (float)rng.NextDouble());
                float w = 1 + (float)rng.NextDouble() * 3;
                Color fiber = new Color(color.r * .78f, color.g * .78f, color.b * .78f, color.a * .18f);
                int at = vh.currentVertCount;
                vh.AddVert(new Vector3(x, y), fiber, Vector2.zero); vh.AddVert(new Vector3(x, y + .45f), fiber, Vector2.zero);
                vh.AddVert(new Vector3(x + w, y + .6f), fiber, Vector2.zero); vh.AddVert(new Vector3(x + w, y), fiber, Vector2.zero);
                vh.AddTriangle(at, at + 1, at + 2); vh.AddTriangle(at, at + 2, at + 3);
            }
        }
    }
}
