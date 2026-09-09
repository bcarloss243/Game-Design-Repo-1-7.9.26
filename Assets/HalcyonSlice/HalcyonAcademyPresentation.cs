using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Halcyon.FirstWeather
{
    public partial class HalcyonSlice
    {
        // A visit is a presentation state, so existing saves still resume safely on the district map.
        bool academyExterior;
        AcademyCourtyard courtyard;

        public void OpenAcademyExterior()
        {
            if (state.view != "map") return;
            academyExterior = true;
            Render();
        }

        public bool CanEnterAcademyClass => state.phase == "class1" || state.phase == "class2";

        public void EnterAcademyClass()
        {
            if (CanEnterAcademyClass) StartStory(state.phase);
        }

        public void ReturnFromAcademy()
        {
            academyExterior = false;
            Render();
        }

        void DrawLivingAcademy()
        {
            PreparePresentation();
            var viewport = Rect("Academy forecourt viewport", screen, 0, 0, 1600, 900);
            viewport.gameObject.AddComponent<RectMask2D>();
            var hit = viewport.gameObject.AddComponent<Image>();
            hit.color = ink;
            courtyard = viewport.gameObject.AddComponent<AcademyCourtyard>();
            courtyard.Initialize(Resources.Load<Texture2D>("HalcyonAcademy/AcademyForecourt"),
                Resources.Load<Texture2D>("HalcyonAcademy/AcademyBuilding"),
                () => modal || title, () => quietMotion, state.phase == "afternoon" ? 15 : 9);

            Ornament(screen, "Academy upper vignette", 0, 0, 1600, 196, HalcyonOrnament.Shape.FadeDown, new Color(.045f, .06f, .085f, .84f));
            Ornament(screen, "Academy lower vignette", 0, 730, 1600, 170, HalcyonOrnament.Shape.FadeUp, new Color(.045f, .07f, .08f, .88f));
            MapControl(screen, "Back to district", 34, 30, 196, ReturnFromAcademy);
            BookText(screen, "The Academy", 35, 91, 550, 68, 52, paper);
            BookText(screen, "F O R E C O U R T", 39, 158, 350, 29, 16, warmGold, false);
            BookText(screen, TimeLabel(), 1150, 38, 410, 32, 18, paper, false, TextAlignmentOptions.Right);
            MapControl(screen, "Settings", 1252, 84, 140, () => OpenModal("settings"));
            MapControl(screen, "Pause", 1404, 84, 156, () => OpenModal("pause"));

            DrawInstrument(screen, 22, 646, 223);
            BookText(screen, "MOLLY'S BAROMETER", 29, 858, 222, 26, 13, paper, false, TextAlignmentOptions.Center);
            MapControl(screen, "Overview", 290, 803, 139, () => courtyard.Overview());
            MapControl(screen, "Entrance", 441, 803, 139, () => courtyard.FocusEntrance());
            MapControl(screen, "Clocktower", 592, 803, 153, () => courtyard.FocusTower());
            MapControl(screen, "−", 769, 803, 48, () => courtyard.ChangeZoom(-.2f));
            MapControl(screen, "+", 829, 803, 48, () => courtyard.ChangeZoom(.2f));
            BookText(screen, "Drag / two-finger scroll to explore   ·   + / − zoom   ·   Home resets", 290, 860, 910, 29, 16, paper, false);

            string label = CanEnterAcademyClass ? "Enter your classroom" : "Explore at your own pace";
            if (CanEnterAcademyClass)
                MapControl(screen, label, 1206, 790, 354, EnterAcademyClass, true);
            else BookText(screen, label, 1206, 792, 354, 45, 26, paper, true, TextAlignmentOptions.Right);
            string status = CanEnterAcademyClass ? "YOUR CLASS IS READY" :
                state.phase == "arrival" || state.phase == "morning2" ? "RETURN HOME BEFORE CLASS" : "CLASS COMPLETE  ·  GROUNDS OPEN";
            BookText(screen, status, 1190, 850, 370, 30, 14, warmGold, false, TextAlignmentOptions.Right);
        }
    }
}
