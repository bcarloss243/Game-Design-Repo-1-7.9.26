using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Halcyon.FirstWeather
{
    public partial class HalcyonSlice
    {
        PressureGaugeFeel gaugeFeel;
        HalcyonSoundscape soundscape;
        TMP_InputField journalInput;

        void DrawEvening()
        {
            Picture(screen, "dorm", 0, 0, 1600, 900); AddSceneLife(screen, "dorm"); Header("DAY 01  /  EVENING");
            Stationery(screen, "Evening correspondence", 920, 124, 628, 716);
            Heading(screen, "Before the lights dim", 958, 166, 549, 84, 34, plum);
            BookText(screen, "The canal is still busy. Tomorrow's bag is beside the door.", 960, 264, 537, 91, 30, cream);
            Button(screen, state.calledHome ? "Call home · finished" : "Call home", 960, 390, 537, 60,
                () => StartStory("callhome"), !state.calledHome, false, 29);
            Button(screen, state.messagedJules ? "Jules · replied" : "Message Jules", 960, 467, 537, 60,
                () => StartStory("messagejules"), !state.messagedJules, false, 29);
            Button(screen, "Write in the journal", 960, 544, 537, 60, () => OpenModal("journal"), true, false, 29);
            Button(screen, "Turn in for the night  →", 960, 721, 537, 60, () => StartStory("night"), true, true, 28);
            DrawInstrument(screen, 39, 584, 277);
        }

        void DrawJournalContents(Transform parent)
        {
            ScrollText(parent, string.Join("\n\n", state.journal.ToArray()), 364, 245, 850, 235);
            BookText(parent, "A page of your own", 364, 498, 850, 42, 30, plum);
            var box = Stationery(parent, "Private journal entry", 364, 550, 852, 136);
            var hit = box.GetComponent<Graphic>(); hit.raycastTarget = true;
            var viewport = Rect("Writing viewport", box, 17, 12, 818, 112);
            viewport.gameObject.AddComponent<RectMask2D>();
            var placeholder = BookText(viewport, "Write here…", 0, 0, 810, 110, 26, fadedInk);
            var entry = BookText(viewport, "", 0, 0, 810, 110, 26, cream);
            entry.richText = false;
            journalInput = box.gameObject.AddComponent<TMP_InputField>();
            journalInput.textViewport = viewport; journalInput.textComponent = entry;
            journalInput.placeholder = placeholder; journalInput.targetGraphic = hit;
            journalInput.lineType = TMP_InputField.LineType.MultiLineNewline;
            journalInput.characterLimit = 1600; journalInput.richText = false;
            journalInput.restoreOriginalTextOnEscape = false;
            journalInput.customCaretColor = true; journalInput.caretColor = plum;
            journalInput.text = state.personalEntry;
            journalInput.onValueChanged.AddListener(value => state.personalEntry = value);
            journalInput.onEndEdit.AddListener(_ => Save());
            Button(parent, "Close journal", 364, 709, 852, 54, () => { Save(); CloseModal(); }, true, true, 26);
        }

        void DrawGardenPresentation()
        {
            Picture(screen, "greenhouse", 0, 0, 1600, 900); AddSceneLife(screen, "greenhouse"); Header("DAY 02  /  LOLA'S WORKBENCH");
            Heading(screen, "The floating garden", 52, 116, 1020, 76, 43, plum);
            BookText(screen, state.puzzleRound == 0 ? "Six measures. Three beds. More need than water." : "The shade has moved. Choose what you can keep growing.", 56, 196, 1000, 73, 31, cream);
            DrawPortrait(screen, "lola", 1340, 107, 187, 187, true);
            Stationery(screen, "Garden field notes", 38, 377, 1524, 505);
            string[] names = { "Kitchen herbs", "Flowers", "Seedlings" };
            string[] roots = state.puzzleRound == 0 ? new[] {
                "Lola finds heat in the curled leaves. These feed a kitchen.",
                "The stems pull at her fingers. Bees will come if they flower.",
                "Small roots, close to the surface. These are next season's beds."
            } : new[] {
                "Shade has reached the herbs. They can hold with a smaller share.",
                "The flowers are hot now. Lola keeps her hand beneath their leaves.",
                "The little roots are still searching. They have no deep reserve."
            };
            for (int i = 0; i < 3; i++)
            {
                int n = i; float x = 68 + i * 496;
                if (i > 0) Hairline(screen, x - 17, 417, x - 17, 667, ruleInk);
                BookText(screen, names[i], x + 5, 421, 453, 48, 34, plum);
                BookText(screen, roots[i], x + 5, 486, 441, 95, largeText ? 29 : 27, cream);
                Button(screen, "−", x + 5, 591, 63, 56, () => { state.AdjustWater(n, -1); gardenMessage = ""; Save(); Render(); }, state.water[i] > 0, false, 29);
                BookText(screen, state.water[i] + " measures", x + 87, 601, 245, 40, 31, cream, true, TextAlignmentOptions.Center);
                Button(screen, "+", x + 376, 591, 63, 56, () => { state.AdjustWater(n, 1); gardenMessage = ""; Save(); Render(); }, state.WaterUsed < 6 && state.water[i] < 4, true, 29);
            }
            BookText(screen, "Reservoir · " + (6 - state.WaterUsed) + " measures left", 73, 690, 670, 43, 29, plum);
            string changed = state.puzzleRound == 1 && state.water[2] != state.firstWater[2] ? "The young roots have a different share now. " : "";
            string note = state.mentor == "circulation" ? "Marchand: keep every bed alive and protect the kitchen harvest." : "Thibodaux: keep every bed alive and protect next season’s young roots.";
            BookText(screen, gardenMessage == "" ? changed + note : gardenMessage, 73, 744, 1450, 43, 25, cream);
            Button(screen, "Ask Lola what she feels", 73, 811, 390, 48, () => {
                state.assisted = true;
                gardenMessage = state.mentor == "circulation" ? "LOLA: “Keep a little flowing to every bed. The kitchen needs more than a taste; the young roots can wait if we return.”" : state.puzzleRound == 0 ? "LOLA: “The small roots can't wait. The others can lose some growth. Which loss can we come back for?”" : "LOLA: “The flowers want more. Wanting and surviving aren't the same. Leave something for the young roots.”";
                Save(); Render();
            }, true, false, 25);
            Button(screen, "Leave a note for tomorrow", 483, 811, 432, 48, () => OpenModal("leavegarden"), true, false, 25);
            Button(screen, state.puzzleRound == 0 ? "Try this allocation  →" : "Commit to this garden  →", 1060, 811, 457, 48, () => {
                if (!state.GardenReady) { gardenMessage = "There is still water in the reservoir. Distribute it, or leave a note for tomorrow."; Render(); }
                else if (state.puzzleRound == 0) { state.GardenShift(); gardenMessage = "Lola moves a hand through the new patch of sun. “It's the flowers now.”"; Save(); Render(); }
                else FinishGarden(true);
            }, true, true, 26);
        }

        void DrawEndingPresentation()
        {
            Picture(screen, "map", 0, 0, 1600, 900); AddSceneLife(screen, "map");
            Stationery(screen, "Lola's first dispatch", 795, 80, 744, 754);
            Heading(screen, "First Weather", 839, 118, 650, 65, 41, plum);
            BookText(screen, "Complete · " + (state.solved ? "Garden Restored" : "Work in Progress"), 842, 198, 647, 39, 27, cream);
            DrawPortrait(screen, "lola", 844, 273, 124, 124, true);
            BookText(screen, "Lola Landry", 994, 285, 445, 41, 33, plum);
            BookText(screen, "A new message", 996, 334, 445, 32, 23, fadedInk);
            string message = state.invitation == "tea" ? "“Tea first. I've put two cups out. The side landing, when you're ready.”" : "“I've found a place for the second stool. Side landing tomorrow?”";
            BookText(screen, message, 848, 438, 635, 130, largeText ? 36 : 33, cream);
            string detail = state.recovery ? "Thibodaux's reduced timetable is folded in the same pocket." : state.academicWarning ? "The revision appointment is folded in the same pocket." : "Marchand has kept the diagram. Lola has kept a cup.";
            BookText(screen, detail, 848, 586, 635, 86, 27, fadedInk);
            Button(screen, "Open journal", 848, 733, 301, 54, () => OpenModal("journal"), true, false, 27);
            Button(screen, "Return to title", 1170, 733, 313, 54, () => { title = true; Render(); }, true, true, 27);
            DrawInstrument(screen, 73, 537, 305);
            BookText(screen, "The greenhouse has a name now.", 59, 841, 690, 38, 29, cream);
        }

        void DrawSpeakerPortrait(StoryPage page)
        {
            if (page.portrait == "") return;
            DrawPortrait(screen, page.portrait, 1320, 250, 223, 223, false);
        }
        void DrawPortrait(Transform parent, string who, float x, float y, float w, float h, bool round)
        {
            var r = Rect(who + " painted portrait", parent, x, y, w, h);
            var portrait = r.gameObject.AddComponent<HalcyonPortraitGraphic>();
            portrait.texture = Resources.Load<Texture2D>("HalcyonArtV3/portraits");
            portrait.column = who == "lola" ? 1 : who == "alaric" ? 2 : 0;
            portrait.round = round; portrait.raycastTarget = false; portrait.SetAllDirty();
            if (!round) { Hairline(parent, x, y + h, x + w, y + h, brass, 2); }
        }
        void DrawComicGutter(StoryPage page)
        {
            if (page.panel != "fracture" && page.panel != "threshold") return;
            // A quiet sequence of held frames. No flashing, auto-advance or forced breathing.
            float[] widths = page.panel == "fracture" && state.panic ? new[] { 312f, 617f, 1060f } : new[] { 500f, 1080f };
            foreach (float x in widths) Panel(screen, x, 155, 18, 375, ink);
            if (page.panel == "fracture" && state.panic) Panel(screen, 0, 155, 1600, 375, new Color(.12f, .08f, .22f, .21f));
        }
        void AddSceneLife(Transform parent, string place)
        {
            var r = Rect("Ambient painted light", parent, 0, 0, 1600, 900);
            var life = r.gameObject.AddComponent<HalcyonSceneLife>();
            life.Initialize(place, () => modal || title, () => quietMotion);
        }
    }

    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class HalcyonPortraitGraphic : MaskableGraphic
    {
        protected override void Awake() { base.Awake(); useLegacyMeshGeneration = false; }
        public Texture2D texture;
        public int column;
        public bool round;
        public override Texture mainTexture => texture != null ? texture : Texture2D.whiteTexture;
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear(); var r = rectTransform.rect;
            if (!round)
            {
                vh.AddVert(new Vector2(r.xMin, r.yMin), color, new Vector2(column / 3f, 0));
                vh.AddVert(new Vector2(r.xMin, r.yMax), color, new Vector2(column / 3f, 1));
                vh.AddVert(new Vector2(r.xMax, r.yMax), color, new Vector2((column + 1) / 3f, 1));
                vh.AddVert(new Vector2(r.xMax, r.yMin), color, new Vector2((column + 1) / 3f, 0));
                vh.AddTriangle(0, 1, 2); vh.AddTriangle(0, 2, 3); return;
            }
            vh.AddVert(r.center, color, new Vector2((column + .51f) / 3, .58f));
            for (int i = 0; i <= 64; i++)
            {
                float a = i * Mathf.PI * 2 / 64;
                Vector2 d = new Vector2(Mathf.Cos(a), Mathf.Sin(a));
                vh.AddVert(r.center + new Vector2(d.x * r.width / 2, d.y * r.height / 2), color,
                    new Vector2((column + .51f + d.x * .33f) / 3, .58f + d.y * .33f));
                if (i > 0) vh.AddTriangle(0, i, i + 1);
            }
        }
    }
}
