using System;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;

namespace Halcyon.FirstWeather
{
    // Ink owns prose, conditional text and choices. Unity owns the shared simulation state.
    // Rebuilding a scene from that state keeps saves small and permits writer edits without
    // serializing an obsolete compiled Ink call stack. Every displayed paragraph is one page.
    public static class SliceStory
    {
        public static List<StoryPage> Get(string key, SliceState s)
        {
            var asset = Resources.Load<TextAsset>("HalcyonNarrative/FirstWeatherCompiled");
            if (asset == null) throw new InvalidOperationException("First Weather Ink has not been compiled. Use Halcyon > Compile story.");
            var ink = new Story(asset.text);
            var errors = new List<string>();
            ink.onError += (message, type) => { if (type == Ink.ErrorType.Error) errors.Add(message); };
            var values = new Dictionary<string, object> {
                {"pressure",s.pressure},{"zone",s.ZoneIndex},{"medicated",s.medicated},{"rebound",s.rebound},
                {"studied",s.studied},{"rested",s.rested},{"socialized",s.socialized},{"shared",s.shared},
                {"solved",s.solved},{"assisted",s.assisted},{"panic",s.panic},{"recovery",s.recovery},
                {"academic_warning",s.academicWarning},{"first_clarity",s.firstClarity},
                {"tolerance",s.tolerance},{"mentor",s.mentor},{"decor",s.decor},{"response",s.panicResponse},
                {"invitation",s.invitation},{"shortage",s.gardenPriority},{"called_home",s.calledHome},
                {"messaged_jules",s.messagedJules},{"day",s.day}
            };
            foreach (var pair in values) ink.variablesState[pair.Key] = pair.Value;
            ink.ChoosePathString(key);
            var pages = new List<StoryPage>(); int guard = 0;
            while ((ink.canContinue || ink.currentChoices.Count > 0) && guard++ < 160)
            {
                if (ink.canContinue)
                {
                    string line = ink.Continue().Trim();
                    if (line.Length == 0) continue;
                    var p = new StoryPage("MOLLY", line);
                    foreach (string tag in ink.currentTags)
                    {
                        if (tag.StartsWith("speaker:")) p.speaker = tag.Substring(8).Trim();
                        if (tag.StartsWith("event:")) p.onEnter = tag.Substring(6).Trim();
                        if (tag.StartsWith("portrait:")) p.portrait = tag.Substring(9).Trim();
                        if (tag.StartsWith("panel:")) p.panel = tag.Substring(6).Trim();
                    }
                    pages.Add(p);
                }
                else
                {
                    if (pages.Count == 0) throw new InvalidOperationException("Ink scene begins with choices before a paragraph: " + key);
                    var choices = new List<StoryChoice>();
                    foreach (var c in ink.currentChoices)
                    {
                        string effect = "";
                        if (c.tags != null) foreach (string tag in c.tags) if (tag.StartsWith("effect:")) effect = tag.Substring(7).Trim();
                        if (effect == "") throw new InvalidOperationException("Choice needs an effect tag: " + c.text);
                        choices.Add(new StoryChoice(c.text, effect));
                    }
                    pages[pages.Count - 1].choices = choices.ToArray();
                    // Every choice gathers to the next paragraph. Effects are applied only when
                    // the player commits, then Ink re-evaluates that paragraph using current state.
                    ink.ChooseChoiceIndex(0);
                }
            }
            if (errors.Count > 0 || guard >= 160 || pages.Count == 0) throw new InvalidOperationException("Ink scene " + key + ": " + string.Join("; ", errors));
            return pages;
        }
        public static string Background(string key)
        {
            if (key == "morning1" || key == "morning2" || key == "rest" || key == "night" || key == "callhome" || key == "messagejules") return "dorm";
            if (key == "greenhouse" || key == "aftergarden") return "greenhouse";
            return key == "social" ? "map" : "academy";
        }
    }
}
