using System;
using System.Collections.Generic;

namespace Halcyon.FirstWeather
{
    [Serializable]
    public class SliceState
    {
        public int version = 3;
        public string view = "map", phase = "arrival", story = "", mentor = "", decor = "";
        public int page, day = 1, pressure = 54, slots = 2, routineDays, puzzleRound;
        public bool studied, rested, socialized, shared, solved, assisted, finished;
        public float playSeconds;
        public int[] water = { 0, 0, 0 }, firstWater = { 0, 0, 0 };
        public List<string> journal = new List<string>(), events = new List<string>();
        public string morningOne = "", morningTwo = "", panicResponse = "", invitation = "", personalEntry = "", gardenPriority = "";
        public bool medicated, wasMedicated, rebound, calledHome, messagedJules, panic, recovery, academicWarning, firstClarity;
        public int tolerance, shelter, morningVariance, weatherLoad = 22, morningPressure, missedPeriods;
        public int seed = 1729;

        public string Zone => pressure < 20 ? "CLARITY" : pressure < 45 ? "MANAGEABLE" : pressure < 70 ? "ELEVATED" : "CRISIS";
        public int ZoneIndex => pressure < 20 ? 0 : pressure < 45 ? 1 : pressure < 70 ? 2 : 3;
        public bool WithLola => story == "greenhouse" || story == "aftergarden" || view == "garden";
        public bool GreenContent => medicated && pressure < 45;
        public float ShelterFactor => WithLola ? Math.Max(.28f, .62f - shelter * .07f) : 1;
        public bool Evening => phase == "evening";
        public static readonly string[] SceneKeys = { "morning1", "class1", "study", "rest", "social", "callhome", "messagejules", "night", "morning2", "class2", "greenhouse", "aftergarden" };

