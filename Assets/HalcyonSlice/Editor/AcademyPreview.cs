using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Halcyon.FirstWeather.Editor
{
    public static class AcademyPreview
    {
        [MenuItem("Halcyon/8 Preview Living Academy (Play Mode)")]
        public static void Preview()
        {
            var game = UnityEngine.Object.FindFirstObjectByType<HalcyonSlice>();
            if (!EditorApplication.isPlaying || game == null) { Debug.Log("Press Play, then preview the academy."); return; }
            game.PreviewMapInterface("class1"); game.OpenAcademyExterior();
            Debug.Log("LIVING ACADEMY PREVIEW: class is ready. Editor story saving remains disabled.");
        }

        [MenuItem("Halcyon/9 Verify Living Academy (Play Mode)")]
        public static void Verify()
        {
            if (!EditorApplication.isPlaying) { Debug.Log("Press Play before verifying the academy."); return; }
            var game = UnityEngine.Object.FindFirstObjectByType<HalcyonSlice>();
            if (game == null) return;
            var issues = new List<string>(); int checks = 0, measured = 0;
            Action<bool, string> check = (ok, why) => { checks++; if (!ok) issues.Add(why); };
            check(AcademyFacadeGraphic.MeshIsComplete, "Architecture sprite geometry is incomplete.");
            foreach (string name in new[] { "AcademyBuilding", "AcademyForecourt" })
                check(Resources.Load<Texture2D>("HalcyonAcademy/" + name) != null, "Missing academy art: " + name);
            var motionHost = new GameObject("Isolated academy motion check", typeof(RectTransform));
            motionHost.SetActive(false);
            try
            {
                bool paused = false, quiet = false;
                var motion = motionHost.AddComponent<AcademyCourtyard>();
                motion.Initialize(null, null, () => paused, () => quiet, 9);
                var student = motion.transform.Find("Academy world/Student following forecourt route 0");
                var initialPosition = student.localPosition;
                check(motion.MovingStudentCount == 8, "Missing student routes.");
                motion.TickAtmosphere(.04f);
                check(Mathf.Abs(motion.AmbientSeconds - .04f) < .001f && student.localPosition != initialPosition, "Ambient clock or walking route did not advance.");
                float before = motion.AmbientSeconds;
                var heldPosition = student.localPosition;
                quiet = true; motion.TickAtmosphere(.04f);
                check(motion.AmbientSeconds == before && student.localPosition == heldPosition, "Reduced motion did not freeze ambience.");
                quiet = false; paused = true; motion.TickAtmosphere(.04f);
                check(motion.AmbientSeconds == before && student.localPosition == heldPosition, "Pause did not freeze ambience.");
                paused = false; motion.TickAtmosphere(float.NaN); motion.TickAtmosphere(float.PositiveInfinity);
                check(motion.AmbientSeconds == before, "Invalid frame time entered the scene clock.");
                motion.TickAtmosphere(10);
                check(Mathf.Abs(motion.AmbientSeconds - before - .05f) < .001f, "Long frame time caused a scenery jump.");
            }
            finally { UnityEngine.Object.DestroyImmediate(motionHost); }
            for (int step = 0; step <= 10; step++)
            {
                float zoom = Mathf.Lerp(AcademyCourtyard.MinZoom, AcademyCourtyard.MaxZoom, step / 10f);
                foreach (var p in new[] { new Vector2(-10000, -10000), new Vector2(10000, 10000), new Vector2(800, 450) })
                {
                    Vector2 c = AcademyCourtyard.ClampCenter(p, zoom);
                    check(c.x - 800 / zoom >= -190.01f && c.x + 800 / zoom <= 1790.01f, "Horizontal camera boundary failed.");
                    check(c.y - 450 / zoom >= -200.01f && c.y + 450 / zoom <= 920.01f, "Vertical camera boundary failed.");
                }
            }
            foreach (var p in new[] { new Vector2(200, 150), new Vector2(800, 450), new Vector2(1400, 800) })
            {
                Vector2 before = AcademyCourtyard.OverviewCenter + (p - AcademyCourtyard.OverviewCenter) / 1.2f;
                Vector2 anchored = AcademyCourtyard.ZoomAnchor(AcademyCourtyard.OverviewCenter, 1.2f, 1.8f, p);
                Vector2 after = anchored + (p - AcademyCourtyard.OverviewCenter) / 1.8f;
                check(Vector2.Distance(before, after) < .01f, "Zoom changed the world point under the cursor.");
            }
            string saveBefore = PlayerPrefs.GetString("Halcyon.FirstWeather.Save.v1", "");
            foreach (string phase in new[] { "arrival", "class1", "afternoon", "morning2", "class2", "corridor", "homecoming" })
                foreach (bool large in new[] { false, true })
                {
                    game.PreviewMapInterface(phase, large);
                    string stateBefore = JsonUtility.ToJson(game.state);
                    game.OpenAcademyExterior();
                    check(JsonUtility.ToJson(game.state) == stateBefore, "Visiting academy changed story state: " + phase);
                    check(game.CanEnterAcademyClass == (phase == "class1" || phase == "class2"), "Class availability mismatch: " + phase);
                    if (!game.CanEnterAcademyClass)
                    {
                        game.EnterAcademyClass();
                        check(JsonUtility.ToJson(game.state) == stateBefore, "Closed classroom changed story state: " + phase);
                    }
                    Canvas.ForceUpdateCanvases();
                    foreach (var t in UnityEngine.Object.FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None))
                    {
                        if (!t.gameObject.activeInHierarchy) continue;
                        measured++;
                        check(t.GetPreferredValues(t.text, t.rectTransform.rect.width, 0).y <= t.rectTransform.rect.height + 1, "Academy text overflow: " + t.text);
                    }
                    game.ReturnFromAcademy();
                    check(JsonUtility.ToJson(game.state) == stateBefore, "Returning to district changed story state: " + phase);
                    check(game.state.Validate(), "State no longer validates: " + phase);
                    if (game.CanEnterAcademyClass)
                    {
                        game.EnterAcademyClass();
                        check(game.state.view == "story" && game.state.story == phase && game.state.Validate(), "Class entry failed: " + phase);
                    }
                }
            check(saveBefore == PlayerPrefs.GetString("Halcyon.FirstWeather.Save.v1", ""), "Editor preview overwrote the story save.");
            game.PreviewMapInterface("class1", PlayerPrefs.GetInt("Halcyon.LargeText", 0) == 1); game.OpenAcademyExterior();
            string report = "LIVING ACADEMY: " + issues.Count + " issues; " + checks + " checks; " + measured + " text measurements.\n" +
                "Checked camera bounds, cursor-anchored zoom, architecture mesh, art loading, seven map phases, two text settings, class entry/gating, state preservation, preview save isolation, student motion, pause, reduced motion and frame-time safety.\n" +
                "Visual rendering and trackpad comfort need separate checks.\n" + string.Join("\n", issues);
            File.WriteAllText("Assets/HalcyonSlice/Documentation/AcademyVerification.txt", report);
            if (issues.Count == 0) Debug.Log(report); else Debug.LogError(report);
        }
    }
}
