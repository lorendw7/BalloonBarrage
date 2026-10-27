using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using Object = UnityEngine.Object;

namespace BalloonBarrage.EditorTools
{
    /// <summary>
    /// Method-driven Play Mode coverage for the continuous spray presentation.
    /// Synthetic input is not physical-device or human feel acceptance.
    /// </summary>
    [InitializeOnLoad]
    public static class ExhibitionSprayPlayCheck
    {
        private const string EnabledKey =
            "ExhibitionSprayPlayCheck.Enabled";
        private const string StageKey =
            "ExhibitionSprayPlayCheck.Stage";
        private const string ScenePath =
            "Assets/Scenes/PrototypeScene.unity";

        private static readonly string OutputDirectory =
            Path.GetFullPath(Path.Combine(
                Application.dataPath,
                "../.local-reference/exhibition-spray-polish"));

        private static Mouse syntheticMouse;
        private static Vector2 aimPosition;
        private static double stageStarted;
        private static double deadline;

        static ExhibitionSprayPlayCheck()
        {
            if (SessionState.GetBool(EnabledKey, false))
            {
                EditorApplication.update += Tick;
            }
        }

        public static void RunBatch()
        {
            if (!Application.isBatchMode)
            {
                throw new InvalidOperationException(
                    "Run the spray Play Mode check in batch mode without -quit.");
            }

            Directory.CreateDirectory(OutputDirectory);
            EditorSceneManager.OpenScene(ScenePath);
            SessionState.SetBool(EnabledKey, true);
            SessionState.SetInt(StageKey, 0);
            EditorApplication.update -= Tick;
            EditorApplication.update += Tick;
            deadline = EditorApplication.timeSinceStartup + 45d;
            EditorApplication.EnterPlaymode();
        }

        private static void Tick()
        {
            if (!SessionState.GetBool(EnabledKey, false) ||
                !EditorApplication.isPlaying)
            {
                return;
            }

            try
            {
                int stage = SessionState.GetInt(StageKey, 0);

                if (stage == 0 && deadline <= 0d)
                {
                    deadline = EditorApplication.timeSinceStartup + 45d;
                }

                Require(EditorApplication.timeSinceStartup < deadline,
                    "Spray Play Mode check timed out.");

                PlayerShooter shooter = Object.FindFirstObjectByType<PlayerShooter>();

                if (stage == 0)
                {
                    if (Time.frameCount < 8 || shooter == null)
                    {
                        return;
                    }

                    InputSystem.settings.backgroundBehavior =
                        InputSettings.BackgroundBehavior.IgnoreFocus;

                    foreach (Mouse mouse in InputSystem.devices
                                 .OfType<Mouse>()
                                 .ToArray())
                    {
                        InputSystem.DisableDevice(mouse);
                    }

                    syntheticMouse = InputSystem.AddDevice<Mouse>(
                        "ExhibitionSpraySyntheticMouse");
                    syntheticMouse.MakeCurrent();

                    Camera camera = Camera.main;
                    Require(camera != null, "Missing production camera.");

                    Vector3 screenPoint = camera.WorldToScreenPoint(
                        new Vector3(0f, 0f, 8f));
                    aimPosition = new Vector2(screenPoint.x, screenPoint.y);

                    Object.FindFirstObjectByType<RoundController>()
                        .BeginRound();
                    QueueMouseState(0);
                    InvokeShooterUpdate(shooter);

                    stageStarted = EditorApplication.timeSinceStartup;
                    SessionState.SetInt(StageKey, 1);
                    return;
                }

                if (stage == 1)
                {
                    if (EditorApplication.timeSinceStartup - stageStarted < 0.25d)
                    {
                        return;
                    }

                    QueueMouseState(1);
                    InvokeShooterUpdate(shooter);
                    stageStarted = EditorApplication.timeSinceStartup;
                    SessionState.SetInt(StageKey, 2);
                    return;
                }

                if (stage == 2)
                {
                    QueueMouseState(1);
                    InvokeShooterUpdate(shooter);

                    if (EditorApplication.timeSinceStartup - stageStarted < 0.45d)
                    {
                        return;
                    }

                    ParticleSystem spray = SprayOf(shooter);
                    Require(spray.isPlaying,
                        "Held synthetic input did not keep the spray playing.");
                    spray.Simulate(
                        0.1f,
                        true,
                        false,
                        true);
                    Require(spray.particleCount > 0,
                        "Held synthetic input produced no live spray particles.");
                    Require(spray.particleCount <= spray.main.maxParticles,
                        "Live spray exceeded its particle cap.");

                    int markCount = Object.FindObjectsByType<SpriteRenderer>(
                            FindObjectsInactive.Include,
                            FindObjectsSortMode.None)
                        .Count(renderer =>
                            renderer.name.StartsWith(
                                "SurfacePaint_Teal_",
                                StringComparison.Ordinal) ||
                            renderer.name.StartsWith(
                                "SurfacePaintCoverage_Teal_",
                                StringComparison.Ordinal));

                    Require(markCount > 0,
                        "Held spray left no teal surface pattern.");

                    QueueMouseState(0);
                    InvokeShooterUpdate(shooter);
                    stageStarted = EditorApplication.timeSinceStartup;
                    SessionState.SetInt(StageKey, 3);
                    return;
                }

                if (stage == 3)
                {
                    if (EditorApplication.timeSinceStartup - stageStarted < 0.12d)
                    {
                        return;
                    }

                    ParticleSystem spray = SprayOf(shooter);
                    Require(!spray.isPlaying && spray.particleCount == 0,
                        "Release did not clear the visual stream.");

                    SessionState.SetBool(EnabledKey, false);
                    Debug.Log(
                        "EXHIBITION_SPRAY_PLAY_OK: synthetic release/hold/release, " +
                        "live bounded stream and persistent teal mark passed. " +
                        "Physical input and feel were excluded at the " +
                        "creator's request.");
                    EditorApplication.Exit(0);
                }
            }
            catch (Exception exception)
            {
                SessionState.SetBool(EnabledKey, false);
                Debug.LogException(exception);
                EditorApplication.Exit(1);
            }
        }

        private static void QueueMouseState(ushort buttons)
        {
            InputSystem.QueueStateEvent(
                syntheticMouse,
                new MouseState
                {
                    position = aimPosition,
                    buttons = buttons
                });
            InputSystem.Update();
        }

        private static void InvokeShooterUpdate(PlayerShooter shooter)
        {
            Require(shooter != null, "Missing PlayerShooter.");

            MethodInfo update = typeof(PlayerShooter).GetMethod(
                "Update",
                BindingFlags.Instance | BindingFlags.NonPublic);

            Require(update != null, "Could not locate PlayerShooter.Update.");
            update.Invoke(shooter, null);
        }

        private static ParticleSystem SprayOf(PlayerShooter shooter)
        {
            SerializedObject serialized = new SerializedObject(shooter);
            ParticleSystem spray = serialized
                .FindProperty("muzzleSpray")
                .objectReferenceValue as ParticleSystem;
            Require(spray != null, "PlayerShooter lost its spray reference.");
            return spray;
        }

        private static void Require(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }
    }
}
