using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using UnityEngine;
using TMPro;

namespace Halcyon.FirstWeather.Editor
{
    public static class SliceEditor
    {
        public const string ScenePath = "Assets/HalcyonSlice/Scenes/FirstWeather.unity";
        static int checks;
        static void Check(bool condition, string message)
        {
            checks++;
            if (!condition) throw new Exception("First Weather verification: " + message);
        }

        [MenuItem("Halcyon/1 Open First Weather")]
        public static void OpenSlice()
        {
            if (EditorApplication.isPlaying) { Debug.LogWarning("Stop Play mode before opening a scene."); return; }
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            foreach (string key in new[] { "map", "dorm", "academy", "greenhouse", "cover" })
            {
                var importer = AssetImporter.GetAtPath("Assets/HalcyonSlice/Resources/HalcyonArt/" + key + ".png") as TextureImporter;
                if (importer == null) continue;
                importer.textureType = TextureImporterType.Default;
                importer.mipmapEnabled = false;
                importer.wrapMode = TextureWrapMode.Clamp;
                importer.filterMode = FilterMode.Bilinear;
                importer.maxTextureSize = 2048;
                importer.textureCompression = TextureImporterCompression.CompressedHQ;
                importer.SaveAndReimport();
            }
            if (File.Exists(ScenePath)) EditorSceneManager.OpenScene(ScenePath);
            else
            {
                Directory.CreateDirectory("Assets/HalcyonSlice/Scenes");
                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                var cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
                cameraObject.tag = "MainCamera";
                var camera = cameraObject.GetComponent<Camera>();
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = Color.black;
                camera.orthographic = true;
                camera.transform.position = new Vector3(0, 0, -10);
                new GameObject("First Weather", typeof(HalcyonSlice));
                EditorSceneManager.SaveScene(scene, ScenePath);
            }
            Debug.Log("FIRST WEATHER READY — press Play. Original SampleScene is preserved.");
        }

        [MenuItem("Halcyon/2 Verify Story and Rules")]
        public static void Verify()
        {
            checks = 0;
            var s = new SliceState();
            Check(s.Validate() && !s.Activity("study"), "New game starts at residence, with afternoon locked.");
            s.Pressure(-999); Check(s.pressure == 0 && s.Zone == "CLARITY", "Pressure lower limit.");
            s.Pressure(999); Check(s.pressure == 100 && s.Zone == "OVERWHELMED", "Pressure upper limit.");
            foreach (string first in new[] { "study", "rest", "social" })
                foreach (string second in new[] { "study", "rest", "social" })
                {
                    s = new SliceState { phase = "afternoon" };
                    Check(s.Activity(first), "First activity available.");
                    Check(!s.Activity(first) && s.slots == 1, "An activity cannot be repeated.");
                    if (second == first) continue;
                    Check(s.Activity(second) && s.slots == 0, "Two different activities fit.");
                    Check(!s.Activity("study") && !s.Activity("rest") && !s.Activity("social"), "A third activity cannot be taken.");
                    int evening = s.pressure;
                    s.Sleep();
                    Check(s.day == 2 && s.phase == "morning2", "Sleep advances exactly one day.");
                    Check(s.pressure == Math.Max(0, evening - 9 + (s.studied && !s.rested ? 5 : 0)), "Previous choices carry into day two.");
                    var loaded = JsonUtility.FromJson<SliceState>(JsonUtility.ToJson(s));
                    Check(loaded.Validate() && loaded.pressure == s.pressure && loaded.studied == s.studied && loaded.journal.Count == s.journal.Count, "Progress survives serialization.");
                }
            s = new SliceState();
            Check(!s.AdjustWater(0, -1) && !s.AdjustWater(-1, 1) && !s.AdjustWater(0, 8), "Invalid allocations rejected.");
            for (int i = 0; i < 3; i++) for (int j = 0; j <= i; j++) s.AdjustWater(i, 1);
            Check(s.GardenReady && s.WaterUsed == 6 && !s.AdjustWater(0, 1), "First puzzle solved with finite supply.");
            s.GardenShift();
            Check(!s.GardenReady, "Changing light requires a new solution.");
            s.AdjustWater(2, -1); s.AdjustWater(2, -1); s.AdjustWater(0, 1); s.AdjustWater(1, 1);
            Check(s.GardenReady && s.WaterUsed == 6, "Redistribution solves second condition.");
            s.water[0] = 8; Check(!s.Validate(), "Corrupt save allocation rejected.");

            var temp = new GameObject("Text measurement", typeof(RectTransform));
            var text = temp.AddComponent<TextMeshProUGUI>();
            text.font = TMP_Settings.defaultFontAsset;
            Check(text.font != null, "TextMesh Pro default font available.");
            text.fontSize = 29; text.lineSpacing = 5; text.textWrappingMode = TextWrappingModes.Normal;
            int words = 0, pageCount = 0;
            string[] keys = { "morning1", "class1", "study", "rest", "night", "morning2", "class2", "greenhouse", "aftergarden" };
            try
            {
                foreach (bool prepared in new[] { false, true })
                    foreach (bool rested in new[] { false, true })
                        foreach (bool social in new[] { false, true })
                            foreach (bool shared in new[] { false, true })
                                foreach (string mentor in new[] { "circulation", "ecology" })
                                {
                                    s = new SliceState { studied = prepared, rested = rested, socialized = social, shared = shared, solved = shared, mentor = mentor, decor = "fern" };
                                    foreach (string key in new[] { "morning1", "class1", "study", "rest", "social", "night", "morning2", "class2", "greenhouse", "aftergarden" })
                                    {
                                        var pages = SliceStory.Get(key, s);
                                        foreach (var page in pages)
                                        {
                                            text.fontSize = 29;
                                            float height = text.GetPreferredValues(page.text, 1220, 0).y;
                                            Check(height <= 203, key + " / " + page.speaker + " larger text overflows: " + height);
                                            Check(!string.IsNullOrWhiteSpace(page.text), "No empty story page.");
                                            foreach (var choice in page.choices)
                                            {
                                                text.fontSize = page.choices.Length == 3 ? 20 : 21;
                                                float width = (1220f - (page.choices.Length - 1) * 15) / page.choices.Length - 32;
                                                Check(text.GetPreferredValues(choice.label, width, 0).y <= 44, "Choice text fits: " + choice.label);
                                            }
                                        }
                                    }
                                }
                foreach (string key in keys)
                    foreach (var page in SliceStory.Get(key, new SliceState { studied = true, rested = true, decor = "fern" }))
                    { words += Regex.Matches(page.text, @"\b[\w'-]+\b").Count; pageCount++; }
                Check(words >= 1250, "Full two-activity route contains at least five minutes of narrative at 250 words/minute.");
                foreach (string key in new[] { "map", "dorm", "academy", "greenhouse", "cover" })
                    Check(Resources.Load<Texture2D>("HalcyonArt/" + key) != null, "Artwork loads: " + key);
            }
            finally { UnityEngine.Object.DestroyImmediate(temp); }
            string report = "FIRST WEATHER VERIFICATION PASSED\n" + checks + " assertions; all authored story branches fit larger text.\n" + pageCount + " pages / " + words + " narrative words on study + rest route.\nReading estimate at 200–250 words/minute: " + (words / 250f).ToString("0.0") + "–" + (words / 200f).ToString("0.0") + " minutes, plus map choices and puzzle.\nThis is automated verification, not a human playtest.\n";
            Directory.CreateDirectory("Assets/HalcyonSlice/Documentation");
            File.WriteAllText("Assets/HalcyonSlice/Documentation/Verification.txt", report);
            Debug.Log(report);
        }

