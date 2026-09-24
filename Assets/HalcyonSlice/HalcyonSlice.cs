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
            academyExterior = false;
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
        Color ink = new Color32(39, 32, 38, 255), cream = new Color32(247, 234, 211, 255);
        Color brass = new Color32(197, 162, 115, 255), mint = new Color32(219, 196, 158, 255);
        Color mutedInk = new Color32(115, 93, 86, 255);

        void Awake()
        {
            Application.targetFrameRate = 60;
            font = TMP_Settings.defaultFontAsset;
            foreach (string key in new[] { "map", "dorm", "academy", "greenhouse", "cover" })
                art[key] = Resources.Load<Texture2D>("HalcyonArtV3/" + key);
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
            PreparePresentation();
            SetupAudio(); displayedPressure = state.pressure;
            Render();
        }

        void Update()
        {
            canvas.scaleFactor = Mathf.Min(Screen.width / 1600f, Screen.height / 900f);
            if (!title && !modal && !state.finished) state.playSeconds += Time.unscaledDeltaTime;
            displayedPressure = quietMotion ? state.pressure : Mathf.Lerp(displayedPressure, state.pressure, Time.unscaledDeltaTime * 3);
            if (gaugeFeel != null)
            {
                gaugeFeel.reducedMotion = quietMotion; gaugeFeel.paused = modal;
                gaugeFeel.trembleMultiplier = state.ShelterFactor * (state.medicated ? .32f : 1);
                gaugeFeel.SetPressure(state.pressure);
            }
            if (soundscape != null) soundscape.Mix(state, muted, modal || title);
            if (gaugeValue != null) gaugeValue.text = Mathf.RoundToInt(displayedPressure) + " / 100";
            if (screenFade != null) { fade = quietMotion ? 1 : Mathf.MoveTowards(fade, 1, Time.unscaledDeltaTime * 4); screenFade.alpha = fade; }
            float level = muted ? 0 : modal ? .006f : state.story == "callhome" ? 0 : state.WithLola ? .022f : .035f;
            music.volume = Mathf.Lerp(music.volume, level, Time.unscaledDeltaTime * 3);
            float water = academyExterior ? .055f : state.story == "greenhouse" || state.story == "aftergarden" || state.view == "garden" ? .08f : .03f;
            ambience.volume = Mathf.Lerp(ambience.volume, muted ? 0 : modal ? .01f : water, Time.unscaledDeltaTime * 2);
            var kb = Keyboard.current;
            if (journalInput != null && journalInput.isFocused)
            {
                if (kb != null && kb.escapeKey.wasPressedThisFrame) { journalInput.DeactivateInputField(); Save(); CloseModal(); }
                return;
            }
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
                if (loaded == null) return false;
                if (loaded.version == 1) { PlayerPrefs.SetString("Halcyon.FirstWeather.Backup.v1", PlayerPrefs.GetString(SaveKey)); loaded.Migrate(); }
                if (!loaded.Validate()) return false;
                state = loaded; return true;
            }
            catch { return false; }
        }
        public void NewGame()
        {
            #if UNITY_EDITOR
            interfacePreview = false;
            #endif
            academyExterior = false;
            title = false; modal = false; state = new SliceState { seed = Environment.TickCount & int.MaxValue };
            state.Note("Quietest wing. Dad checked.");
            Save(); Render();
        }
        void ResumeGame()
        {
            academyExterior = false;
            #if UNITY_EDITOR
            interfacePreview = false;
            #endif
            if (Load()) { title = false; displayedPressure = state.pressure; Render(); } else NewGame();
        }
        public void StartStory(string key)
        {
            academyExterior = false;
            state.view = "story"; state.story = key; state.page = 0; Save(); Render();
        }
        public void Advance(string effect)
        {
            if (Time.unscaledTime - lastAction < .15f) return;
            lastAction = Time.unscaledTime;
            state.Apply(effect);
            state.page++;
            if (state.page >= SliceStory.Get(state.story, state).Count) EndStory(); else { Save(); Render(); }
        }
        void EnterStoryPage()
        {
            var pages = SliceStory.Get(state.story, state);
            state.page = Mathf.Clamp(state.page, 0, pages.Count - 1);
            string ev = pages[state.page].onEnter;
            bool fresh = ev != "" && !state.events.Contains(ev);
            state.EnterEvent(ev);
            if (fresh && (ev == "assessment" || ev == "call-end" || ev == "harvest-lights")) soundscape?.Harvest();
            if (fresh) Save();
        }
        void EndStory()
        {
            string key = state.story;
            state.view = "map"; state.story = ""; state.page = 0;
            if (key == "morning1") state.phase = "class1";
            if (key == "class1") state.phase = "afternoon";
            if (key == "callhome" || key == "messagejules") { state.phase = "evening"; state.view = "evening"; }
            if (key == "night") state.Sleep();
            if (key == "morning2") state.phase = "class2";
            if (key == "class2") state.phase = "corridor";
            if (key == "greenhouse") state.view = "garden";
            if (key == "aftergarden") { state.phase = "homecoming"; state.Note("Lola showed me the side landing."); }
            Save(); Render();
        }
        void BeginEvening()
        {
            state.phase = "evening"; state.slots = 0; state.view = "evening"; Save(); Render();
        }
        void DoActivity(string key) { if (state.Activity(key)) StartStory(key); }
        void FinishGarden(bool committed)
        {
            state.CompleteGarden(committed); StartStory("aftergarden");
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
            t.font = displayFont != null ? displayFont : font; t.text = value; t.fontSize = size; t.color = c; t.fontStyle = bold ? FontStyles.Bold : FontStyles.Normal;
            t.alignment = align; t.textWrappingMode = TextWrappingModes.Normal; t.overflowMode = TextOverflowModes.Overflow;
            t.raycastTarget = false; t.lineSpacing = 3; return t;
        }
        Button Button(Transform p, string label, float x, float y, float w, float h, Action action, bool enabled = true, bool primary = false, float size = 22)
        {
            return StationeryButton(p, label, x, y, w, h, action, enabled, primary, Mathf.Max(size, 23));
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
            PreparePresentation();
            if (title) { DrawTitle(); return; }
            if (state.view == "story") DrawStory();
            else if (state.view == "evening") DrawEvening();
            else if (state.view == "garden") DrawGarden();
            else if (state.view == "ending") DrawEnding();
            else if (academyExterior) DrawLivingAcademy();
            else DrawMap();
            if (modal) OpenModal(modalKind);
        }
        void Header(string right)
        {
            Ornament(screen, "Header shade", 0, 0, 1600, 160, HalcyonOrnament.Shape.FadeDown, new Color(.13f, .08f, .13f, .88f));
            Heading(screen, "Halcyon Academy", 38, 20, 480, 57, 29, plum);
            BookText(screen, right, 505, 29, 560, 34, 22, cream);
            InkLink(screen, "Journal", 1210, 23, 154, () => OpenModal("journal"));
            InkLink(screen, "Pause", 1380, 23, 174, () => OpenModal("pause"));
        }
        void DrawTitle()
        {
            Picture(screen, "cover", 660, 0, 940, 900);
            Stationery(screen, "Cloth book cover", 0, 0, 712, 900, true, false);
            Hairline(screen, 33, 35, 677, 35, brass);
            Hairline(screen, 33, 865, 677, 865, brass);
            Hairline(screen, 33, 35, 33, 865, brass);
            Hairline(screen, 677, 35, 677, 865, brass);
            BookText(screen, "Bergen Carloss", 76, 83, 560, 38, 27, cream);
            Heading(screen, "Halcyon\nAcademy", 68, 167, 588, 212, 72, plum);
            BookText(screen, "First Weather", 77, 389, 535, 62, 40, cream);
            Hairline(screen, 78, 463, 155, 463, brass);
            PrinterFlower(screen, 171, 449, brass, 28);
            BookText(screen, "The city is quiet.\nMolly can still hear it.", 78, 493, 548, 83, 28, cream);
            Button(screen, "Begin a new story", 78, 607, 548, 58, NewGame, true, false, 28);
            Button(screen, "Continue your story", 78, 679, 548, 53, ResumeGame, PlayerPrefs.HasKey(SaveKey), true, 26);
            Button(screen, "How to play", 78, 765, 175, 45, () => OpenModal("help"));
            Button(screen, "Settings", 268, 765, 169, 45, () => OpenModal("settings"));
            Button(screen, "Credits", 452, 765, 174, 45, () => OpenModal("credits"));
            BookText(screen, "An illustrated story  ·  First Weather", 78, 830, 548, 30, 20, cream);
            MapControl(screen, "Quit", 1460, 822, 98, Quit);
        }
        string TimeLabel()
        {
            if (state.phase == "evening") return "DAY 01  /  EVENING";
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
            EnterStoryPage();
            var pages = SliceStory.Get(state.story, state);
            state.page = Mathf.Clamp(state.page, 0, pages.Count - 1);
            var page = pages[state.page];
            Picture(screen, SliceStory.Background(state.story), 0, 35, 1600, 900, state.story == "night" ? new Color32(145, 165, 200, 255) : Color.white);
            AddSceneLife(screen, SliceStory.Background(state.story));
            Header(TimeLabel());
            DrawSpeakerPortrait(page);
            DrawComicGutter(page);
            BookText(screen, Chapter().ToLowerInvariant(), 42, 101, 950, 37, 24, cream);
            Ornament(screen, "Story shadow", 0, 513, 1600, 56, HalcyonOrnament.Shape.FadeUp, new Color(.17f, .1f, .13f, .58f));
            Stationery(screen, "Story folio", 0, 551, 1600, 349, false, false);
            Hairline(screen, 25, 560, 1575, 560, ruleInk);
            Hairline(screen, 298, 579, 298, 876, new Color(ruleInk.r, ruleInk.g, ruleInk.b, .45f));
            DrawGauge(screen, 38, 611);
            BookText(screen, "Molly's barometer", 30, 853, 234, 31, 21, fadedInk, true, TextAlignmentOptions.Center);
            BookText(screen, page.speaker, 334, 576, 1050, 36, 26, plum);
            Text(screen, page.text, 334, 618, StoryTextWidth, StoryTextHeight, largeText ? LargerStoryTextSize : StoryTextSize, printedInk);
            BookText(screen, (state.page + 1).ToString("00") + " / " + pages.Count.ToString("00"), 1430, 579, 107, 30, 20, fadedInk, true, TextAlignmentOptions.Right);
            if (page.choices.Length > 0)
            {
                float width = (1200f - (page.choices.Length - 1) * 15) / page.choices.Length;
                for (int i = 0; i < page.choices.Length; i++)
                {
                    var c = page.choices[i];
                    Button(screen, c.label, 334 + i * (width + 15), 831, width, 50, () => Advance(c.effect), true, false, ChoiceTextSize);
                }
            }
            else
            {
                BookText(screen, "Space to turn the page", 334, 842, 740, 32, 22, fadedInk);
                Button(screen, state.page == pages.Count - 1 ? "Continue  →" : "Next  →", 1272, 831, 262, 50, () => Advance(""), true, true, 25);
            }
        }

        string Chapter()
        {
            switch (state.story)
            {
                case "morning1": return "01  /  A ROOM OF YOUR OWN";
                case "class1": return "02  /  THE FIRST COHORT";
                case "callhome": return "04  /  CALLING HOME";
                case "messagejules": return "04  /  EVENING DISPATCHES";
                case "night": return "04  /  THE CITY AT NIGHT";
                case "morning2": return "05  /  WHAT YOU CARRY";
                case "class2": return "06  /  THE UNFINISHED ANSWER";
                case "greenhouse": return "07  /  A QUIETER PLACE";
                case "aftergarden": return "09  /  THE SIDE LANDING";
                default: return "03  /  AN AFTERNOON TO CHOOSE";
            }
        }
        void DrawGarden() { DrawGardenPresentation(); }
        void DrawEnding() { DrawEndingPresentation(); }
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
            journalInput = null;
            modal = true; modalKind = kind;
            modalScroll = null;
            screen.GetComponent<CanvasGroup>().interactable = false;
            overlay = Rect("Overlay " + kind, root, 0, 0, 1600, 900);
            var blocker = Panel(overlay, 0, 0, 1600, 900, new Color(.09f, .055f, .085f, .72f)); blocker.raycastTarget = true;
            Stationery(overlay, "Academy correspondence", 320, 105, 960, 690);
            Hairline(overlay, 364, 218, 1216, 218, ruleInk);
            PrinterFlower(overlay, 1200, 147, plum, 28);
            string heading = kind == "tracks" ? "ACADEMY CONCENTRATIONS" : kind == "pause" ? "PAUSED" : kind == "help" ? "YOUR FIRST DAYS IN HALCYON" : kind == "settings" ? "MAKE YOURSELF COMFORTABLE" : kind == "journal" ? "MOLLY'S JOURNAL" : kind == "credits" ? "CREDITS & PROVENANCE" : kind == "licenses" ? "LICENSES & NOTICES" : kind == "leavegarden" ? "LEAVE THE GARDEN FOR TOMORROW?" : "BACK AT YOUR ROOM";
            BookText(overlay, System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase(heading.ToLowerInvariant()), 364, 148, 816, 63, 39, plum);
            if (kind == "pause")
            {
                Text(overlay, "Your place is saved.\nTime in play is stopped while this menu is open.", 364, 245, 850, 104, 28, printedInk);
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
                Text(overlay, "All information is written as well as colored. There are no timed choices, flashing effects, or audio-only clues. Reduced motion stills the scene, barometer and transitions. Settings persist between visits.", 364, 548, 852, 132, 25, printedInk);
                Button(overlay, "Return", 364, 709, 852, 54, CloseModal, true, true);
            }
            else if (kind == "tracks")
            {
                string[] tracks = { "Ironwork · Marchand", "Inkwork · Delacroix", "Brasswork · Beaumont", "Bridgework · Fontenot", "Greenwork · Thibodaux" };
                for (int i = 0; i < tracks.Length; i++)
                {
                    BookText(overlay, tracks[i], 366, 251 + i * 66, 610, 46, 31, i == 4 ? fadedInk : cream);
                    BookText(overlay, i == 4 ? "Minor only" : "Primary track", 1000, 259 + i * 66, 213, 35, 23, i == 4 ? fadedInk : plum, true, TextAlignmentOptions.Right);
                }
                BookText(overlay, "Per parental recommendation, this concentration is not available as a primary track.", 366, 604, 845, 80, 27, fadedInk);
                Button(overlay, "Return", 364, 709, 852, 54, CloseModal, true, true);
            }
            else if (kind == "journal")
            {
                DrawJournalContents(overlay);
            }
            else if (kind == "residence")
            {
                Text(overlay, "You have " + state.slots + " afternoon " + (state.slots == 1 ? "slot" : "slots") + " left. Rest helps now; ending the day passes the remaining opportunities. Your room contains the " + state.decor + " you chose this morning.", 364, 253, 830, 158, 29, printedInk);
                Button(overlay, "Rest  ·  −13 Pressure  ·  1 slot", 364, 461, 852, 58, () => { CloseModal(); DoActivity("rest"); }, state.CanActivity("rest"), true);
                Button(overlay, "Return home for the evening", 364, 547, 852, 58, () => { CloseModal(); BeginEvening(); });
                Button(overlay, "Return to the city", 364, 701, 852, 58, CloseModal);
            }
            else if (kind == "leavegarden")
            {
                Text(overlay, "Leave the pump low and keep the current allocation in the notebook. Molly and Lola can return to the unfinished beds tomorrow.", 364, 261, 844, 220, 30, printedInk);
                Button(overlay, "Keep working together", 364, 530, 852, 60, CloseModal, true, true);
                Button(overlay, "Leave a note and finish the story", 364, 634, 852, 60, () => { CloseModal(); FinishGarden(false); });
            }
            else
            {
                string content = kind == "help" ? "Play as Molly through two days at Halcyon Academy. Begin at your residence, attend class and spend two afternoon slots. Studying prepares your work; rest and company change what you carry into tomorrow. You can return home early. Call home, reply to Jules, write in your journal, then sleep.\n\nThe barometer has four bands: Clarity, Manageable, Elevated and Crisis. Morning routines affect its behavior. Read the changing dialogue and watch the needle. A severe classroom episode changes the remaining timetable; the story continues.\n\nThe greenhouse has six measures of water and more need than supply. Use the + and − buttons to distribute water, try the allocation, then respond to the moving shade. Ask Lola for clues. Keeping every bed alive while protecting your mentor’s priority yields Garden Restored; an unfinished garden yields Work in Progress. Both finish the two-day story.\n\nClick, or use Tab / Shift+Tab and Enter. Space advances pages without choices. J opens your journal, M toggles sound, Esc opens pause. Drag to explore the district and academy; use the trackpad to pan, or Command/Ctrl + scroll to zoom. The on-screen buttons also zoom and recenter.\n\nProgress saves after choices and journal edits. New game replaces the current save. Settings include larger story text and reduced motion.\n\nContent note: family pressure and an anxiety/panic episode described in text. No timed responses or flashing imagery."
                    : kind == "credits" ? "Concept, world, characters & creative direction\nBergen Carloss\n\nPrototype programming, working dialogue draft & original synthesized audio\nOpenAI Codex, under Bergen’s direction. Dialogue remains an editable draft for Bergen.\n\nIllustrations\nOriginal drawing and art direction by Bergen Carloss. Final title illustration developed with ChatGPT. Environments, portraits, student sprites and interface materials generated with OpenAI image generation under Bergen’s direction, using the supplied artwork as reference.\n\nTypography\nCinzel Decorative — Natanael Gama\nCormorant Garamond — Christian Thalmann and the Cormorant Project Authors\nSIL Open Font License 1.1; full notices available below.\n\nTechnology\nUnity 6.3 LTS; Unity UI, TextMesh Pro and Input System. Ink by inkle powers story text and choices. DOTween by Demigiant animates the barometer. FMOD — Firelight Technologies Pty Ltd. Other preexisting project dependencies retain their notices. This slice uses original Unity-generated audio.\n\nA fictional city inspired by New Orleans. No documentary or therapeutic claim is intended.\n\nCopyright 2026 Bergen Carloss for original project contributions. Third-party components retain their own copyrights and licenses."
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
            var text = Text(viewport, content, 0, 0, w - 25, 2000, 25, printedInk);
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
            modal = false; modalKind = ""; journalInput = null;
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
            soundscape = gameObject.AddComponent<HalcyonSoundscape>();
        }
    }
}
