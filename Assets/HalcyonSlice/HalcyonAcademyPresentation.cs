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

            Ornament(screen, "Academy upper vignette", 0, 0, 1600, 268, HalcyonOrnament.Shape.FadeDown, new Color(.13f, .08f, .13f, .84f));
            Ornament(screen, "Academy lower vignette", 0, 690, 1600, 210, HalcyonOrnament.Shape.FadeUp, new Color(.13f, .08f, .13f, .84f));
            InkLink(screen, "←  District", 35, 27, 164, ReturnFromAcademy);
            RunningTitle(screen, "The Academy", "Forecourt", 37, 83, 550);
            BookText(screen, TimeLabel().Replace(" / ", "  ·  "), 1100, 34, 452, 35, 23, cream, true, TextAlignmentOptions.Right);
            InkLink(screen, "Settings", 1285, 82, 130, () => OpenModal("settings"));
            InkLink(screen, "Pause", 1430, 82, 122, () => OpenModal("pause"));

            DrawInstrument(screen, 22, 646, 223);
            BookText(screen, "Molly's barometer", 29, 858, 222, 32, 21, cream, true, TextAlignmentOptions.Center);
            Stationery(screen, "Forecourt visitor's guide", 290, 778, 1262, 104);
            InkLink(screen, "Overview", 311, 792, 134, () => courtyard.Overview(), true);
            InkLink(screen, "Entrance", 458, 792, 134, () => courtyard.FocusEntrance(), true);
            InkLink(screen, "Clocktower", 605, 792, 154, () => courtyard.FocusTower(), true);
            InkLink(screen, "−", 793, 792, 48, () => courtyard.ChangeZoom(-.2f), true);
            InkLink(screen, "+", 848, 792, 48, () => courtyard.ChangeZoom(.2f), true);
            BookText(screen, "Scroll to explore   ·   + / − to zoom   ·   Home to reset", 322, 847, 830, 28, 20, fadedInk);
            Hairline(screen, 1178, 797, 1178, 865, ruleInk);

            if (CanEnterAcademyClass)
                MapControl(screen, "Enter the classroom  →", 1200, 796, 330, EnterAcademyClass, true);
            else BookText(screen, "The grounds are open", 1195, 797, 335, 40, 28, plum, true, TextAlignmentOptions.Center);
            string status = CanEnterAcademyClass ? "Your class is ready" :
                state.phase == "arrival" || state.phase == "morning2" ? "Return home before class" : "Class complete";
            BookText(screen, status, 1200, 849, 330, 28, 21, fadedInk, true, TextAlignmentOptions.Center);
        }
    }
}
