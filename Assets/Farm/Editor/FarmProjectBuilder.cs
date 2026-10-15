using System;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Orchard.Editor
{
    public static class FarmProjectBuilder
    {
        [MenuItem("Orchard/Generate Farm Scene")]
        public static void Generate()
        {
            if (!File.Exists("Assets/TextMesh Pro/Resources/TMP Settings.asset"))
            {
                AssetDatabase.importPackageCompleted += EssentialsImported;
                TMP_PackageResourceImporter.ImportResources(true, false, false);
                return; // Package import completes on later Editor ticks; do not force -quit.
            }

            FarmSceneBuilder.Create();
            FarmPlayerConfiguration.Configure();
            AssetDatabase.SaveAssets();
            Debug.Log("[Orchard] Farm scene and Android settings generated.");
            if (Application.isBatchMode)
            {
                EditorApplication.Exit(0);
            }
        }

        private static void EssentialsImported(string package)
        {
            AssetDatabase.importPackageCompleted -= EssentialsImported;
            EditorApplication.delayCall += Generate;
        }

        [MenuItem("Orchard/Build Android APK")]
        public static void BuildAndroid()
        {
            if (!File.Exists(FarmSceneBuilder.ScenePath))
            {
                FarmSceneBuilder.Create();
            }

            FarmPlayerConfiguration.Configure();
            EditorUserBuildSettings.buildAppBundle = false;
            EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
            string output = GetOutput("Builds/Android/VuonNho.apk");
            Directory.CreateDirectory(Path.GetDirectoryName(output));
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions { scenes = new[] { FarmSceneBuilder.ScenePath }, locationPathName = output, target = BuildTarget.Android, options = BuildOptions.None });
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new InvalidOperationException("Android build failed: " + report.summary.result);
            }

            Debug.Log("[Orchard] APK ready: " + output);
        }

        public static void BuildMacVerification()
        {
            string output = GetOutput("Builds/Mac/VuonNho.app");
            Directory.CreateDirectory(Path.GetDirectoryName(output));
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.Mono2x);
            var report = BuildPipeline.BuildPlayer(new[] { FarmSceneBuilder.ScenePath }, output, BuildTarget.StandaloneOSX, BuildOptions.Development);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new InvalidOperationException("Verification build failed");
            }
        }

        private static string GetOutput(string fallback)
        {
            var args = Environment.GetCommandLineArgs();
            for (int i = 0; i + 1 < args.Length; i++)
            {
                if (args[i] == "-buildOutput")
                {
                    return args[i + 1];
                }
            }

            return fallback;
        }
    }
}
