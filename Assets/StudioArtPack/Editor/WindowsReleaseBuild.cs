using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
namespace BalloonBarrage.EditorTools {
 public static class WindowsReleaseBuild {
  public static void Build(){
   if(!Application.isBatchMode)throw new Exception("Use an isolated batch project.");
   var output=Environment.GetEnvironmentVariable("BALLOON_BUILD_OUTPUT");
   if(string.IsNullOrWhiteSpace(output)||!Path.IsPathRooted(output))throw new Exception("Set an absolute BALLOON_BUILD_OUTPUT.");
   if(Directory.Exists(output)&&Directory.GetFileSystemEntries(output).Length>0)throw new Exception("Build output must be empty.");
   bool diagnostic=Environment.GetEnvironmentVariable("BALLOON_DIAGNOSTIC_BUILD")=="1";
   PlayerSettings.companyName=diagnostic?"BalloonBarrageDiagnostics":"DefaultCompany";
   PlayerSettings.productName=diagnostic?"ReleaseStress20261018":"BalloonShooter";
   PlayerSettings.bundleVersion="0.1.0-demo.2";
   PlayerSettings.enableFrameTimingStats=true;
   var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions {
    scenes=new[]{"Assets/Scenes/PrototypeScene.unity"},
    locationPathName=Path.Combine(output,"BalloonShooter.exe"),
    target=BuildTarget.StandaloneWindows64,options=BuildOptions.None
   });
   var summary=report.summary;
   string reportPath=Environment.GetEnvironmentVariable("BALLOON_BUILD_REPORT");
   if(!string.IsNullOrEmpty(reportPath))File.WriteAllText(reportPath,
    "result="+summary.result+" errors="+summary.totalErrors+" warnings="+summary.totalWarnings+" options="+summary.options+" seconds="+summary.totalTime.TotalSeconds+" diagnostic="+diagnostic);
   EditorApplication.Exit(summary.result==BuildResult.Succeeded?0:1);
  }
 }
}
