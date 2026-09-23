using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Halcyon.FirstWeather.Editor
{
    public static class V03RuntimeReview
    {
        static readonly string[] scenes = { "academy", "morning1", "callhome", "class1", "evening", "garden", "ending", "map", "title" };
        static int preview, step;
        static bool fallback;
        static double next;
        static HalcyonSlice game;
        static string saved;
        [MenuItem("Halcyon/Review next v0.3 scene (Play Mode)")]
        public static void Preview()
        {
            var g = UnityEngine.Object.FindFirstObjectByType<HalcyonSlice>();
            if (!EditorApplication.isPlaying || g == null) return;
            g.PreviewStationery(scenes[preview++ % scenes.Length]);
        }
        [MenuItem("Halcyon/Playtest complete route (isolated save)")]
        public static void Playthrough() { fallback = false; StartRun(); }
        [MenuItem("Halcyon/Playtest fallback route (isolated save)")]
        public static void Fallback() { fallback = true; StartRun(); }
        static void StartRun()
        {
            game = UnityEngine.Object.FindFirstObjectByType<HalcyonSlice>();
            if (!EditorApplication.isPlaying || game == null) return;
            saved = PlayerPrefs.GetString("Halcyon.FirstWeather.Save.v1", "");
            game.PreviewMapInterface("arrival"); step = 0; next = 0;
            EditorApplication.update -= Tick; EditorApplication.update += Tick;
        }
        static void Click(string fragment)
        {
            var b = UnityEngine.Object.FindObjectsByType<Button>(FindObjectsSortMode.None).FirstOrDefault(x => x.gameObject.activeInHierarchy && x.interactable && x.GetComponentsInChildren<TMP_Text>().Any(t => t.text.Contains(fragment)));
            if (b == null) throw new Exception("No active button for " + fragment);
            b.onClick.Invoke();
        }
        static void Tick()
        {
            if (!EditorApplication.isPlaying || game == null) { EditorApplication.update -= Tick; return; }
            if (EditorApplication.timeSinceStartup < next) return;
            next = EditorApplication.timeSinceStartup + .24;
            try
            {
                if (++step > 130) throw new Exception("Playthrough failed to terminate");
                var s = game.state;
                if (s.view == "story")
                {
                    var p = SliceStory.Get(s.story, s)[s.page];
                    string selected = p.choices.FirstOrDefault(c => c.effect == (fallback ? "skip" : "hum") || c.effect == "mentor:ecology" || c.effect == "support" || c.effect == "share" || c.effect == "tea")?.label;
                    Click(p.choices.Length == 0 ? (s.page == SliceStory.Get(s.story, s).Count - 1 ? "Continue" : "Next") : selected ?? p.choices[0].label);
                }
                else if (s.view == "evening")
                {
                    if (!s.calledHome) Click("Call home");
                    else if (!s.messagedJules) Click("Message Jules");
                    else Click("Turn in for the night");
                }
                else if (s.view == "garden")
                {
                    if (fallback)
                    {
                        bool prompt = UnityEngine.Object.FindObjectsByType<Button>(FindObjectsSortMode.None).Any(x => x.gameObject.activeInHierarchy && x.GetComponentsInChildren<TMP_Text>().Any(t => t.text.Contains("Leave a note and finish")));
                        Click(prompt ? "Leave a note and finish" : "Leave a note for tomorrow");
                    }
                    else if (s.WaterUsed < 6)
                    {
                        var plus = UnityEngine.Object.FindObjectsByType<Button>(FindObjectsSortMode.None).Where(x => x.gameObject.activeInHierarchy && x.interactable && x.GetComponentsInChildren<TMP_Text>().Any(t => t.text == "+")).OrderBy(x => x.transform.position.x).ToArray();
                        int bed = Array.FindIndex(s.water, n => n < 2); plus[bed].onClick.Invoke();
                    }
                    else Click(s.puzzleRound == 0 ? "Try this allocation" : "Commit to this garden");
                }
                else if (s.view == "ending")
                {
                    if (!s.finished || !s.Validate() || (s.solved == fallback) || (fallback && !s.panic) || saved != PlayerPrefs.GetString("Halcyon.FirstWeather.Save.v1", "")) throw new Exception("Final state/save isolation failed");
                    string report = DateTime.UtcNow.ToString("u") + "\nPASS: " + step + " successive UI button actions through both mornings, both classes, two activities, evening call and message, garden and final dispatch. Outcome: " + (s.solved ? "Garden Restored" : "Work in Progress") + "; panic=" + s.panic + "; valid complete state; original player save unchanged. Automated interaction run, not a human playtest.\n";
                    File.WriteAllText("Assets/HalcyonSlice/Documentation/" + (fallback ? "V03-Fallback-Playthrough.txt" : "V03-Playthrough.txt"), report); Debug.Log(report);
                    EditorApplication.update -= Tick;
                }
                else if (s.phase == "arrival" || s.phase == "morning2") Click("Residence");
                else if (s.phase == "class1" || s.phase == "class2")
                {
                    if (UnityEngine.Object.FindFirstObjectByType<AcademyCourtyard>() != null) Click("Enter the classroom"); else Click("Academy");
                }
                else if (s.phase == "afternoon")
                {
                    if (fallback ? !s.socialized : !s.studied) Click(fallback ? "Canal Steps" : "Reading Room");
                    else if (!s.rested)
                    {
                        if (UnityEngine.Object.FindObjectsByType<Button>(FindObjectsSortMode.None).Any(x => x.gameObject.activeInHierarchy && x.GetComponentsInChildren<TMP_Text>().Any(t => t.text.StartsWith("Rest  ·")))) Click("Rest  ·");
                        else Click("Residence");
                    }
                    else Click("Residence");
                }
                else Click("Greenhouse");
            }
            catch (Exception e) { EditorApplication.update -= Tick; Debug.LogException(e); }
        }
    }
}
