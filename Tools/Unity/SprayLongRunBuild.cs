// Copy into Assets/Editor of the diagnostic project, alongside the runtime probe.
#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;

public static class SprayLongRunBuild
{
    public static void Run()
    {
        string[] args = Environment.GetCommandLineArgs();
        string output = Path.GetFullPath(args[Array.IndexOf(args, "-probeBuildOutput") + 1]);
        if (Directory.Exists(output) && Directory.GetFileSystemEntries(output).Length != 0) throw new Exception("Use a new diagnostic output.");
        Directory.CreateDirectory(output);
        string company = PlayerSettings.companyName, product = PlayerSettings.productName;
        bool frameTimings = PlayerSettings.enableFrameTimingStats;
        try
        {
            PlayerSettings.companyName = "BalloonBarrage.Diagnostics";
            PlayerSettings.productName = "SprayLongRunProbe";
            PlayerSettings.enableFrameTimingStats = true;
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
                scenes = new[] { "Assets/Scenes/PrototypeScene.unity" },
                locationPathName = Path.Combine(output, "SprayLongRunProbe.exe"),
                target = BuildTarget.StandaloneWindows64, options = BuildOptions.None });
            File.WriteAllText(Path.Combine(output, "BuildReport.txt"),
                $"result={report.summary.result}\nerrors={report.summary.totalErrors}\nwarnings={report.summary.totalWarnings}\nseconds={report.summary.totalTime.TotalSeconds:F3}\n");
            if (report.summary.result != BuildResult.Succeeded || report.summary.totalErrors != 0 || report.summary.totalWarnings != 0)
                throw new Exception("Diagnostic build must succeed without errors or warnings.");
        }
        finally
        {
            PlayerSettings.companyName = company;
            PlayerSettings.productName = product;
            PlayerSettings.enableFrameTimingStats = frameTimings;
            AssetDatabase.SaveAssets();
        }
    }
}
#endif
