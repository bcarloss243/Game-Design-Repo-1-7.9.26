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
            V03Verification.Rules();
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
            string output = Path.GetFullPath("Builds/FirstWeather-Mac-v0.3.0");
            Directory.CreateDirectory(output);
            string product = PlayerSettings.productName;
            string company = PlayerSettings.companyName, version = PlayerSettings.bundleVersion;
            string identifier = PlayerSettings.GetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.Standalone);
            int width = PlayerSettings.defaultScreenWidth, height = PlayerSettings.defaultScreenHeight;
            var mode = PlayerSettings.fullScreenMode;
            bool resize = PlayerSettings.resizableWindow;
            try
            {
                PlayerSettings.productName = "Halcyon Academy - First Weather";
                PlayerSettings.companyName = "Bergen Carloss";
                PlayerSettings.bundleVersion = "0.3.0";
                PlayerSettings.SetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.Standalone, "com.bergencarloss.halcyon.firstweather");
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
                PlayerSettings.companyName = company; PlayerSettings.bundleVersion = version;
                PlayerSettings.SetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.Standalone, identifier);
                PlayerSettings.defaultScreenWidth = width; PlayerSettings.defaultScreenHeight = height;
                PlayerSettings.fullScreenMode = mode; PlayerSettings.resizableWindow = resize;
            }
        }
    }
}
