using UnityEditor;
using UnityEngine;
using TMPro;
using System.IO;
using System.Collections.Generic;

namespace Halcyon.FirstWeather.Editor
{
    public static class MapInterfacePreview
    {
        static int phaseIndex;
        static readonly string[] phases = { "arrival", "class1", "afternoon", "morning2", "class2", "corridor", "homecoming" };

        [MenuItem("Halcyon/5 Preview Map Interface (Play Mode)")]
        public static void Preview()
        {
            if (!EditorApplication.isPlaying) { Debug.Log("Press Play before previewing the map interface."); return; }
            phaseIndex = 0;
            Show();
        }

        [MenuItem("Halcyon/6 Preview Next Map Phase (Play Mode)")]
        public static void NextPhase()
        {
            if (!EditorApplication.isPlaying) return;
            phaseIndex = (phaseIndex + 1) % phases.Length;
            Show();
        }

        static void Show()
        {
            var game = Object.FindFirstObjectByType<HalcyonSlice>();
            if (game == null) return;
            game.PreviewMapInterface(phases[phaseIndex]);
            Debug.Log("MAP INTERFACE PREVIEW: " + phases[phaseIndex] + ". Story saving is disabled for this editor preview; restart Play Mode to return to normal play.");
        }

        [MenuItem("Halcyon/7 Check Map Interface Layout (Play Mode)")]
        public static void CheckLayout()
        {
            if (!EditorApplication.isPlaying) return;
            var game = Object.FindFirstObjectByType<HalcyonSlice>();
            if (game == null) return;
            int measured = 0;
            var issues = new List<string>();
            foreach (string phase in phases)
                foreach (bool larger in new[] { false, true })
                    foreach (int pressure in new[] { 0, 54, 100 })
                    {
                        game.PreviewMapInterface(phase, larger, pressure);
                        Canvas.ForceUpdateCanvases();
                        foreach (var t in Object.FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None))
                        {
                            if (!t.gameObject.activeInHierarchy) continue;
                            measured++;
                            float height = t.GetPreferredValues(t.text, t.rectTransform.rect.width, 0).y;
                            if (height > t.rectTransform.rect.height + 1)
                                issues.Add(phase + " / larger=" + larger + " / " + t.text + ": " + height + " > " + t.rectTransform.rect.height);
                        }
                    }
            game.PreviewMapInterface("arrival", PlayerPrefs.GetInt("Halcyon.LargeText", 0) == 1);
            string report = "MAP INTERFACE LAYOUT: " + issues.Count + " issues across " + measured + " text measurements.\n7 phases x 2 text settings x 3 Pressure readings (0, 54, 100).\n" + string.Join("\n", issues);
            File.WriteAllText("Assets/HalcyonSlice/Documentation/MapInterfaceVerification.txt", report);
            if (issues.Count == 0) Debug.Log(report); else Debug.LogWarning(report);
        }
    }
}
