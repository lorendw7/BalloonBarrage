using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace BalloonBarrage.EditorTools
{
    public static class WindowsDevelopmentBuild
    {
        public static void Build()
        {
            string output = Environment.GetEnvironmentVariable("BALLOON_BUILD_OUTPUT");
            if (string.IsNullOrWhiteSpace(output))
                throw new InvalidOperationException("Set BALLOON_BUILD_OUTPUT to an empty output directory.");
            output = Path.GetFullPath(output);
            if (Directory.Exists(output) && Directory.GetFileSystemEntries(output).Length != 0)
                throw new InvalidOperationException("Build output must be empty to avoid mixing versions.");
            Directory.CreateDirectory(output);
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Scenes/PrototypeScene.unity" },
                locationPathName = Path.Combine(output, "BalloonBarrage.exe"),
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.Development
            });
            if (report.summary.result != BuildResult.Succeeded)
                throw new InvalidOperationException("Windows development build failed: " + report.summary.result);
            File.Copy("CREDITS.md", Path.Combine(output, "CREDITS.md"), true);
            File.WriteAllText(Path.Combine(output, "README.txt"),
                "BalloonBarrage noncommercial development snapshot\n" +
                "Click Start, aim with the mouse and hold left click to fire. Play Again returns to the start menu.\n" +
                "Each round plays the complete Goin' Under song (about 123.429 seconds), from the beginning to natural settlement.\n" +
                "The Beginner chart schedules 93 target events. Ordinary hits award 10 points; on-beat shots hitting their matching target award 20.\n" +
                "Esc pauses; Resume continues; Quit or Alt+F4 exits. Menus use original studio music; gameplay uses the imported song.\n" +
                "See BalloonBarrage_Data/StreamingAssets/Rhythm/StepMania/NOTICE.txt for the original songs/charts and notices.\n" +
                "Included songs carry upstream noncommercial restrictions. Do not sell this package.\n" +
                "This is not an exhibition-ready release. Target-PC playtesting remains required.\n");
            UnityEngine.Debug.Log("BALLOON_WINDOWS_BUILD_OK: " + output);
        }
    }
}
