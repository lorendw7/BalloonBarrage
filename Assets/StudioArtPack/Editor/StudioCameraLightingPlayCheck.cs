using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

// Opt-in editor acceptance check. Synthetic mouse input is not a human playtest.
[InitializeOnLoad]
public static class StudioCameraLightingPlayCheck
{
    const string Enabled = "CameraLightingPlayCheck.Enabled";
    const string Stage = "CameraLightingPlayCheck.Stage";
    const string Index = "CameraLightingPlayCheck.Index";
    static double started, deadline;
    static int targetId;
    static Vector2 aim;
    static Mouse mouse;

    static StudioCameraLightingPlayCheck()
    {
        if (SessionState.GetBool(Enabled, false)) EditorApplication.update += Tick;
    }

    public static void RunBatch()
    {
        if (!Application.isBatchMode) throw new InvalidOperationException("Use an isolated batch project without -quit.");
        SessionState.SetBool(Enabled, true); SessionState.SetInt(Index, 0);
        StudioCameraLightingPass.RegisterCopiedScene();
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        File.WriteAllText(StudioCameraLightingPass.Root + "/PlayCheck.txt",
            "Automated Play Mode check with synthetic mouse input; no human playtest or release acceptance.\n");
        Prepare();
    }

    static void Prepare()
    {
        EditorSceneManager.OpenScene(SessionState.GetInt(Index, 0) == 0 ?
            StudioCameraLightingPass.Source : StudioCameraLightingPass.Destination);
        SessionState.SetInt(Stage, 0);
        EditorApplication.EnterPlaymode();
    }

