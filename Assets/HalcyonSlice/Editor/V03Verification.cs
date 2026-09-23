using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Halcyon.FirstWeather.Editor
{
    public static class V03Verification
    {
        static int checks, routes, words;
        static readonly List<string> issues = new List<string>();
        static void Check(bool ok, string why) { checks++; if (!ok) issues.Add(why); }
        static string Choose(StoryPage p, string routine, string mentor)
        {
            if (p.choices.Length == 0) return "";
            return p.choices.FirstOrDefault(c => c.effect == routine || c.effect == "mentor:" + mentor || c.effect == "support" || c.effect == "share" || c.effect == "tea")?.effect ?? p.choices[0].effect;
        }
        static void Scene(SliceState s, string key, string routine, string mentor)
        {
            s.story = key; s.view = "story";
            var pages = SliceStory.Get(key, s);
            for (int i = 0; i < pages.Count; i++)
            {
                var p = pages[i];
                s.EnterEvent(p.onEnter);
                if (p.choices.Length > 0) s.Apply(Choose(p, routine, mentor));
            }
            s.story = ""; s.view = "map";
        }
        public static void Rules()
        {
            FirstWeatherInkCompiler.Compile();
            checks = routes = words = 0; issues.Clear();
            foreach (var first in new[] { "study", "rest", "social" })
            foreach (var second in new[] { "study", "rest", "social" }.Where(a => a != first))
            foreach (var morning in new[] { "routine", "hum", "skip" })
            foreach (var next in new[] { "routine", "hum", "skip" })
            foreach (var mentor in new[] { "circulation", "ecology" })
            {
                var s = new SliceState { seed = 291 + routes };
                Scene(s, "morning1", morning, mentor); Scene(s, "class1", morning, mentor);
                s.phase = "afternoon";
                Check(s.Activity(first) && !s.Activity(first) && s.Activity(second) && s.slots == 0, "Afternoon budget/repeat");
                Check(!s.Activity("study") && !s.Activity("rest") && !s.Activity("social"), "Third activity locked");
                Scene(s, first, morning, mentor); Scene(s, second, morning, mentor);
                Scene(s, "callhome", morning, mentor); Scene(s, "messagejules", morning, mentor);
                Scene(s, "night", morning, mentor); s.Sleep();
                int pressure = s.pressure, variance = s.morningVariance; s.Sleep();
                Check(s.pressure == pressure && s.morningVariance == variance && Math.Abs(variance) <= 8, "Sleep idempotence and persisted variance");
                Scene(s, "morning2", next, mentor); Scene(s, "class2", next, mentor);
                Check(s.academicWarning == !s.studied && s.recovery == s.panic, "Academic and threshold consequences");
                int held = s.pressure; s.EnterEvent("assessment"); Check(held == s.pressure, "Assessment cannot replay on resume");
                Scene(s, "greenhouse", next, mentor);
                s.water = new[] { 2, 2, 2 }; s.GardenShift(); s.water = new[] { 2, 2, 2 }; s.CompleteGarden(true);
                Scene(s, "aftergarden", next, mentor); s.finished = true; s.phase = "homecoming"; s.view = "ending";
                s.personalEntry = "My words — not a telemetry log.";
                var loaded = JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(s));
                Check(loaded.Validate() && loaded.solved && loaded.personalEntry == s.personalEntry && loaded.events.Count == s.events.Count && loaded.morningVariance == s.morningVariance, "Full route save/resume");
                Check(!s.medicated || s.pressure >= 20, "Vapeur floor");
                Check(s.rebound == (morning == "routine" && next != "routine"), "Withdrawal route");
                routes++;
            }
            var medicine = new SliceState(); medicine.SetRoutine("vapeur"); medicine.Pressure(-500);
            Check(medicine.pressure == 20, "Medication cannot enter Clarity");
            medicine.pressure = 38; medicine.Pressure(20); int once = medicine.pressure - 38;
            medicine.Sleep(); medicine.SetRoutine("vapeur"); medicine.pressure = 38; medicine.Pressure(20);
            Check(medicine.pressure - 38 < once, "Repeated routine steadies subsequent pressure");
            var clarity = new SliceState { day = 2, pressure = 23, tolerance = 2, story = "aftergarden" };
            clarity.events.Add("lola-arrival"); clarity.EnterEvent("settling");
            Check(clarity.firstClarity && clarity.pressure == 18, "First unmedicated Clarity has its own scene response");
            var low = new SliceState { day = 2, pressure = 30, studied = true, rested = true, medicated = true, routineDays = 2 };
            low.EnterEvent("assessment"); Check(!low.panic, "Low-pressure route has no forced panic");
            var high = new SliceState { day = 2, pressure = 90 }; high.EnterEvent("assessment");
            Check(high.panic && high.missedPeriods == 2, "Threshold episode costs remaining seminar");
            var garden = new SliceState { day = 2, water = new[] { 3, 3, 0 } }; garden.CompleteGarden(true);
            Check(!garden.solved && garden.Target(0) + garden.Target(1) + garden.Target(2) > 6, "Scarcity and lost seedlings matter");
            Check(!garden.AdjustWater(0, 1) && !garden.AdjustWater(-1, 1), "Allocation bounds");
            garden.CompleteGarden(false); Check(!garden.solved, "Unfinished result");
            var priorities = new SliceState { water = new[] { 2, 3, 1 }, mentor = "circulation" };
            Check(priorities.GardenMaintained, "Ironwork plan protects kitchen"); priorities.mentor = "ecology";
            Check(!priorities.GardenMaintained, "Greenwork plan requires young-root reserve"); priorities.water = new[] { 1, 3, 2 };
            Check(priorities.GardenMaintained, "Greenwork accepts young-root priority"); priorities.mentor = "circulation";
            Check(!priorities.GardenMaintained, "Ironwork has a different shortage commitment");
            var migrated = new SliceState { version = 1, routineDays = 1 }; migrated.Migrate();
            Check(migrated.Validate() && migrated.medicated && migrated.version == 3, "Legacy save migration");
            var obj = new GameObject("V03 typography measure", typeof(RectTransform));
            var text = obj.AddComponent<TextMeshProUGUI>();
            var face = TMP_FontAsset.CreateFontAsset(Resources.Load<Font>("HalcyonFonts/CormorantGaramond-Semibold"));
            text.font = face; text.lineSpacing = 3; text.textWrappingMode = TextWrappingModes.Normal;
            foreach (int pressure in new[] { 18, 34, 59, 98 })
            foreach (string mentor in new[] { "circulation", "ecology" })
            foreach (string key in SliceState.SceneKeys)
            {
                var s = new SliceState { pressure = pressure, mentor = mentor, studied = pressure < 60, panic = pressure > 95, medicated = pressure == 34, tolerance = 2, shared = pressure < 60, decor = "fern", gardenPriority = "flowers" };
                foreach (var p in SliceStory.Get(key, s))
                {
                    text.fontSize = HalcyonSlice.LargerStoryTextSize;
                    float h = text.GetPreferredValues(p.text, HalcyonSlice.StoryTextWidth, 0).y;
                    Check(h <= HalcyonSlice.StoryTextHeight + 1, key + " larger text: " + h + " / " + p.text);
                    foreach (var c in p.choices)
                    {
                        text.fontSize = HalcyonSlice.ChoiceTextSize;
                        float w = (1200 - (p.choices.Length - 1) * 15) / p.choices.Length - 38;
                        Check(text.GetPreferredValues(c.label, w, 0).y <= 44 && c.effect != "", "Choice: " + c.label);
                    }
                }
            }
            foreach (string key in new[] { "morning1", "class1", "study", "rest", "night", "morning2", "class2", "greenhouse", "aftergarden" })
                foreach (var p in SliceStory.Get(key, new SliceState { studied = true, decor = "fern" })) words += Regex.Matches(p.text, @"\b[\w'-]+\b").Count;
            Check(words >= 1250, "Five-minute reading-content floor");
            UnityEngine.Object.DestroyImmediate(obj); foreach (var a in face.atlasTextures) UnityEngine.Object.DestroyImmediate(a); UnityEngine.Object.DestroyImmediate(face.material); UnityEngine.Object.DestroyImmediate(face);
            Report("V03-Rules.txt", "Rules, Ink, persistence and actual-font text measurements; " + routes + " complete simulation routes; " + words + " words on the study/rest route. Reading estimate only; not a human timing study.");
            if (issues.Count > 0) throw new Exception("V03 rules verification failed: " + issues.Count + " issues.");
        }
        static void Report(string file, string context)
        {
            string report = DateTime.UtcNow.ToString("u") + "\n" + context + "\n" + checks + " assertions; " + issues.Count + " issues.\n" + string.Join("\n", issues);
            File.WriteAllText("Assets/HalcyonSlice/Documentation/" + file, report);
            if (issues.Count == 0) Debug.Log(report); else Debug.LogError(report);
        }
        [MenuItem("Halcyon/Verify v0.3 screens (Play Mode)")]
        public static void Screens()
        {
            var game = UnityEngine.Object.FindFirstObjectByType<HalcyonSlice>();
            if (!EditorApplication.isPlaying || game == null) return;
            checks = 0; issues.Clear();
            string saved = PlayerPrefs.GetString("Halcyon.FirstWeather.Save.v1", "");
            foreach (bool bigger in new[] { false, true })
            {
                foreach (var sample in new[] { "title", "map", "academy", "evening", "garden", "ending" })
                { game.PreviewStationery(sample, bigger); Measure(sample); }
                foreach (string key in SliceState.SceneKeys)
                {
                    var pages = SliceStory.Get(key, new SliceState());
                    for (int i = 0; i < pages.Count; i++) { game.PreviewStationery(key, bigger, i); Measure(key + " " + i); }
                }
                foreach (var dialog in new[] { "help", "pause", "settings", "credits", "licenses", "journal", "tracks", "residence", "leavegarden" })
                { game.PreviewStationery("map", bigger, 0, dialog); Measure(dialog); }
            }
            Check(saved == PlayerPrefs.GetString("Halcyon.FirstWeather.Save.v1", ""), "Preview save isolation");
            game.PreviewStationery("academy");
            Report("V03-Screens.txt", "Runtime screens, dialogue pages and dialogs; default and larger text; no player save modified.");
        }
        static void Measure(string context)
        {
            Canvas.ForceUpdateCanvases();
            foreach (var t in UnityEngine.Object.FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None))
            {
                if (!t.gameObject.activeInHierarchy || t.GetComponentInParent<ScrollRect>() != null || t.GetComponentInParent<TMP_InputField>() != null) continue;
                Check(t.GetPreferredValues(t.text, t.rectTransform.rect.width, 0).y <= t.rectTransform.rect.height + 1, context + ": " + t.text);
            }
        }
        public static void Batch() { try { Rules(); EditorApplication.Exit(0); } catch (Exception e) { Debug.LogException(e); EditorApplication.Exit(1); } }
    }
}
