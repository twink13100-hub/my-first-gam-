using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;

public static class BuildScript
{
    public static void BuildAndroid()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        PlayerSettings.companyName = "CrashDriveStudio";
        PlayerSettings.productName = "CrashDrive 3D";
        PlayerSettings.applicationIdentifier = "com.crashdrive3d.mobile";
        PlayerSettings.bundleVersion = "0.1.0";
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        string scene = "Assets/Scenes/Main.unity";
        if (!File.Exists(scene)) throw new FileNotFoundException("Missing scene: " + scene);
        Directory.CreateDirectory("build");
        var options = new BuildPlayerOptions {
            scenes = new[] { scene },
            locationPathName = "build/CrashDrive3D.apk",
            target = BuildTarget.Android,
            options = BuildOptions.None
        };
        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
            throw new System.Exception("APK build failed: " + report.summary.result + ", errors: " + report.summary.totalErrors);
        Debug.Log("APK built at " + options.locationPathName);
    }
}