    static void Tick()
    {
        if (!SessionState.GetBool(Enabled, false)) return;
        try
        {
            int stage = SessionState.GetInt(Stage, 0);
            if (!EditorApplication.isPlaying)
            {
                if (stage != 6 || EditorApplication.isPlayingOrWillChangePlaymode) return;
                if (SessionState.GetInt(Index, 0) == 0)
                {
                    SessionState.SetInt(Index, 1); Prepare(); return;
                }
                SessionState.SetBool(Enabled, false);
                Debug.Log("CAMERA_LIGHTING_PLAY_OK: source and copied scene; three natural spawns, projectile hit/score, respawn, retry. See PlayCheck.txt for mouse-input limitations.");
                EditorApplication.Exit(0); return;
            }
            if (stage == 6) return;
            if (stage == 0)
            {
                if (Time.frameCount < 8) return;
                Application.targetFrameRate = 60;
                if (RoundController.IsPlaying || Score.Value != 0) throw new InvalidOperationException("Start gate invalid.");
                UnityEngine.Random.InitState(104729);
                InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
                foreach (var nativeMouse in InputSystem.devices.OfType<Mouse>().ToArray()) InputSystem.DisableDevice(nativeMouse);
                mouse = InputSystem.AddDevice<Mouse>("CameraLightingSyntheticMouse");
                mouse.MakeCurrent();
                UnityEngine.Object.FindFirstObjectByType<RoundController>().BeginRound();
                started = EditorApplication.timeSinceStartup; deadline = started + 20;
                SessionState.SetInt(Stage, 1); return;
            }
            if (EditorApplication.timeSinceStartup > deadline) throw new TimeoutException("Play check timed out at stage " + stage);
            var balloons = UnityEngine.Object.FindObjectsByType<Balloon>(FindObjectsSortMode.None);
            if (balloons.Length > 3) throw new InvalidOperationException("Three-balloon cap exceeded.");
            if (stage == 1)
            {
                if (balloons.Length != 3) return;
                string label = SessionState.GetInt(Index, 0) == 0 ? "Before_Runtime" : "After_Runtime";
                StudioCameraLightingPass.Capture(SceneManager.GetActiveScene(), label);
                var balloon = balloons.OrderBy(b => Mathf.Abs(b.transform.position.x)).First();
                targetId = balloon.GetInstanceID();
                var body = balloon.GetComponentsInChildren<Collider>().First(c => !c.isTrigger && c.enabled);
                Vector3 screen = Camera.main.WorldToScreenPoint(body.bounds.center);
                aim = new Vector2(screen.x, screen.y);
                InputSystem.QueueStateEvent(mouse, new MouseState { position = aim, buttons = 0 });
                started = EditorApplication.timeSinceStartup;
                SessionState.SetInt(Stage, 2); return;
            }
            if (stage == 2)
            {
                if (EditorApplication.timeSinceStartup - started < .15) return;
                InputSystem.QueueStateEvent(mouse, new MouseState { position = aim, buttons = 1 });
                started = EditorApplication.timeSinceStartup;
                SessionState.SetInt(Stage, 3); return;
            }
            if (stage == 3)
            {
                var tracked = balloons.FirstOrDefault(b => b.GetInstanceID() == targetId);
                if (tracked != null)
                {
                    var body = tracked.GetComponentsInChildren<Collider>().First(c => !c.isTrigger && c.enabled);
                    Vector3 screen = Camera.main.WorldToScreenPoint(body.bounds.center);
                    aim = new Vector2(screen.x, screen.y);
                    InputSystem.QueueStateEvent(mouse, new MouseState { position = aim, buttons = 1 });
                }
                if (EditorApplication.timeSinceStartup - started < 1.2) return;
                InputSystem.QueueStateEvent(mouse, new MouseState { position = aim, buttons = 0 });
                if (Score.Value < 10 || balloons.Any(b => b.GetInstanceID() == targetId))
                {
                    var shooter = UnityEngine.Object.FindFirstObjectByType<PlayerShooter>();
                    string limitation = "Synthetic mouse check did not fire/hit: playing=" + RoundController.IsPlaying +
                        ", shooterEnabled=" + shooter.enabled + ", pointerOverUI=" +
                        (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) +
                        ", projectiles=" + UnityEngine.Object.FindObjectsByType<PaintProjectile>(FindObjectsSortMode.None).Length +
                        ", currentMouseMatches=" + (Mouse.current == mouse) + ".\n";
                    File.AppendAllText(StudioCameraLightingPass.Root + "/PlayCheck.txt", limitation);
                    Debug.LogWarning(limitation);
                    // Check the existing projectile/collision/score chain independently of editor/UI input.
                    var so = new SerializedObject(shooter);
                    var muzzle = (Transform)so.FindProperty("muzzle").objectReferenceValue;
                    var projectile = (PaintProjectile)so.FindProperty("paintProjectilePrefab").objectReferenceValue;
                    var target = balloons.First(b => b.GetInstanceID() == targetId);
                    var collider = target.GetComponentsInChildren<Collider>().First(c => !c.isTrigger && c.enabled);
                    var velocity = target.GetComponent<Rigidbody>().linearVelocity;
                    float speed = new SerializedObject(projectile).FindProperty("speed").floatValue;
                    var aimPoint = collider.bounds.center + velocity * (Vector3.Distance(muzzle.position, collider.bounds.center) / speed);
                    UnityEngine.Object.Instantiate(projectile, muzzle.position, Quaternion.LookRotation(aimPoint - muzzle.position));
                    started = EditorApplication.timeSinceStartup;
                    SessionState.SetInt(Stage, 7); return;
                }
                File.AppendAllText(StudioCameraLightingPass.Root + "/PlayCheck.txt", "Synthetic mouse shot: passed.\n");
                started = EditorApplication.timeSinceStartup;
                SessionState.SetInt(Stage, 4); return;
            }
            if (stage == 7)
            {
                if (EditorApplication.timeSinceStartup - started < 1.2) return;
                if (Score.Value < 10 || balloons.Any(b => b.GetInstanceID() == targetId))
                    throw new InvalidOperationException("Existing projectile did not hit/pop/score in the isolated check.");
                File.AppendAllText(StudioCameraLightingPass.Root + "/PlayCheck.txt", "Programmatic launch of unchanged projectile prefab: hit/pop/score passed. Mouse input remains unverified.\n");
                started = EditorApplication.timeSinceStartup;
                SessionState.SetInt(Stage, 4); return;
            }
            if (stage == 4)
            {
                if (balloons.Length != 3 || EditorApplication.timeSinceStartup - started < 1) return;
                var name = SessionState.GetInt(Index, 0) == 0 ? "Source" : "CameraLighting copy";
                File.AppendAllText(StudioCameraLightingPass.Root + "/PlayCheck.txt",
                    name + ": three natural spawns; aimed target popped; score " +
                    Score.Value + "; three targets restored by normal respawn.\n");
                Debug.Log("CAMERA_LIGHTING_PLAY_SCENE_OK " + name + " score=" + Score.Value);
                UnityEngine.Object.FindFirstObjectByType<RoundController>().RestartRound();
                started = EditorApplication.timeSinceStartup;
                SessionState.SetInt(Stage, 5);
                return;
            }
            if (stage == 5)
            {
                if (EditorApplication.timeSinceStartup - started < .5) return;
                if (RoundController.IsPlaying || Score.Value != 0 || balloons.Length != 0)
                    throw new InvalidOperationException("Retry did not restore the start gate.");
                File.AppendAllText(StudioCameraLightingPass.Root + "/PlayCheck.txt", "Retry: start gate restored, score zero, no targets.\n");
                SessionState.SetInt(Stage, 6); EditorApplication.ExitPlaymode();
            }
        }
        catch (Exception ex)
        {
            SessionState.SetBool(Enabled, false);
            File.AppendAllText(StudioCameraLightingPass.Root + "/PlayCheck.txt", "FAILED: " + ex + "\n");
            Debug.LogException(ex); EditorApplication.Exit(1);
        }
    }
}