        public void Pressure(int delta)
        {
            if (delta > 0 && medicated) delta = (int)Math.Round(delta * (routineDays >= 2 ? .55 : .68));
            if (WithLola && shelter > 0) delta = delta > 0 ? (int)Math.Round(delta * .65) : (int)Math.Round(delta * 1.15);
            int floor = medicated ? 20 : (day == 1 || !events.Contains("lola-arrival")) ? 25 : 0;
            pressure = Math.Max(floor, Math.Min(100, pressure + delta));
        }
        public void Note(string text) { if (!journal.Contains(text)) journal.Add(text); }
        public void SetRoutine(string choice)
        {
            if (day == 1 ? morningOne != "" : morningTwo != "") return;
            if (day == 1) morningOne = choice; else morningTwo = choice;
            medicated = choice == "vapeur";
            if (medicated)
            {
                routineDays++; pressure = Math.Min(pressure, routineDays > 1 ? 34 : 38); pressure = Math.Max(20, pressure);
                Note("The Vapeur case still catches on the same hinge. I should get it fixed.");
            }
            else
            {
                if (choice == "hum") { tolerance++; Note("I sat until I could pick out a second note."); }
                else Note("Left the case beside breakfast.");
                if (day == 2 && wasMedicated) { rebound = true; Pressure(13); Note("My hand was unsteady before I reached the landing."); }
            }
        }
        public bool CanActivity(string activity) => phase == "afternoon" && slots > 0 &&
            (activity == "study" ? !studied : activity == "rest" ? !rested : activity == "social" && !socialized);
        public int ActivityDelta(string activity) => activity == "study" ? (medicated ? (int)Math.Round(12 * (routineDays >= 2 ? .55 : .68)) : 12) : activity == "rest" ? -13 : -7;
        public bool Activity(string activity)
        {
            if (!CanActivity(activity)) return false;
            slots--;
            if (activity == "study") { studied = true; Pressure(12); Note("Kept both drawings. The first one was very neat and completely wrong."); }
            else if (activity == "rest") { rested = true; Pressure(-13); Note("Slept through the first bell. Ate the toast cold."); }
            else { socialized = true; Pressure(-7); Note("Jules borrowed my surname for the radio proposal. I said yes too quickly."); }
            return true;
        }
        public void Apply(string effect)
        {
            if (effect.StartsWith("decor:")) { decor = effect.Substring(6); Pressure(-2); Note("Put the " + decor + " on the sill."); }
            else if (effect == "routine") SetRoutine("vapeur");
            else if (effect == "hum") SetRoutine("hum");
            else if (effect == "skip") SetRoutine("skip");
            else if (effect.StartsWith("mentor:")) { mentor = effect.Substring(7); Note(mentor == "circulation" ? "Marchand kept my calculation. Thibodaux kept a seat." : "Greenwork: minor only. I copied the wording exactly."); }
            else if (effect == "support" || effect == "outside") { panicResponse = effect; if (effect == "support") Pressure(-4); }
            else if (effect == "share") { shared = true; shelter++; Pressure(-5); Note("Told Lola about class. Stopped halfway through an apology."); }
            else if (effect == "askgarden") Note("Asked about the raft before I could ask whether I was in the way.");
            else if (effect == "tea" || effect == "return") { invitation = effect; Note(effect == "tea" ? "Asked for tea when I meant yes." : "Said tomorrow before I could take it back."); }
        }
        public void EnterEvent(string key)
        {
            if (key == "" || events.Contains(key)) return;
            events.Add(key);
            switch (key)
            {
                case "call-start": calledHome = true; Pressure(4); break;
                case "call-end": Pressure(5); Note("Dad asked whether the quiet wing was quiet enough. I said it was perfect."); break;
                case "message-jules": messagedJules = true; Pressure(-2); Note("Jules sent three drafts of the radio pitch. One of them is mostly exclamation marks."); break;
                case "assessment":
                    academicWarning = !studied;
                    Pressure((studied ? 18 : 30) + 30 - (rested ? 14 : 0) - tolerance * 3);
                    panic = pressure > 95;
                    if (panic) { recovery = true; missedPeriods = 2; }
                    Note(academicWarning ? "A revision appointment. Marchand said the office would copy Dad." : "The drawing held up. Marchand asked to keep it for the next group.");
                    break;
                case "corridor":
                    if (panic) { pressure = Math.Min(pressure, 78); Note("Missed the afternoon field seminar. Thibodaux brought the recovery timetable himself."); }
                    else Pressure(-5);
                    break;
                case "lola-arrival": shelter++; Pressure(-10); break;
                case "lola-touch": Pressure(-8); break;
                case "lola-reciprocity": shelter++; Pressure(-7); break;
                case "disclosure": Pressure(-8); break;
                case "settling":
                    Pressure(-6);
                    if (!medicated && tolerance >= 2 && pressure < 25) { pressure = 18; firstClarity = true; }
                    else if (!medicated) pressure = Math.Max(20, pressure);
                    break;
            }
        }
        public void Sleep()
        {
            if (day != 1) return;
            wasMedicated = medicated; medicated = false;
            day = 2; morningVariance = new Random(seed).Next(-8, 9);
            pressure = Math.Max(25, Math.Min(100, pressure - 8 + (studied && !rested ? 5 : 0) - (socialized ? 3 : 0) - (messagedJules ? 2 : 0) + weatherLoad + morningVariance));
            morningPressure = pressure; phase = "morning2";
            Note("Slept badly. Can't find a reason.");
        }
        public int WaterUsed => water[0] + water[1] + water[2];
        // Developer-side targets are never printed as an answer key in the game.
        public int Target(int plant) => puzzleRound == 0 ? new[] { 3, 3, 2 }[plant] : new[] { 2, 4, 2 }[plant];
        public bool AdjustWater(int plant, int delta)
        {
            if (plant < 0 || plant > 2 || Math.Abs(delta) != 1) return false;
            if (water[plant] + delta < 0 || water[plant] + delta > 4 || (delta > 0 && WaterUsed >= 6)) return false;
            water[plant] += delta; return true;
        }
        public bool GardenReady => WaterUsed == 6;
        public string Shortage
        {
            get { int most = 0; for (int i = 1; i < 3; i++) if (Target(i) - water[i] > Target(most) - water[most]) most = i; return new[] { "herbs", "flowers", "seedlings" }[most]; }
        }
        public bool GardenMaintained => WaterUsed == 6 && water[0] >= 1 && water[1] >= 1 && water[2] >= 1 && (mentor == "circulation" ? water[0] >= 2 : water[2] >= 2);
        public void GardenShift() { firstWater = (int[])water.Clone(); puzzleRound = 1; Note("The flowers drank first. The shade kept moving."); }
        public void CompleteGarden(bool committed)
        {
            gardenPriority = Shortage; solved = committed && GardenMaintained;
            Pressure(committed ? -12 : -7);
            Note("Left " + gardenPriority + " short. " + (assisted ? "Lola and I argued it through." : "Lola asked why. I showed her."));
        }
        public void Migrate()
        {
            if (version == 3) return;
            if (version != 1) return;
            version = 3; events = new List<string>(); firstWater = new int[3]; personalEntry = "";
            morningOne = routineDays > 0 ? "vapeur" : "skip";
            morningTwo = day == 2 && routineDays > 1 ? "vapeur" : "";
            medicated = day == 1 ? routineDays > 0 : routineDays > 1;
            wasMedicated = routineDays > 0; if (medicated) pressure = Math.Max(20, pressure);
            if (view == "garden" || story == "greenhouse" || story == "aftergarden" || phase == "homecoming") events.Add("lola-arrival");
            journal = new List<string> { "The first pages are packed away. Kept my place." };
            page = 0;
        }
        public bool Validate()
        {
            if (version != 3 || day < 1 || day > 2 || pressure < 0 || pressure > 100 || slots < 0 || slots > 2 || page < 0 || page > 40 ||
                water == null || water.Length != 3 || firstWater == null || firstWater.Length != 3 || journal == null || events == null ||
                personalEntry == null || personalEntry.Length > 1600 || journal.Count > 100 || events.Count > 100 ||
                float.IsNaN(playSeconds) || float.IsInfinity(playSeconds) || playSeconds < 0 || puzzleRound < 0 || puzzleRound > 1) return false;
            foreach (int n in water) if (n < 0 || n > 4) return false;
            foreach (int n in firstWater) if (n < 0 || n > 4) return false;
            return WaterUsed <= 6 && new List<string> { "map", "story", "garden", "ending", "evening" }.Contains(view) &&
                new List<string> { "arrival", "class1", "afternoon", "evening", "morning2", "class2", "corridor", "homecoming" }.Contains(phase) &&
                (view != "story" || Array.IndexOf(SceneKeys, story) >= 0);
        }
    }
    public class StoryChoice
    {
        public string label, effect;
        public StoryChoice(string label, string effect) { this.label = label; this.effect = effect; }
    }
    public class StoryPage
    {
        public string speaker, text, onEnter = "", portrait = "", panel = "";
        public StoryChoice[] choices;
        public StoryPage(string speaker, string text, params StoryChoice[] choices) { this.speaker = speaker; this.text = text; this.choices = choices; }
    }
}
