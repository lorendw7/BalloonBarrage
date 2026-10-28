// Copy into Assets/Editor of an isolated project. Run with -batchmode (without
// -quit) -executeMethod BalloonTimingValidation.Run. Uses natural fixed updates.
#if UNITY_EDITOR
using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

public sealed class BalloonTimingCollisionProbe : MonoBehaviour
{
    public Balloon target;
    public bool contacted;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponentInParent<Balloon>() == target) contacted = true;
    }
}

[InitializeOnLoad]
public static class BalloonTimingValidation
{
    private const string ActiveKey = "BalloonTimingValidation.Active";
    private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
    private static int stage;
    private static double started, pausedAt;
    private static RoundController controller;
    private static Balloon balloon;
    private static Rigidbody body;
    private static Vector3 origin;
    private static float pausedElapsed;
    private static Rigidbody incoming;
    private static BalloonTimingCollisionProbe collisionProbe;
    private static RigidbodyConstraints originalConstraints;
    private static readonly string Report = "Logs/balloon-timing-validation.txt";

    static BalloonTimingValidation()
    {
        if (SessionState.GetBool(ActiveKey, false)) EditorApplication.update += Tick;
    }

    public static void Run()
    {
        Require(Application.isBatchMode, "Use a separate batch project.");
        EditorSceneManager.OpenScene("Assets/Scenes/PrototypeScene.unity");
        SessionState.SetBool(ActiveKey, true);
        EditorApplication.EnterPlaymode();
    }

    private static void Tick()
    {
        if (!EditorApplication.isPlaying) return;
        try
        {
            double now = EditorApplication.timeSinceStartup;
            if (stage == 0) { started = now; stage = 1; return; }
            Require(now - started < 40d, "Timing check timed out.");
            if (stage == 1)
            {
                if (now - started < .6) return;
                controller = Object.FindFirstObjectByType<RoundController>();
                var reader = Object.FindFirstObjectByType<RhythmChartReader>();
                Require(reader.IsReady, "Production chart failed to load.");
                reader.enabled = false;
                var spawner = Object.FindFirstObjectByType<BalloonSpawner>();
                Require(spawner.Spawn(new RhythmEventData { id = 10000, timeSeconds = 0 }), "Spawn failed.");
                balloon = Object.FindFirstObjectByType<Balloon>();
                body = balloon.GetComponent<Rigidbody>();
                originalConstraints = (RigidbodyConstraints)typeof(BalloonFloat).GetField("originalConstraints", Private)
                    .GetValue(balloon.GetComponent<BalloonFloat>());
                origin = body.position;
                Require((body.constraints & RigidbodyConstraints.FreezePosition) == RigidbodyConstraints.FreezePosition,
                    "Initial formation position is not protected.");
                Require(Math.Abs(ReadFloat("driftDelaySeconds") - 3f) < .001f, "Wrong production drift delay.");
                Require(Math.Abs(new SerializedObject(balloon).FindProperty("cleanupDelaySeconds").doubleValue - 8d) < .001d,
                    "Wrong production expiry delay.");
                pausedAt = now; stage = 2; return;
            }
            if (stage == 2)
            {
                if (now - pausedAt < .5) return;
                Require(ReadFloat("driftElapsedSeconds") == 0f, "Menu time consumed drift delay.");
                controller.StartSelectedSong();
                Require(RoundController.IsPlaying, "Round did not start.");
                var targetCollider = Array.Find(balloon.GetComponentsInChildren<Collider>(), item => item.enabled);
                Physics.SyncTransforms();
                var incomingObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                incomingObject.transform.localScale = Vector3.one * .25f;
                incomingObject.transform.position = targetCollider.bounds.center - Vector3.right * (targetCollider.bounds.extents.x + .8f);
                incoming = incomingObject.AddComponent<Rigidbody>();
                incoming.useGravity = false;
                incoming.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ |
                    RigidbodyConstraints.FreezeRotation;
                collisionProbe = incomingObject.AddComponent<BalloonTimingCollisionProbe>();
                collisionProbe.target = balloon;
                incoming.linearVelocity = Vector3.right * 2f;
                body.AddForce(Vector3.right * 20f, ForceMode.Impulse);
                stage = 3; return;
            }
            if (stage == 3)
            {
                Require(Vector3.Distance(body.position, origin) < .01f, "Balloon drifted during formation hold.");
                if (ReadFloat("driftElapsedSeconds") < 1f) return;
                Require(collisionProbe.contacted, "Collision probe did not contact the held target.");
                Object.Destroy(incoming.gameObject);
                controller.GetType().GetMethod("TogglePause", Private).Invoke(controller, null);
                Require(RoundController.IsPaused, "Pause failed.");
                pausedElapsed = ReadFloat("driftElapsedSeconds"); pausedAt = now; stage = 4; return;
            }
            if (stage == 4)
            {
                if (now - pausedAt < 3.2) return;
                Require(ReadFloat("driftElapsedSeconds") == pausedElapsed, "Pause consumed drift delay.");
                Require(Vector3.Distance(body.position, origin) < .01f, "Balloon moved while paused.");
                controller.ResumeRound(); stage = 5; return;
            }
            if (stage == 5)
            {
                float elapsed = ReadFloat("driftElapsedSeconds");
                if (elapsed < 2.99f)
                {
                    Require(Vector3.Distance(body.position, origin) < .01f, "Balloon moved before three active seconds.");
                    return;
                }
                if (body.linearVelocity.magnitude < .1f) return;
                Require(elapsed >= 2.99f && elapsed <= 3.03f, "Drift started at the wrong active time.");
                Require(Math.Abs(body.linearVelocity.magnitude - .6f) < .01f, "Drift speed changed.");
                Require(body.constraints == originalConstraints, "Drift did not restore original physics constraints.");
                pausedAt = now; stage = 6; return;
            }
            if (stage == 6)
            {
                if (now - pausedAt < .4) return;
                Require(Vector3.Distance(body.position, origin) > .1f, "Drift did not continue after delay.");
                balloon.CheckExpiry(5.01d);
                Require(balloon.gameObject.activeSelf, "Balloon expired at the old deadline.");
                balloon.CheckExpiry(8d);
                Require(balloon.gameObject.activeSelf, "Balloon expired at the inclusive boundary.");
                balloon.CheckExpiry(8.01d);
                Require(!balloon.gameObject.activeSelf, "Balloon survived the new deadline.");
                Require(Score.Value == 0 && Score.SuccessfulHits == 0, "Expiry awarded a hit or points.");
                Finish(0, "PASS: production prefab; menu hold; three active seconds before drift; position survives impulse and physical collision; pause/resume; restored constraints and original 0.6 speed; eight-second expiry boundary; no expiry score.");
            }
        }
        catch (Exception error) { Finish(1, "FAIL: " + error); }
    }

    private static float ReadFloat(string field) =>
        (float)typeof(BalloonFloat).GetField(field, Private).GetValue(balloon.GetComponent<BalloonFloat>());

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private static void Finish(int code, string message)
    {
        Directory.CreateDirectory("Logs");
        File.WriteAllText(Report, message + Environment.NewLine);
        Debug.Log(message);
        SessionState.SetBool(ActiveKey, false);
        EditorApplication.update -= Tick;
        EditorApplication.Exit(code);
    }
}
#endif
