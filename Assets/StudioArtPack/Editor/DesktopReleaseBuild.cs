using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace BalloonBarrage.EditorTools
{
    public static class DesktopReleaseBuild
    {
        private const string ScenePath = "Assets/Scenes/PrototypeScene.unity";

        public static void Build()
        {
            if (!Application.isBatchMode)
            {
                throw new InvalidOperationException("Desktop release builds must run in batch mode from an isolated checkout.");
            }

            string targetName = RequiredArgument("-balloonTarget").ToLowerInvariant();
            string version = RequiredArgument("-balloonVersion");
            string output = Path.GetFullPath(RequiredArgument("-balloonOutput"));

            if (Directory.Exists(output) && Directory.GetFileSystemEntries(output).Length > 0)
            {
                throw new InvalidOperationException("Build output must be empty: " + output);
            }

            Directory.CreateDirectory(output);

            string previousCompany = PlayerSettings.companyName;
            string previousProduct = PlayerSettings.productName;
            string previousVersion = PlayerSettings.bundleVersion;
            int previousArchitecture = PlayerSettings.GetArchitecture(NamedBuildTarget.Standalone);

            try
            {
                PlayerSettings.companyName = "BalloonBarrage";
                PlayerSettings.productName = "BalloonBarrage";
                PlayerSettings.bundleVersion = version;
                PlayerSettings.enableFrameTimingStats = true;

                BuildTarget target;
                string location;

                switch (targetName)
                {
                    case "windows":
                        target = BuildTarget.StandaloneWindows64;
                        location = Path.Combine(output, "BalloonShooter.exe");
                        break;
                    case "macos":
                        target = BuildTarget.StandaloneOSX;
                        PlayerSettings.SetArchitecture(NamedBuildTarget.Standalone, 2); // Universal: Intel 64-bit + Apple silicon.
                        location = Path.Combine(output, "BalloonBarrage.app");
                        break;
                    default:
                        throw new ArgumentException("Unsupported -balloonTarget: " + targetName);
                }

                BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
                {
                    scenes = new[] { ScenePath },
                    locationPathName = location,
                    target = target,
                    options = BuildOptions.None
                });

                BuildSummary summary = report.summary;
                string reportPath = Path.Combine(output, "BuildReport.txt");
                File.WriteAllText(reportPath,
                    "result=" + summary.result + Environment.NewLine +
                    "target=" + target + Environment.NewLine +
                    "version=" + version + Environment.NewLine +
                    "errors=" + summary.totalErrors + Environment.NewLine +
                    "warnings=" + summary.totalWarnings + Environment.NewLine +
                    "bytes=" + summary.totalSize + Environment.NewLine +
                    "seconds=" + summary.totalTime.TotalSeconds.ToString("F3") + Environment.NewLine);

                if (summary.result != BuildResult.Succeeded)
                {
                    throw new InvalidOperationException("Unity desktop build failed: " + summary.result);
                }
            }
            finally
            {
                PlayerSettings.companyName = previousCompany;
                PlayerSettings.productName = previousProduct;
                PlayerSettings.bundleVersion = previousVersion;
                PlayerSettings.SetArchitecture(NamedBuildTarget.Standalone, previousArchitecture);
            }
        }

        private static string RequiredArgument(string name)
        {
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (string.Equals(args[i], name, StringComparison.OrdinalIgnoreCase) &&
                    !string.IsNullOrWhiteSpace(args[i + 1]))
                {
                    return args[i + 1];
                }
            }

            throw new ArgumentException("Missing required command-line argument " + name + ".");
        }
    }
}
