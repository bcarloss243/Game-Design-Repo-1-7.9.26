using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using TMPro;

namespace Halcyon.FirstWeather
{
    public partial class HalcyonSlice : MonoBehaviour
    {
        const string SaveKey = "Halcyon.FirstWeather.Save.v1";
        public SliceState state = new SliceState();
        Canvas canvas;
        RectTransform root, screen, overlay, needle;
        TMP_FontAsset font;
        AudioSource music, ambience, clicks;
        AudioClip clickClip;
        TMP_Text gaugeValue;
        readonly List<Button> buttons = new List<Button>();
        readonly Dictionary<string, Texture2D> art = new Dictionary<string, Texture2D>();
        bool title = true, modal, largeText, quietMotion, muted;
        #if UNITY_EDITOR
        bool interfacePreview;
        public void PreviewMapInterface(string phase = "arrival", bool? biggerText = null, int pressure = 54)
        {
            if (modal) CloseModal();
            interfacePreview = true; title = false;
            state = new SliceState { phase = phase, pressure = Mathf.Clamp(pressure, 0, 100), day = phase == "morning2" || phase == "class2" || phase == "corridor" || phase == "homecoming" ? 2 : 1 };
            if (biggerText.HasValue) largeText = biggerText.Value;
            displayedPressure = state.pressure;
            Render();
        }
        #endif
        string modalKind = "", gardenMessage = "";
        int noticePage;
        float displayedPressure, fade = 1, lastAction = -10;
        CanvasGroup screenFade;
        ScrollRect modalScroll;
        Color ink = new Color32(24, 36, 41, 255), cream = new Color32(242, 229, 205, 255);
        Color brass = new Color32(204, 170, 111, 255), mint = new Color32(173, 211, 193, 255);
        Color mutedInk = new Color32(80, 93, 94, 255);

        void Awake()
        {
            Application.targetFrameRate = 60;
            font = TMP_Settings.defaultFontAsset;
            foreach (string key in new[] { "map", "dorm", "academy", "greenhouse", "cover" })
                art[key] = Resources.Load<Texture2D>("HalcyonArt/" + key);
            largeText = PlayerPrefs.GetInt("Halcyon.LargeText", 0) == 1;
            quietMotion = PlayerPrefs.GetInt("Halcyon.ReducedMotion", 0) == 1;
            muted = PlayerPrefs.GetInt("Halcyon.Muted", 0) == 1;
            var go = new GameObject("First Weather Canvas", typeof(RectTransform), typeof(Canvas), typeof(GraphicRaycaster));
            canvas = go.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            root = Rect("Safe area", go.transform, 0, 0, 1600, 900);
            root.anchorMin = root.anchorMax = new Vector2(.5f, .5f); root.pivot = new Vector2(.5f, .5f); root.anchoredPosition = Vector2.zero;
            if (FindFirstObjectByType<EventSystem>() == null)
            {
                var ev = new GameObject("First Weather Input", typeof(EventSystem), typeof(InputSystemUIInputModule));
                ev.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
            }
            SetupAudio(); displayedPressure = state.pressure;
            Render();
        }

