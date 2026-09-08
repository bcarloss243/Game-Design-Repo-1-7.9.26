using System;
using System.Collections.Generic;

namespace Halcyon.FirstWeather
{
    [Serializable]
    public class SliceState
    {
        public int version = 1;
        public string view = "map", phase = "arrival", story = "", mentor = "", decor = "";
        public int page, day = 1, pressure = 54, slots = 2, routineDays, puzzleRound;
        public bool studied, rested, socialized, shared, solved, assisted, finished;
        public float playSeconds;
        public int[] water = { 0, 0, 0 };
        public List<string> journal = new List<string>();

        public void Pressure(int delta) { pressure = Math.Max(0, Math.Min(100, pressure + delta)); }
        public string Zone { get { return pressure < 20 ? "CLARITY" : pressure < 45 ? "STEADY" : pressure < 70 ? "ELEVATED" : "OVERWHELMED"; } }
        public void Note(string text) { if (!journal.Contains(text)) journal.Add(text); }
        public bool CanActivity(string activity)
        {
            return phase == "afternoon" && slots > 0 &&
                (activity == "study" ? !studied : activity == "rest" ? !rested : activity == "social" && !socialized);
        }
        public bool Activity(string activity)
        {
            if (!CanActivity(activity)) return false;
            slots--;
            if (activity == "study") { studied = true; Pressure(12); Note("Prepared the circulation assignment. Tomorrow, Molly will have an answer."); }
            else if (activity == "rest") { rested = true; Pressure(-17); Note("Made time to rest. Molly knows an early sign that she needs a quieter place."); }
            else { socialized = true; Pressure(-9); Note("Met Jules at the canal steps. There is one familiar face in tomorrow's classroom."); }
            return true;
        }
        public void Sleep()
        {
            day = 2; Pressure(-9 + (studied && !rested ? 5 : 0));
            phase = "morning2";
            Note("Overnight: sleep eased Pressure by 9. " + (studied && !rested ? "Late study carried 5 Pressure into morning." : "The day's small choices came with you."));
        }
        public int WaterUsed { get { return water[0] + water[1] + water[2]; } }
        public int Target(int plant) { return puzzleRound == 0 ? new int[] { 1, 2, 3 }[plant] : new int[] { 2, 3, 1 }[plant]; }
        public bool AdjustWater(int plant, int delta)
        {
            if (plant < 0 || plant > 2 || Math.Abs(delta) != 1) return false;
            if (water[plant] + delta < 0 || water[plant] + delta > 4 || (delta > 0 && WaterUsed >= 6)) return false;
            water[plant] += delta; return true;
        }
        public bool GardenReady { get { return water[0] == Target(0) && water[1] == Target(1) && water[2] == Target(2); } }
        public void GardenShift() { puzzleRound = 1; Note("The shade shifted. The garden's needs changed; the total supply stayed at six."); }
        public bool Validate()
        {
            return version == 1 && day >= 1 && day <= 2 && pressure >= 0 && pressure <= 100 && slots >= 0 && slots <= 2 &&
                page >= 0 && page < 30 && water != null && water.Length == 3 && journal != null &&
                puzzleRound >= 0 && puzzleRound <= 1 && water[0] >= 0 && water[1] >= 0 && water[2] >= 0 &&
                water[0] <= 4 && water[1] <= 4 && water[2] <= 4 && WaterUsed <= 6 &&
                !float.IsNaN(playSeconds) && !float.IsInfinity(playSeconds) && playSeconds >= 0 &&
                new List<string> { "map", "story", "garden", "ending" }.Contains(view) &&
                new List<string> { "arrival", "class1", "afternoon", "morning2", "class2", "corridor", "homecoming" }.Contains(phase) &&
                (view != "story" || (new List<string> { "morning1", "class1", "study", "rest", "social", "night", "morning2", "class2", "greenhouse", "aftergarden" }.Contains(story) && page < SliceStory.Get(story, this).Count));
        }
    }
    public class StoryChoice
    {
        public string label, effect;
        public StoryChoice(string label, string effect) { this.label = label; this.effect = effect; }
    }
    public class StoryPage
    {
        public string speaker, text;
        public StoryChoice[] choices;
        public StoryPage(string speaker, string text, params StoryChoice[] choices) { this.speaker = speaker; this.text = text; this.choices = choices; }
    }
}