        [MenuItem("Halcyon/3 Check Current Screen Text (Play Mode)")]
        public static void CheckScreen()
        {
            if (!EditorApplication.isPlaying) return;
            Canvas.ForceUpdateCanvases();
            int issues = 0;
            foreach (var text in UnityEngine.Object.FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None))
            {
                if (!text.gameObject.activeInHierarchy || text.GetComponentInParent<UnityEngine.UI.ScrollRect>() != null) continue;
                var size = text.GetPreferredValues(text.text, text.rectTransform.rect.width, 0);
                if (size.y > text.rectTransform.rect.height + 1)
                { issues++; Debug.LogWarning("FIRST WEATHER TEXT OVERFLOW: " + text.text + " requires " + size.y + " available " + text.rectTransform.rect.height); }
            }
            Debug.Log("FIRST WEATHER SCREEN CHECK: " + issues + " overflow issues.");
        }

        [MenuItem("Halcyon/4 Build Mac Prototype")]
        public static void BuildMac()
        {
            if (EditorApplication.isPlaying) { Debug.LogWarning("Stop Play mode before building."); return; }
            Verify();
            if (!File.Exists(ScenePath)) OpenSlice();
            string output = Path.GetFullPath("Builds/FirstWeather-Mac");
            Directory.CreateDirectory(output);
            string product = PlayerSettings.productName;
            int width = PlayerSettings.defaultScreenWidth, height = PlayerSettings.defaultScreenHeight;
            var mode = PlayerSettings.fullScreenMode;
            bool resize = PlayerSettings.resizableWindow;
            try
            {
                PlayerSettings.productName = "Halcyon Academy - First Weather";
                PlayerSettings.defaultScreenWidth = 1280; PlayerSettings.defaultScreenHeight = 720;
                PlayerSettings.fullScreenMode = FullScreenMode.Windowed; PlayerSettings.resizableWindow = true;
                var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
                {
                    scenes = new[] { ScenePath }, target = BuildTarget.StandaloneOSX,
                    locationPathName = output + "/Halcyon Academy - First Weather.app",
                    options = BuildOptions.None
                });
                string result = "Build result: " + report.summary.result + "; errors: " + report.summary.totalErrors + "; warnings: " + report.summary.totalWarnings + "; bytes: " + report.summary.totalSize;
                File.WriteAllText(output + "/BuildReport.txt", result);
                if (report.summary.result != BuildResult.Succeeded) throw new Exception(result);
                Debug.Log("FIRST WEATHER MAC BUILD READY: " + output + "\n" + result);
            }
            finally
            {
                PlayerSettings.productName = product;
                PlayerSettings.defaultScreenWidth = width; PlayerSettings.defaultScreenHeight = height;
                PlayerSettings.fullScreenMode = mode; PlayerSettings.resizableWindow = resize;
            }
        }
    }
}