        void Update()
        {
            canvas.scaleFactor = Mathf.Min(Screen.width / 1600f, Screen.height / 900f);
            if (!title && !modal && !state.finished) state.playSeconds += Time.unscaledDeltaTime;
            displayedPressure = quietMotion ? state.pressure : Mathf.Lerp(displayedPressure, state.pressure, Time.unscaledDeltaTime * 3);
            if (needle != null)
                needle.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(120, -120, displayedPressure / 100f) + (quietMotion || modal ? 0 : Mathf.Sin(Time.unscaledTime * 2.5f) * displayedPressure / 130f));
            if (gaugeValue != null) gaugeValue.text = Mathf.RoundToInt(displayedPressure) + " / 100";
            if (screenFade != null) { fade = quietMotion ? 1 : Mathf.MoveTowards(fade, 1, Time.unscaledDeltaTime * 4); screenFade.alpha = fade; }
            float level = muted ? 0 : modal ? .025f : .055f;
            music.volume = Mathf.Lerp(music.volume, level, Time.unscaledDeltaTime * 3);
            float water = state.story == "greenhouse" || state.story == "aftergarden" || state.view == "garden" ? .08f : .03f;
            ambience.volume = Mathf.Lerp(ambience.volume, muted ? 0 : modal ? .01f : water, Time.unscaledDeltaTime * 2);
            var kb = Keyboard.current;
            if (kb == null) return;
            if (kb.escapeKey.wasPressedThisFrame) { if (modal) CloseModal(); else OpenModal(title ? "help" : "pause"); }
            if (kb.mKey.wasPressedThisFrame) { muted = !muted; SaveSettings(); if (modalKind == "settings") OpenModal("settings"); }
            if (kb.jKey.wasPressedThisFrame && !title) { if (modal) CloseModal(); else OpenModal("journal"); }
            if (kb.tabKey.wasPressedThisFrame) FocusNext(kb.shiftKey.isPressed ? -1 : 1);
            if (modal && modalScroll != null)
            {
                float direction = (kb.upArrowKey.isPressed ? 1 : 0) - (kb.downArrowKey.isPressed ? 1 : 0);
                modalScroll.verticalNormalizedPosition = Mathf.Clamp01(modalScroll.verticalNormalizedPosition + direction * Time.unscaledDeltaTime * .5f);
            }
            if (kb.spaceKey.wasPressedThisFrame && !modal && state.view == "story" && !title)
            {
                var pages = SliceStory.Get(state.story, state);
                if (state.page < pages.Count && pages[state.page].choices.Length == 0) Advance("");
            }
        }
        void OnApplicationQuit() { if (!title) Save(); }
        void OnApplicationFocus(bool focused) { if (!focused && !title && !modal && !state.finished) OpenModal("pause"); }
        void OnDestroy()
        {
            if (music != null && music.clip != null) Destroy(music.clip);
            if (ambience != null && ambience.clip != null) Destroy(ambience.clip);
            if (clickClip != null) Destroy(clickClip);
            if (canvas != null) Destroy(canvas.gameObject);
            ReleasePresentation();
        }
        public void Save()
        {
            #if UNITY_EDITOR
            if (interfacePreview) return;
            #endif
            PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(state)); PlayerPrefs.Save();
        }
        void SaveSettings()
        {
            PlayerPrefs.SetInt("Halcyon.LargeText", largeText ? 1 : 0);
            PlayerPrefs.SetInt("Halcyon.ReducedMotion", quietMotion ? 1 : 0);
            PlayerPrefs.SetInt("Halcyon.Muted", muted ? 1 : 0); PlayerPrefs.Save();
        }
        bool Load()
        {
            try
            {
                var loaded = JsonUtility.FromJson<SliceState>(PlayerPrefs.GetString(SaveKey, ""));
                if (loaded == null || !loaded.Validate()) return false;
                state = loaded; return true;
            }
            catch { return false; }
        }
        public void NewGame()
        {
            #if UNITY_EDITOR
            interfacePreview = false;
            #endif
            title = false; modal = false; state = new SliceState();
            state.Note("Arrived in Halcyon. Begin at Molly's residence; morning routines open the city.");
            Save(); Render();
        }
        void ResumeGame()
        {
            #if UNITY_EDITOR
            interfacePreview = false;
            #endif
            if (Load()) { title = false; displayedPressure = state.pressure; Render(); } else NewGame();
        }
        public void StartStory(string key)
        {
            state.view = "story"; state.story = key; state.page = 0; Save(); Render();
        }
        public void Advance(string effect)
        {
            if (Time.unscaledTime - lastAction < .15f) return;
            lastAction = Time.unscaledTime;
            if (effect.StartsWith("decor:")) { state.decor = effect.Substring(6); state.Pressure(-3); state.Note("Made the dorm your own with a " + state.decor + ". Pressure eased by 3."); }
            if (effect.StartsWith("mentor:")) { state.mentor = effect.Substring(7); state.Note("Chose " + state.mentor + " as the first mentor track."); }
            if (effect == "routine") { state.routineDays++; state.Pressure(-6); state.Note("Day " + state.day + ": the familiar routine eased Pressure by 6. No story paths were closed."); }
            if (effect == "panic" || effect == "support")
            {
                state.Pressure(state.rested ? 24 : 32);
                if (effect == "support") state.Pressure(-4);
                state.Note("Class was interrupted by a panic attack. Asking for space or support opened a route to the greenhouse.");
            }
            if (effect == "share") { state.shared = true; state.Pressure(-5); state.Note("Told Lola that class was difficult. She offered company without requiring an explanation."); }
            if (effect == "tea" || effect == "return") state.Note(effect == "tea" ? "Made a plan to start with tea tomorrow." : "Accepted Lola's invitation to return tomorrow.");
            state.page++;
            if (state.page >= SliceStory.Get(state.story, state).Count) EndStory(); else { Save(); Render(); }
        }
        void EndStory()
        {
            string key = state.story;
            state.view = "map"; state.story = ""; state.page = 0;
            if (key == "morning1") state.phase = "class1";
            if (key == "class1") state.phase = "afternoon";
            if (key == "night") state.Sleep();
            if (key == "morning2") state.phase = "class2";
            if (key == "class2") { state.phase = "corridor"; state.Pressure(-7); }
            if (key == "greenhouse") { state.view = "garden"; state.Pressure(-12); }
            if (key == "aftergarden") { state.phase = "homecoming"; state.Note("Discovered the greenhouse: a place to return to. The two-day story is complete."); }
            Save(); Render();
        }
        void DoActivity(string key) { if (state.Activity(key)) StartStory(key); }
        void FinishGarden(bool success)
        {
            state.solved = success; state.Pressure(success ? -8 : -4);
            state.Note(success ? "Balanced the floating garden through both light conditions." : "Left the prototype safe with a note for another trial. The invitation remains open.");
            StartStory("aftergarden");
        }

        RectTransform Rect(string name, Transform parent, float x, float y, float w, float h)
        {
            var r = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            r.SetParent(parent, false); r.anchorMin = r.anchorMax = new Vector2(0, 1); r.pivot = new Vector2(0, 1);
            r.anchoredPosition = new Vector2(x, -y); r.sizeDelta = new Vector2(w, h); return r;
        }
        Image Panel(Transform p, float x, float y, float w, float h, Color c)
        {
            var r = Rect("Panel", p, x, y, w, h); var im = r.gameObject.AddComponent<Image>(); im.color = c; im.raycastTarget = false; return im;
        }
        TMP_Text Text(Transform p, string value, float x, float y, float w, float h, float size, Color c, bool bold = false, TextAlignmentOptions align = TextAlignmentOptions.TopLeft)
        {
            var t = Rect("Text: " + (value.Length > 35 ? value.Substring(0, 35) : value), p, x, y, w, h).gameObject.AddComponent<TextMeshProUGUI>();
            t.font = font; t.text = value; t.fontSize = size; t.color = c; t.fontStyle = bold ? FontStyles.Bold : FontStyles.Normal;
            t.alignment = align; t.textWrappingMode = TextWrappingModes.Normal; t.overflowMode = TextOverflowModes.Overflow;
            t.raycastTarget = false; t.lineSpacing = 5; return t;
        }
        Button Button(Transform p, string label, float x, float y, float w, float h, Action action, bool enabled = true, bool primary = false, float size = 22)
        {
            var r = Rect(label, p, x, y, w, h); var im = r.gameObject.AddComponent<Image>();
            im.color = primary ? mint : new Color32(45, 61, 64, 255);
            var b = r.gameObject.AddComponent<Button>(); b.targetGraphic = im; b.interactable = enabled;
            var colors = b.colors; colors.normalColor = Color.white; colors.highlightedColor = new Color(1.2f, 1.2f, 1.15f);
            colors.selectedColor = new Color(1.28f, 1.24f, 1.1f); colors.pressedColor = new Color(.8f, .85f, .8f); colors.disabledColor = new Color(.45f, .48f, .5f); b.colors = colors;
            Text(r, label, 16, 4, w - 32, h - 8, size, primary ? ink : cream, false, TextAlignmentOptions.MidlineLeft);
            Panel(r, 0, h - 2, w, 2, enabled ? brass : mutedInk);
            b.onClick.AddListener(() => { if (muted == false) clicks.PlayOneShot(clickClip, .07f); action(); });
            buttons.Add(b); return b;
        }
        void Picture(Transform p, string key, float x, float y, float w, float h, Color? tint = null)
        {
            if (!art.ContainsKey(key) || art[key] == null) { Panel(p, x, y, w, h, mutedInk); return; }
            var im = Rect("Illustration: " + key, p, x, y, w, h).gameObject.AddComponent<RawImage>();
            im.texture = art[key]; im.color = tint ?? Color.white; im.raycastTarget = false;
            float original = (float)art[key].width / art[key].height, target = w / h;
            im.uvRect = target > original ? new UnityEngine.Rect(0, (1 - original / target) / 2, 1, original / target) : new UnityEngine.Rect((1 - target / original) / 2, 0, target / original, 1);
        }
        void ClearScreen()
        {
            if (screen != null) { screen.gameObject.SetActive(false); Destroy(screen.gameObject); }
            buttons.Clear(); needle = null; gaugeValue = null;
            screen = Rect("Current screen", root, 0, 0, 1600, 900);
            screenFade = screen.gameObject.AddComponent<CanvasGroup>(); fade = .8f;
        }
        public void Render()
        {
            ClearScreen();
            if (title) { DrawTitle(); return; }
            if (state.view == "story") DrawStory();
            else if (state.view == "garden") DrawGarden();
            else if (state.view == "ending") DrawEnding();
            else DrawMap();
            if (modal) OpenModal(modalKind);
        }
        void Header(string right)
        {
            Panel(screen, 0, 0, 1600, 76, ink); Panel(screen, 0, 74, 1600, 2, brass);
            Text(screen, "HALCYON ACADEMY", 34, 21, 500, 40, 26, cream, true);
            Text(screen, right, 480, 24, 580, 34, 20, mint);
            Button(screen, "Journal  [J]", 1140, 13, 190, 49, () => OpenModal("journal"), true, false, 19);
            Button(screen, "Pause  [Esc]", 1350, 13, 210, 49, () => OpenModal("pause"), true, false, 19);
        }
        void DrawTitle()
        {
            Panel(screen, 0, 0, 1600, 900, ink);
            Picture(screen, "cover", 740, 0, 860, 900);
            Panel(screen, 740, 0, 3, 900, brass);
            Text(screen, "AN ILLUSTRATED STORY OF HALCYON", 70, 82, 630, 40, 18, brass, true);
            Text(screen, "HALCYON\nACADEMY", 65, 145, 645, 190, 72, cream, true);
            Text(screen, "FIRST WEATHER", 72, 353, 590, 52, 29, mint);
            Panel(screen, 72, 426, 95, 3, brass);
            Text(screen, "Two mornings. A city to discover.\nA place to return to.", 72, 459, 600, 100, 29, cream);
            Button(screen, "Begin a new story", 72, 598, 555, 62, NewGame, true, true, 26);
            Button(screen, "Continue your story", 72, 675, 555, 55, ResumeGame, PlayerPrefs.HasKey(SaveKey), false, 23);
            Button(screen, "How to play", 72, 755, 173, 45, () => OpenModal("help"), true, false, 18);
            Button(screen, "Settings", 258, 755, 173, 45, () => OpenModal("settings"), true, false, 18);
            Button(screen, "Credits", 444, 755, 183, 45, () => OpenModal("credits"), true, false, 18);
            Text(screen, "A capstone prototype by Bergen Carloss  ·  v0.2", 72, 838, 590, 28, 16, brass);
            Button(screen, "Quit", 1468, 835, 95, 42, Quit, true, false, 18);
        }
        string TimeLabel()
        {
            if (state.phase == "afternoon") return "DAY 01  /  " + (state.slots == 2 ? "14:00" : state.slots == 1 ? "15:00" : "EVENING");
            return "DAY " + state.day.ToString("00") + "  /  " + (state.phase == "arrival" || state.phase == "morning2" ? "08:00" : state.phase == "corridor" ? "11:20" : state.phase == "homecoming" ? "AFTERNOON" : "09:00");
        }
        string Objective()
        {
            switch (state.phase)
            {
                case "arrival": return "Begin at your residence.\nMake a small space your own.";
                case "class1": return "Attend the inaugural class.\nChoose your first mentor.";
                case "afternoon": return state.slots > 0 ? "Choose " + state.slots + " afternoon " + (state.slots == 1 ? "activity" : "activities") + ".\nWhat will you bring into tomorrow?" : "Your afternoon is complete.\nReturn to the residence to sleep.";
                case "morning2": return "Begin at your residence.\nYesterday came with you.";
                case "class2": return "Return to class.\nThe assignment is due this morning.";
                case "corridor": return "Find the greenhouse.\nFor now, class can wait.";
                case "homecoming": return "A place has become a possibility.\nSelect the greenhouse to finish.";
                default: return "Explore your first days in Halcyon.";
            }
        }
        void DrawMap() { DrawMapPresentation(); }
        void DrawGauge(Transform parent, float x, float y) { DrawInstrument(parent, x - 12, y - 16, 252); }
        void DrawStory()
        {
            var pages = SliceStory.Get(state.story, state);
            state.page = Mathf.Clamp(state.page, 0, pages.Count - 1);
            var page = pages[state.page];
            Picture(screen, SliceStory.Background(state.story), 0, 35, 1600, 900, state.story == "night" ? new Color32(145, 165, 200, 255) : Color.white);
            Header(TimeLabel());
            Panel(screen, 38, 104, 445, 50, new Color(ink.r, ink.g, ink.b, .91f));
            Text(screen, Chapter(), 58, 116, 405, 34, 21, cream);
            Panel(screen, 0, 541, 1600, 359, ink); Panel(screen, 0, 541, 1600, 2, brass);
            DrawGauge(screen, 44, 614);
            Text(screen, page.speaker, 334, 568, 1050, 36, 20, brass, true);
            Text(screen, page.text, 334, 613, 1220, 203, largeText ? 29 : 26, cream);
            Text(screen, (state.page + 1).ToString("00") + " / " + pages.Count.ToString("00"), 1450, 570, 107, 30, 17, mint, false, TextAlignmentOptions.Right);
            if (page.choices.Length > 0)
            {
                float width = (1220f - (page.choices.Length - 1) * 15) / page.choices.Length;
                for (int i = 0; i < page.choices.Length; i++)
                {
                    var c = page.choices[i];
                    Button(screen, c.label, 334 + i * (width + 15), 828, width, 52, () => Advance(c.effect), true, true, page.choices.Length == 3 ? 20 : 21);
                }
            }
            else
            {
                Text(screen, "Read at your own pace.  Space or Continue to turn the page.", 334, 842, 800, 30, 17, mint);
                Button(screen, state.page == pages.Count - 1 ? "Continue  →" : "Next  →", 1292, 828, 262, 52, () => Advance(""), true, true, 22);
            }
        }
        string Chapter()
        {
            switch (state.story)
            {
                case "morning1": return "01  /  A ROOM OF YOUR OWN";
                case "class1": return "02  /  THE FIRST COHORT";
                case "night": return "04  /  THE CITY AT NIGHT";
                case "morning2": return "05  /  WHAT YOU CARRY";
                case "class2": return "06  /  THE UNFINISHED ANSWER";
                case "greenhouse": return "07  /  A QUIETER PLACE";
                case "aftergarden": return "09  /  TOMORROW, PERHAPS";
                default: return "03  /  AN AFTERNOON TO CHOOSE";
            }
        }
        void DrawGarden()
        {
            Picture(screen, "greenhouse", 0, 0, 1600, 900);
            Header("DAY 02  /  LOLA'S WORKSHOP");
            Panel(screen, 47, 112, 1506, 744, new Color(ink.r, ink.g, ink.b, .96f));
            Text(screen, "08  /  THE FLOATING GARDEN", 84, 142, 1200, 40, 20, brass, true);
            Text(screen, state.puzzleRound == 0 ? "Every root needs something different." : "The shade moves. The needs change.", 84, 202, 1400, 67, 42, cream, true);
            Text(screen, state.puzzleRound == 0 ? "Share 6 measures of water among 3 beds. Match each bed's labeled need, then test circulation. Use − to reclaim water and + to redirect it. There is no timer or penalty for experimenting." : "The afternoon sun has moved. The pump still supplies 6 measures, but the beds now need 2, 3, and 1. Reclaim water from a bed with too much, then redirect it. Observe, adapt, test again.", 84, 285, 1380, 90, largeText ? 29 : 26, cream);
            string[] names = { "01  ·  HERBS", "02  ·  FLOWERS", "03  ·  SEEDLINGS" };
            for (int i = 0; i < 3; i++)
            {
                int n = i; float x = 85 + i * 478;
                Panel(screen, x, 411, 440, 213, new Color32(48, 66, 65, 255));
                Text(screen, names[i], x + 24, 433, 390, 33, 21, brass, true);
                Text(screen, "Needs " + state.Target(i) + "  /  Receiving " + state.water[i], x + 24, 484, 390, 39, 29, cream);
                Button(screen, "−", x + 24, 546, 72, 54, () => { state.AdjustWater(n, -1); gardenMessage = ""; Save(); Render(); }, state.water[i] > 0, false, 28);
                Text(screen, state.water[i] == state.Target(i) ? "BALANCED" : state.water[i] > state.Target(i) ? "TOO MUCH" : "NEEDS WATER", x + 108, 563, 216, 30, 18, mint, true, TextAlignmentOptions.Center);
                Button(screen, "+", x + 343, 546, 72, 54, () => { state.AdjustWater(n, 1); gardenMessage = ""; Save(); Render(); }, state.WaterUsed < 6 && state.water[i] < 4, true, 28);
            }
            Text(screen, "RESERVOIR: " + (6 - state.WaterUsed) + " / 6 available", 86, 658, 600, 40, 24, mint, true);
            Text(screen, gardenMessage == "" ? (state.mentor == "ecology" ? "Your mentor's question: can the people using this repair it themselves?" : "Your mentor's note: changing conditions require another observation.") : gardenMessage, 86, 708, 1370, 56, 21, cream);
            Button(screen, "Test circulation", 1115, 773, 394, 57, () => {
                if (!state.GardenReady) { gardenMessage = "The flow is uneven. Match the printed need on each bed; reclaim spare water with −."; Render(); }
                else if (state.puzzleRound == 0) { state.GardenShift(); gardenMessage = "First test passed. Now adapt to the afternoon light."; Save(); Render(); }
                else FinishGarden(true);
            }, true, true, 23);
            Button(screen, "Ask Lola for a hint", 85, 773, 310, 57, () => { state.assisted = true; gardenMessage = "LOLA: 'Try " + state.Target(0) + " for the herbs, " + state.Target(1) + " for the flowers, " + state.Target(2) + " for the seedlings. Taking water back is part of the work.'"; Save(); Render(); }, true, false, 21);
            Button(screen, "Leave a note for tomorrow", 413, 773, 402, 57, () => OpenModal("leavegarden"), true, false, 21);
        }
        void DrawEnding()
        {
            Picture(screen, "greenhouse", 0, 0, 1600, 900);
            Panel(screen, 0, 0, 810, 900, new Color(ink.r, ink.g, ink.b, .98f));
            Text(screen, "HALCYON ACADEMY  /  FIRST WEATHER", 63, 66, 680, 38, 19, brass, true);
            Text(screen, "A place to\nreturn to.", 60, 149, 700, 175, 67, cream, true);
            Text(screen, state.solved ? "STORY COMPLETE  ·  GARDEN RESTORED" : "STORY COMPLETE  ·  WORK IN PROGRESS", 66, 379, 680, 40, 23, mint, true);
            Text(screen, state.solved ? "You adapted the garden to both light conditions.\nMolly and Lola have made something work together." : "The garden needs another trial.\nMolly and Lola will have another afternoon.", 66, 447, 665, 109, 29, cream);
            Text(screen, "Prepared: " + (state.studied ? "yes" : "not this time") + "   /   Rested: " + (state.rested ? "yes" : "not this time") + "\nConnected with Jules: " + (state.socialized ? "yes" : "not this time") + "\nFinal Pressure: " + state.pressure + " · " + state.Zone + "\nTime in play: " + TimeSpan.FromSeconds(state.playSeconds).ToString(@"mm\:ss"), 66, 588, 665, 137, 23, cream);
            Button(screen, "Read your journal", 66, 755, 315, 57, () => OpenModal("journal"), true, true);
            Button(screen, "Return to title", 402, 755, 315, 57, () => { title = true; Render(); }, true, false);
            Text(screen, "The full story continues beyond this prototype.", 66, 844, 680, 30, 18, brass);
        }
        void FocusNext(int direction)
        {
            var candidates = buttons.FindAll(b => b != null && b.IsActive() && b.interactable && (modal ? b.transform.IsChildOf(overlay) : !b.transform.IsChildOf(overlay == null ? transform : overlay)));
            if (candidates.Count == 0) return;
            int at = candidates.FindIndex(b => b.gameObject == EventSystem.current.currentSelectedGameObject);
            at = (at + direction + candidates.Count) % candidates.Count;
            EventSystem.current.SetSelectedGameObject(candidates[at].gameObject);
        }
        void OpenModal(string kind)
        {
            int licensePages = 1;
            if (!title) Save();
            if (overlay != null) { overlay.gameObject.SetActive(false); Destroy(overlay.gameObject); }
            modal = true; modalKind = kind;
            modalScroll = null;
            screen.GetComponent<CanvasGroup>().interactable = false;
            overlay = Rect("Overlay " + kind, root, 0, 0, 1600, 900);
            var blocker = Panel(overlay, 0, 0, 1600, 900, new Color(0, .03f, .04f, .84f)); blocker.raycastTarget = true;
            Panel(overlay, 320, 105, 960, 690, ink); Panel(overlay, 320, 105, 960, 3, brass);
            string heading = kind == "pause" ? "PAUSED" : kind == "help" ? "YOUR FIRST DAYS IN HALCYON" : kind == "settings" ? "MAKE YOURSELF COMFORTABLE" : kind == "journal" ? "MOLLY'S JOURNAL" : kind == "credits" ? "CREDITS & PROVENANCE" : kind == "licenses" ? "LICENSES & NOTICES" : kind == "leavegarden" ? "LEAVE THE GARDEN FOR TOMORROW?" : "BACK AT YOUR ROOM";
            Text(overlay, heading, 364, 145, 850, 70, 31, brass, true);
            if (kind == "pause")
            {
                Text(overlay, "Your place is saved.\nTime in play is stopped while this menu is open.", 364, 245, 850, 104, 28, cream);
                Button(overlay, "Resume story  [Esc]", 364, 409, 852, 59, CloseModal, true, true);
                Button(overlay, "Settings & accessibility", 364, 487, 852, 59, () => OpenModal("settings"));
                Button(overlay, "How to play", 364, 565, 410, 59, () => OpenModal("help"));
                Button(overlay, "Save & title", 804, 565, 412, 59, () => { CloseModal(); title = true; Render(); });
                Button(overlay, "Quit", 364, 680, 852, 59, Quit);
            }
            else if (kind == "settings")
            {
                Button(overlay, "Larger story text: " + (largeText ? "ON" : "OFF"), 364, 251, 852, 61, () => { largeText = !largeText; SaveSettings(); Render(); OpenModal("settings"); });
                Button(overlay, "Reduced motion: " + (quietMotion ? "ON" : "OFF"), 364, 340, 852, 61, () => { quietMotion = !quietMotion; SaveSettings(); OpenModal("settings"); });
                Button(overlay, "Audio: " + (muted ? "MUTED" : "ON") + "   [M]", 364, 429, 852, 61, () => { muted = !muted; SaveSettings(); OpenModal("settings"); });
                Text(overlay, "All information is written as well as colored. There are no timed choices, flashing effects, or audio-only clues. Reduced motion stops gauge tremble and scene fades. Settings persist between visits.", 364, 548, 852, 132, 25, cream);
                Button(overlay, "Return", 364, 709, 852, 54, CloseModal, true, true);
            }
            else if (kind == "residence")
            {
                Text(overlay, "You have " + state.slots + " afternoon " + (state.slots == 1 ? "slot" : "slots") + " left. Rest helps now; ending the day passes the remaining opportunities. Your room contains the " + state.decor + " you chose this morning.", 364, 253, 830, 158, 29, cream);
                Button(overlay, "Rest  ·  −17 Pressure  ·  1 slot", 364, 461, 852, 58, () => { CloseModal(); DoActivity("rest"); }, state.CanActivity("rest"), true);
                Button(overlay, "End the day and sleep", 364, 547, 852, 58, () => { CloseModal(); StartStory("night"); });
                Button(overlay, "Return to the city", 364, 701, 852, 58, CloseModal);
            }
            else if (kind == "leavegarden")
            {
                Text(overlay, "You can keep experimenting with no penalty, or finish the story with the garden still in progress. Both routes preserve Molly and Lola's connection. A restored garden requires balancing both light conditions.", 364, 261, 844, 220, 30, cream);
                Button(overlay, "Keep working together", 364, 530, 852, 60, CloseModal, true, true);
                Button(overlay, "Leave a note and finish the story", 364, 634, 852, 60, () => { CloseModal(); FinishGarden(false); });
            }
            else
            {
                string content = kind == "help" ? "Play as Molly through two short days at the first cross-Ward Academy. Begin each morning at your residence. Follow the map's bright markers. After the first class, choose how to spend two afternoon slots. Your choices change tomorrow.\n\nThe Pressure gauge describes strain: 0–19 Clarity; 20–44 Steady; 45–69 Elevated; 70–100 Overwhelmed. Lower is easier. Pressure never blocks the greenhouse or a relationship.\n\nIn the greenhouse, balance six water measures across three beds, then adapt to a light change. Restore both arrangements for Garden Restored, or leave a note for Work in Progress. Both complete the story.\n\nClick buttons, or use Tab / Shift+Tab and Enter. Space advances a story page without choices. J opens the journal, M mutes audio, Esc pauses. Progress saves after each choice.\n\nContent note: an anxiety/panic episode is described in text. No flashing imagery, forced breathing, or timed responses. Read at your own pace."
                    : kind == "credits" ? "Concept, world, characters & creative direction\nBergen Carloss\n\nPrototype programming, draft scene writing & original synthesized audio\nCreated with OpenAI Codex under Bergen's direction\n\nEnvironment illustrations\nAI-generated with OpenAI image generation, using the supplied character reference as the art direction. These are prototype assets.\n\nTitle illustration\nUser-supplied Halcyon Academy reference. Its original creator and publication rights still need to be recorded before public distribution.\n\nBarometer housing\nGenerated with OpenAI image generation for this UI pass; scale, needle and readings are drawn live in Unity.\n\nDisplay typography\nCormorant Garamond by Christian Thalmann and the Cormorant Project Authors, SIL Open Font License 1.1. The full license is included in Dependency licenses.\n\nTechnology\nUnity 6.3 LTS · Unity UI / TextMesh Pro · Unity Input System. This slice uses Unity audio directly. Existing Ink, DOTween and FMOD packages remain part of the surrounding project; their notices are retained there.\n\nA fictional city inspired by New Orleans. Draft scenes and mentor/classmate names added for this prototype remain open to creative revision.\n\nFull dependency notices accompany the project in ThirdPartyNotices.txt."
                    : state.journal.Count == 0 ? "Your story begins at the residence." : "DAY " + state.day + "  /  " + state.Zone + "\n\n" + string.Join("\n\n", state.journal.ToArray());
                if (kind == "licenses")
                {
                    var notices = Resources.Load<TextAsset>("HalcyonNotices");
                    string all = notices == null ? "See ThirdPartyNotices.txt in the accompanying documentation." : notices.text;
                    licensePages = Mathf.Max(1, (all.Length + 5999) / 6000);
                    noticePage = Mathf.Clamp(noticePage, 0, licensePages - 1);
                    int start = noticePage * 6000;
                    content = "PART " + (noticePage + 1) + " OF " + licensePages + "  ·  SCROLL TO READ\n\n" + all.Substring(start, Mathf.Min(6000, all.Length - start));
                }
                ScrollText(overlay, content, 364, 247, 850, 410);
                if (kind == "credits")
                {
                    Button(overlay, "Dependency licenses", 364, 709, 410, 54, () => { noticePage = 0; OpenModal("licenses"); });
                    Button(overlay, "Return", 796, 709, 420, 54, CloseModal, true, true);
                }
                else if (kind == "licenses")
                {
                    Button(overlay, "Previous part", 364, 709, 256, 54, () => { noticePage--; OpenModal("licenses"); }, noticePage > 0);
                    Button(overlay, "Next part", 638, 709, 256, 54, () => { noticePage++; OpenModal("licenses"); }, noticePage < licensePages - 1);
                    Button(overlay, "Back to credits", 914, 709, 302, 54, () => OpenModal("credits"), true, true);
                }
                else Button(overlay, "Return", 364, 709, 852, 54, CloseModal, true, true);
            }
        }
        void ScrollText(Transform p, string content, float x, float y, float w, float h)
        {
            var viewport = Rect("Scroll viewport", p, x, y, w, h);
            viewport.gameObject.AddComponent<RectMask2D>();
            var bg = viewport.gameObject.AddComponent<Image>(); bg.color = new Color(0, 0, 0, .02f);
            var sr = viewport.gameObject.AddComponent<ScrollRect>(); sr.horizontal = false; sr.vertical = true; sr.scrollSensitivity = 35;
            modalScroll = sr;
            var text = Text(viewport, content, 0, 0, w - 25, 2000, 25, cream);
            text.ForceMeshUpdate(); float height = text.GetPreferredValues(content, w - 25, 0).y + 20;
            text.rectTransform.sizeDelta = new Vector2(w - 25, Mathf.Max(h, height));
            sr.content = text.rectTransform; sr.viewport = viewport; sr.movementType = ScrollRect.MovementType.Clamped;
            var track = Panel(p, x + w - 12, y, 8, h, mutedInk); track.raycastTarget = true;
            var sb = track.gameObject.AddComponent<Scrollbar>(); sb.direction = Scrollbar.Direction.BottomToTop;
            var handle = Panel(track.transform, 0, 0, 8, 50, brass); sb.handleRect = handle.rectTransform; sb.targetGraphic = handle;
            sr.verticalScrollbar = sb;
        }
        void CloseModal()
        {
            modal = false; modalKind = "";
            if (overlay != null) { overlay.gameObject.SetActive(false); Destroy(overlay.gameObject); overlay = null; }
            if (screen != null) screen.GetComponent<CanvasGroup>().interactable = true;
            EventSystem.current.SetSelectedGameObject(null);
        }
        void Quit()
        {
            if (!title) Save();
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
        void SetupAudio()
        {
            music = gameObject.AddComponent<AudioSource>(); ambience = gameObject.AddComponent<AudioSource>(); clicks = gameObject.AddComponent<AudioSource>();
            music.loop = ambience.loop = true; music.volume = ambience.volume = 0;
            const int sr = 22050, seconds = 24;
            float[] song = new float[sr * seconds], water = new float[sr * seconds];
            var random = new System.Random(26); float filtered = 0;
            double[] notes = { 220, 261.6256, 329.6276, 293.6648, 196, 246.9417, 293.6648, 261.6256 };
            for (int i = 0; i < song.Length; i++)
            {
                double t = i / (double)sr, local = t % 3;
                double n = notes[(int)(t / 3) % notes.Length];
                double env = Math.Min(local * 8, 1) * Math.Exp(-local * 1.4);
                song[i] = (float)((Math.Sin(t * n * 2 * Math.PI) * .6 + Math.Sin(t * n * 4 * Math.PI) * .15) * env + Math.Sin(t * 110 * 2 * Math.PI) * .05);
                filtered = filtered * .97f + ((float)random.NextDouble() * 2 - 1) * .03f;
                water[i] = filtered * 2 + (float)(Math.Sin(t * 73 * 2 * Math.PI) * .03);
            }
            music.clip = AudioClip.Create("First Weather - original sketch", song.Length, 1, sr, false); music.clip.SetData(song, 0);
            ambience.clip = AudioClip.Create("Under glass - original water and hum", water.Length, 1, sr, false); ambience.clip.SetData(water, 0);
            float[] click = new float[3308];
            for (int i = 0; i < click.Length; i++) click[i] = (float)(Math.Sin(i / (double)sr * 540 * 2 * Math.PI) * Math.Exp(-i / (double)sr * 38));
            clickClip = AudioClip.Create("Brass key - original", click.Length, 1, sr, false); clickClip.SetData(click, 0);
            music.Play(); ambience.Play();
        }
    }
}
