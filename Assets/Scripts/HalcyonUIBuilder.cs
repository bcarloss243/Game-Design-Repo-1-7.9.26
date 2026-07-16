using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

// HOW TO USE:
// 1. Put this file anywhere under Assets/ (e.g. Assets/Scripts/).
// 2. In your scene, create an empty GameObject and add this component to it.
// 3. In the Inspector, right-click the "Halcyon UI Builder" component header
//    and choose "Build Halcyon UI".
// 4. The Canvas, header, gauge, and activity cards appear in the Hierarchy as
//    normal objects. Save the scene (Cmd+S) to keep them, then commit.
// You can delete the empty builder GameObject afterward; the UI it made stays.
//
// PREREQUISITE: import TMP essentials once via
// Window > TextMeshPro > Import TMP Essential Resources, or the text won't show.
public class HalcyonUIBuilder : MonoBehaviour
{
    [ContextMenu("Build Halcyon UI")]
    public void BuildUI()
    {
        if (GameObject.Find("HalcyonCanvas") != null)
        {
            Debug.LogWarning("HalcyonCanvas already exists. Delete it first if you want to rebuild.");
            return;
        }

        Canvas canvas = CreateCanvas();
        EnsureEventSystem();
        CreateHeader(canvas.transform);        // Element 1: text
        CreateGauge(canvas.transform);         // Element 2: image + needle
        CreateActivityStack(canvas.transform); // Element 3: buttons with text

        Debug.Log("Halcyon UI built. Save the scene to keep it.");
    }

    Canvas CreateCanvas()
    {
        GameObject go = new GameObject("HalcyonCanvas");
        Canvas canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        go.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }
    }

    // Element 1: day and time header (contains text)
    void CreateHeader(Transform parent)
    {
        GameObject go = new GameObject("HeaderText", typeof(RectTransform));
        go.transform.SetParent(parent, false);
        TextMeshProUGUI t = go.AddComponent<TextMeshProUGUI>();
        t.text = "Day 3 — Morning";
        t.fontSize = 48;
        t.alignment = TextAlignmentOptions.Center;
        RectTransform rt = (RectTransform)go.transform;
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.sizeDelta = new Vector2(800, 80);
        rt.anchoredPosition = new Vector2(0, -40);
    }

    // Element 2: Pressure Gauge (dial image, needle that pivots at its base, portrait)
    void CreateGauge(Transform parent)
    {
        GameObject gauge = MakeImage("PressureGauge", parent, new Color(0.72f, 0.55f, 0.30f));
        RectTransform g = (RectTransform)gauge.transform;
        g.anchorMin = Vector2.zero;
        g.anchorMax = Vector2.zero;
        g.pivot = Vector2.zero;
        g.sizeDelta = new Vector2(300, 300);
        g.anchoredPosition = new Vector2(80, 80);

        GameObject needle = MakeImage("Needle", gauge.transform, new Color(0.15f, 0.15f, 0.15f));
        RectTransform n = (RectTransform)needle.transform;
        n.anchorMin = new Vector2(0.5f, 0.5f);
        n.anchorMax = new Vector2(0.5f, 0.5f);
        n.pivot = new Vector2(0.5f, 0f);   // rotate localEulerAngles.z to sweep the needle
        n.sizeDelta = new Vector2(8, 130);
        n.anchoredPosition = Vector2.zero;

        GameObject portrait = MakeImage("Portrait", gauge.transform, new Color(0.85f, 0.85f, 0.9f));
        RectTransform p = (RectTransform)portrait.transform;
        p.anchorMin = new Vector2(0.5f, 0.5f);
        p.anchorMax = new Vector2(0.5f, 0.5f);
        p.pivot = new Vector2(0.5f, 0.5f);
        p.sizeDelta = new Vector2(90, 110);
        p.anchoredPosition = Vector2.zero;
    }

    // Element 3: activity cards (buttons with a text label) in a vertical layout
    void CreateActivityStack(Transform parent)
    {
        GameObject stack = new GameObject("ActivityStack", typeof(RectTransform));
        stack.transform.SetParent(parent, false);
        RectTransform s = (RectTransform)stack.transform;
        s.anchorMin = new Vector2(1f, 1f);
        s.anchorMax = new Vector2(1f, 1f);
        s.pivot = new Vector2(1f, 1f);
        s.sizeDelta = new Vector2(320, 0);
        s.anchoredPosition = new Vector2(-60, -140);

        VerticalLayoutGroup vlg = stack.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 12;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        ContentSizeFitter fitter = stack.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        string[] labels = { "Morning class", "Visit Lola", "Greenhouse", "Rest" };
        foreach (string label in labels)
            MakeButton("Card_" + label.Replace(" ", ""), stack.transform, label);
    }

    GameObject MakeButton(string name, Transform parent, string label)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        Image bg = go.AddComponent<Image>();
        bg.color = new Color(0.95f, 0.95f, 0.95f);
        go.AddComponent<Button>();
        LayoutElement le = go.AddComponent<LayoutElement>();
        le.preferredHeight = 58;
        le.minHeight = 58;

        GameObject labelGo = new GameObject("Label", typeof(RectTransform));
        labelGo.transform.SetParent(go.transform, false);
        TextMeshProUGUI t = labelGo.AddComponent<TextMeshProUGUI>();
        t.text = label;
        t.fontSize = 24;
        t.color = Color.black;
        t.alignment = TextAlignmentOptions.Center;
        RectTransform lt = (RectTransform)labelGo.transform;
        lt.anchorMin = Vector2.zero;
        lt.anchorMax = Vector2.one;
        lt.offsetMin = new Vector2(12, 6);
        lt.offsetMax = new Vector2(-12, -6);

        return go;
    }

    GameObject MakeImage(string name, Transform parent, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        go.AddComponent<Image>().color = color;
        return go;
    }
}
