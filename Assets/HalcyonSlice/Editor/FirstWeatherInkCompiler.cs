using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Halcyon.FirstWeather.Editor
{
    [InitializeOnLoad]
    public class FirstWeatherInkCompiler : AssetPostprocessor
    {
        const string Source = "Assets/HalcyonSlice/Narrative/FirstWeather.ink";
        const string Output = "Assets/HalcyonSlice/Resources/HalcyonNarrative/FirstWeatherCompiled.json";
        static FirstWeatherInkCompiler() { EditorApplication.delayCall += Compile; }
        static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] moved, string[] movedFrom)
        { if (Array.IndexOf(imported, Source) >= 0) EditorApplication.delayCall += Compile; }
        [MenuItem("Halcyon/Compile story")]
        public static void Compile()
        {
            if (!File.Exists(Source)) return;
            var errors = new List<string>();
            var compiler = new Ink.Compiler(File.ReadAllText(Source), new Ink.Compiler.Options {
                sourceFilename = Source,
                errorHandler = (message, type) => { if (type == Ink.ErrorType.Error) errors.Add(message); else Debug.LogWarning(message); }
            });
            var story = compiler.Compile();
            if (story == null || errors.Count > 0) { Debug.LogError("HALCYON INK: " + string.Join("\n", errors)); return; }
            string json = story.ToJson();
            if (!File.Exists(Output) || File.ReadAllText(Output) != json)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Output)); File.WriteAllText(Output, json);
                AssetDatabase.ImportAsset(Output);
                Debug.Log("HALCYON INK: story compiled successfully.");
            }
        }
    }
}
