#if UNITY_EDITOR
using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

/// <summary>Expand real runtime dropdowns and check generated text geometry, not just option strings.</summary>
[InitializeOnLoad]
public static class DropdownVisualValidation
{
    const string Active = "DropdownVisualValidation.Active";
    static readonly string[] names = { "SongMode", "LocalPlayerLayout", "SongRules", "SongList" };
    static int stage, index, assertions;
    static double wait, deadline;
    static UILanguage language;
    static Dropdown dropdown;
    static string diagnostic = "";
    static DropdownVisualValidation() { if (SessionState.GetBool(Active, false)) EditorApplication.update += Tick; }
    public static void Run()
    {
        if (!Application.isBatchMode) throw new InvalidOperationException("Use a batch Editor.");
        SessionState.SetBool(Active, true);
        EditorSceneManager.OpenScene("Assets/Scenes/PrototypeScene.unity");
        EditorApplication.update -= Tick; EditorApplication.update += Tick;
        EditorApplication.EnterPlaymode();
    }
    static void Tick()
    {
        if (!EditorApplication.isPlaying || !SessionState.GetBool(Active, false)) return;
        try
        {
            var round = Object.FindFirstObjectByType<RoundController>();
            if (round == null || Time.frameCount < 8 || EditorApplication.timeSinceStartup < wait) return;
            if (stage == 0)
            {
                language = LanguageSettings.Current; round.BeginRound();
                deadline = EditorApplication.timeSinceStartup + 150; stage = 1; wait = EditorApplication.timeSinceStartup + .2; return;
            }
            if (EditorApplication.timeSinceStartup > deadline) throw new TimeoutException("Dropdown checks timed out.");
            if (stage == 1)
            {
                if (index == 12) { Finish(null); return; }
                LanguageSettings.SetLanguage((UILanguage)(index / 4));
                typeof(RoundController).GetMethod("RefreshSongSelectionText", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(round, null);
                dropdown = GameObject.Find(names[index % 4]).GetComponent<Dropdown>();
                dropdown.Show(); Canvas.ForceUpdateCanvases();
                wait = EditorApplication.timeSinceStartup + .2; stage = 2; return;
            }
            if (stage == 2)
            {
                Canvas.ForceUpdateCanvases();
                GameObject list = GameObject.Find("Dropdown List"); Require(list != null, "Expanded list is missing.");
                var toggles = list.GetComponentsInChildren<Toggle>();
                Require(toggles.Length == dropdown.options.Count, "Expanded option count differs.");
                var viewport = list.GetComponentInChildren<ScrollRect>().viewport;
                int visible = 0;
                foreach (Toggle toggle in toggles)
                {
                    Text text = toggle.GetComponentInChildren<Text>();
                    var corners = new Vector3[4]; text.rectTransform.GetWorldCorners(corners);
                    Vector3 middle = (corners[0] + corners[2]) * .5f;
                    if (!RectTransformUtility.RectangleContainsScreenPoint(viewport, RectTransformUtility.WorldToScreenPoint(null, middle))) continue;
                    visible++;
                    var mesh = text.canvasRenderer.GetMesh(); int vertices = mesh != null ? mesh.vertexCount : 0;
                    diagnostic = names[index % 4] + " / " + LanguageSettings.Current + ": height=" + text.rectTransform.rect.height + ", preferred=" + text.preferredHeight + ", vertices=" + vertices;
                    bool rendered = vertices > 0;
                    if (!rendered) Capture("DropdownBeforeFix-" + index);
                    Require(rendered && !string.IsNullOrWhiteSpace(text.text), "Option text is invisible: " + diagnostic);
                    Require(text.preferredHeight <= text.rectTransform.rect.height + 1f, "Option row clips its font: " + diagnostic);
                }
                Require(visible >= Math.Min(2, toggles.Length), "Too few visible options.");
                if (index < 4) Capture("DropdownFixed-" + names[index % 4]);
                // Exercise the actual option callback. Selecting a layout needs no attached pad.
                toggles[Math.Min(1, toggles.Length - 1)].isOn = true;
                Require(dropdown.value == Math.Min(1, toggles.Length - 1), "Expanded option did not select.");
                dropdown.Hide(); wait = EditorApplication.timeSinceStartup + .25; index++; stage = 1;
            }
        }
        catch (Exception error) { Finish(error); }
    }
    static void Capture(string name)
    {
        typeof(KyudaisaiRuntimeSmokeCheck).GetMethod("Capture", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { name });
    }
    static void Require(bool value, string message) { if (!value) throw new InvalidOperationException(message); assertions++; }
    static void Finish(Exception error)
    {
        SessionState.SetBool(Active, false); LanguageSettings.SetLanguage(language); PlayerPrefs.Save();
        Directory.CreateDirectory("Logs");
        File.WriteAllText("Logs/DropdownVisualRuntime.json", JsonUtility.ToJson(new Evidence { status = error == null ? "passed" : "failed", error = error?.ToString(), assertions = assertions, diagnostic = diagnostic }, true));
        if (error == null) Debug.Log("DROPDOWN_VISUAL_RUNTIME_OK: " + assertions); else Debug.LogException(error);
        EditorApplication.Exit(error == null ? 0 : 1);
    }
    [Serializable] class Evidence
    {
        public string status, error, diagnostic; public int assertions;
        public string scope = "Controlled Editor expansion and option callback, all four runtime dropdowns across three locales; visible text mesh/line height and bounded scrolling viewport. No native pointer/controller acceptance.";
    }
}
#endif
