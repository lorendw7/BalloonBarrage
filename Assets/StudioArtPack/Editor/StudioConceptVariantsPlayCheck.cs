using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

/// <summary>Isolated automated cosmetic/regression checks; no physical-input acceptance.</summary>
[InitializeOnLoad]
public static class StudioConceptVariantsPlayCheck
{
    const string Enabled = "ConceptVariantsPlay.Enabled", Stage = "ConceptVariantsPlay.Stage";
    const string TargetScene = "ConceptVariantsPlay.Scene", TargetRoot = "ConceptVariantsPlay.Root";
    static string Report => SessionState.GetString(TargetRoot, StudioConceptVariantsPass.Root) + "/PlayCheck.txt";
    static double deadline, started;
    static int targetId;
    static Color shotColor;
    static Mouse mouse;
    static Vector2 aim;
    static StudioConceptVariantsPlayCheck() { if (SessionState.GetBool(Enabled, false)) EditorApplication.update += Tick; }
    static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
    static void Log(string message) => File.AppendAllText(Report, message + "\n");
    static bool RGB(Color a, Color b) => Mathf.Abs(a.r - b.r) < .001f && Mathf.Abs(a.g - b.g) < .001f && Mathf.Abs(a.b - b.b) < .001f;

    public static void RunBatch()
    {
        StudioConceptVariantsPass.RegisterScene();
        Run(StudioConceptVariantsPass.Destination, StudioConceptVariantsPass.Root);
    }

    public static void RunDetailBatch()
    {
        StudioConceptDetailPass.RegisterScene();
        Run(StudioConceptDetailPass.Destination, StudioConceptDetailPass.Root);
    }

    static void Run(string scene, string root)
    {
        Require(Application.isBatchMode, "Run in an isolated batch project without -quit.");
        SessionState.SetString(TargetScene, scene); SessionState.SetString(TargetRoot, root);
        EditorSceneManager.OpenScene(scene);
        File.WriteAllText(Report, "Unity " + Application.unityVersion + "; isolated Play Mode, synthetic input/method-driven checks.\nNo human playtest, natural full round, frame-time acceptance or release.\n");
        SessionState.SetBool(Enabled, true); SessionState.SetInt(Stage, 0);
        EditorApplication.update -= Tick; EditorApplication.update += Tick;
        deadline = EditorApplication.timeSinceStartup + 90;
        EditorApplication.EnterPlaymode();
    }

    static void CheckAppearances()
    {
        var spawner = new SerializedObject(Object.FindFirstObjectByType<BalloonSpawner>());
        var prefab = (GameObject)spawner.FindProperty("balloonPrefab").objectReferenceValue;
        var pigmentSeen = new HashSet<int>(); var shapeSeen = new HashSet<int>(); var randomBefore = UnityEngine.Random.state;
        for (int i = 0; i < 48; i++)
        {
            var target = Object.Instantiate(prefab, new Vector3(0, 0, 8), Quaternion.identity);
            var art = target.GetComponent<StudioBalloonAppearance>(); pigmentSeen.Add(art.ColorIndex); shapeSeen.Add(art.ShapeIndex);
            var collider = target.GetComponentsInChildren<Collider>(true).Select(EditorJsonUtility.ToJson).ToArray();
            var rigidbody = EditorJsonUtility.ToJson(target.GetComponent<Rigidbody>()); int points = target.GetComponent<Balloon>().points;
            for (int c = 0; c < 6; c++) for (int s = 0; s < 3; s++)
            {
                art.ApplyVariant(c, s);
                Require(art.body.sharedMesh == art.silhouettes[s] && art.latex.sharedMaterial == art.pigments[c], "Balloon variant did not apply.");
                Require(collider.SequenceEqual(target.GetComponentsInChildren<Collider>(true).Select(EditorJsonUtility.ToJson)) && rigidbody == EditorJsonUtility.ToJson(target.GetComponent<Rigidbody>()) && points == target.GetComponent<Balloon>().points, "Appearance changed balloon physics/points.");
            }
            Object.DestroyImmediate(target);
        }
        Require(UnityEngine.Random.state.Equals(randomBefore), "Appearance consumed gameplay random state.");
        Require(pigmentSeen.Count > 1 && shapeSeen.Count > 1, "Balloon initialization never varied.");
        Log("48 balloon initializations varied (" + pigmentSeen.Count + " colors, " + shapeSeen.Count + " silhouettes). All 18 combinations preserve colliders, Rigidbody and points; gameplay Random state unchanged.");
        var gun = Object.FindFirstObjectByType<StudioGunAppearance>();
        var originalMaterials = gun.regions.Select(r => EditorJsonUtility.ToJson(r.renderer.sharedMaterials[r.materialIndex])).ToArray();
        var seen = new HashSet<int>();
        for (int i = 0; i < 48; i++)
        {
            var temporary = new GameObject("GunInitializationCheck"); temporary.SetActive(false);
            var art = temporary.AddComponent<StudioGunAppearance>(); art.themes = gun.themes; temporary.SetActive(true); seen.Add(art.ThemeIndex); Object.DestroyImmediate(temporary);
        }
        Require(seen.Count > 1, "Gun initialization never varied.");
        for (int i = 0; i < 6; i++)
        {
            gun.ApplyTheme(i);
            foreach (var region in gun.regions)
            {
                var block = new MaterialPropertyBlock(); region.renderer.GetPropertyBlock(block, region.materialIndex);
                var theme = gun.themes[i]; var expected = region.role == 0 ? theme.body : region.role == 1 ? theme.paint : region.role == 2 ? theme.muzzle : theme.hardware;
                Require(RGB(block.GetColor("_BaseColor"), expected), "Gun material region tint mismatch.");
            }
            Require(RGB(gun.muzzleSpray.main.startColor.Evaluate(.5f), gun.PaintColor), "Muzzle pigment mismatch.");
        }
        Require(originalMaterials.SequenceEqual(gun.regions.Select(r => EditorJsonUtility.ToJson(r.renderer.sharedMaterials[r.materialIndex]))), "Theme mutated shared material assets.");
        Require(UnityEngine.Random.state.Equals(randomBefore), "Gun appearance consumed gameplay random state.");
        gun.ApplyTheme(3); shotColor = gun.PaintColor;
        Log("48 gun initializations varied (" + seen.Count + " themes). All 6 region palettes/muzzle pigments passed; shared material assets and gameplay Random state unchanged.");
        var emitted = new GameObject("AlreadyEmittedBurstCheck"); var ps = emitted.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var main = ps.main; main.startColor = new Color(.1f, .2f, .3f, .4f); main.startLifetime = 2f; ps.Emit(4);
        var particlesBefore = new ParticleSystem.Particle[4]; ps.GetParticles(particlesBefore);
        StudioPaintTint.Effect(emitted, gun.PaintColor);
        var particlesAfter = new ParticleSystem.Particle[4]; int liveCount = ps.GetParticles(particlesAfter);
        Require(liveCount == 4, "Tint changed existing particle count.");
        for (int i = 0; i < liveCount; i++)
        {
            Color actual = particlesAfter[i].startColor;
            Require(Mathf.Abs(actual.r - gun.PaintColor.r) < .005f && Mathf.Abs(actual.g - gun.PaintColor.g) < .005f && Mathf.Abs(actual.b - gun.PaintColor.b) < .005f, "Awake-emitted particles kept the old pigment.");
            Require(particlesAfter[i].startColor.a == particlesBefore[i].startColor.a && particlesAfter[i].remainingLifetime == particlesBefore[i].remainingLifetime, "Tint changed existing particle alpha/lifetime.");
        }
        Object.DestroyImmediate(emitted);
        Log("Already-emitted burst particles recolored without changing count, alpha or remaining lifetime.");
    }

    static void Tick()
    {
        if (!SessionState.GetBool(Enabled, false)) return;
        try
        {
            if (!EditorApplication.isPlaying) return;
            int stage = SessionState.GetInt(Stage, 0);
            if (stage == 0)
            {
                if (Time.frameCount < 8) return;
                deadline = EditorApplication.timeSinceStartup + 45;
                Require(!RoundController.IsPlaying && Score.Value == 0 && Object.FindObjectsByType<Balloon>(FindObjectsSortMode.None).Length == 0, "Start gate changed.");
                CheckAppearances();
                InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
                foreach (var device in InputSystem.devices.OfType<Mouse>().ToArray()) InputSystem.DisableDevice(device);
                mouse = InputSystem.AddDevice<Mouse>("ConceptVariantsSyntheticMouse"); mouse.MakeCurrent();
                Object.FindFirstObjectByType<RoundController>().BeginRound(); SessionState.SetInt(Stage, 1); return;
            }
            Require(EditorApplication.timeSinceStartup < deadline, "Play check timed out at stage " + stage);
            var balloons = Object.FindObjectsByType<Balloon>(FindObjectsSortMode.None);
            Require(balloons.Length <= 3, "Three-target budget exceeded.");
            if (stage == 1)
            {
                if (balloons.Length != 3) return;
                StudioConceptVariantsPass.Capture(SceneManager.GetActiveScene(), "Runtime_Variants");
                string root = SessionState.GetString(TargetRoot, StudioConceptVariantsPass.Root);
                if (root != StudioConceptVariantsPass.Root) File.Copy(StudioConceptVariantsPass.Root + "/Previews/Runtime_Variants.png", root + "/Previews/Runtime_Variants.png", true);
                var target = balloons.OrderBy(b => Mathf.Abs(b.transform.position.x)).First(); targetId = target.GetInstanceID();
                var body = target.GetComponentsInChildren<Collider>().First(c => c.enabled && !c.isTrigger);
                var screen = Camera.main.WorldToScreenPoint(body.bounds.center); aim = new Vector2(screen.x, screen.y);
                InputSystem.QueueStateEvent(mouse, new MouseState { position = aim, buttons = 0 }); InputSystem.Update();
                typeof(PlayerShooter).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(Object.FindFirstObjectByType<PlayerShooter>(), null);
                started = EditorApplication.timeSinceStartup; SessionState.SetInt(Stage, 2); return;
            }
            if (stage == 2)
            {
                if (EditorApplication.timeSinceStartup - started < .2) return;
                var target = balloons.First(b => b.GetInstanceID() == targetId); var body = target.GetComponentsInChildren<Collider>().First(c => c.enabled && !c.isTrigger);
                var screen = Camera.main.WorldToScreenPoint(body.bounds.center); aim = new Vector2(screen.x, screen.y);
                InputSystem.QueueStateEvent(mouse, new MouseState { position = aim, buttons = 1 }); InputSystem.Update();
                var shooter = Object.FindFirstObjectByType<PlayerShooter>(); typeof(PlayerShooter).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(shooter, null);
                var projectile = Object.FindObjectsByType<PaintProjectile>(FindObjectsSortMode.None).FirstOrDefault();
                Require(projectile != null, "Synthetic method-driven shooter did not create a projectile.");
                foreach (var r in projectile.GetComponentsInChildren<Renderer>())
                { var block = new MaterialPropertyBlock(); r.GetPropertyBlock(block); Require(RGB(block.GetColor("_BaseColor"), shotColor), "Flying paint pigment mismatch."); }
                Require(RGB(shooter.GetComponent<StudioGunAppearance>().PaintColor, shotColor), "Gun recolored while firing.");
                InputSystem.QueueStateEvent(mouse, new MouseState { position = aim, buttons = 0 }); InputSystem.Update();
                Log("Accepted method-driven synthetic shot created the existing projectile; every projectile renderer and muzzle spray matched the selected gun pigment. Gun color remained stable.");
                started = EditorApplication.timeSinceStartup; SessionState.SetInt(Stage, 3); return;
            }
            if (stage == 3)
            {
                if (EditorApplication.timeSinceStartup - started < 1) return;
                Require(Score.Value == 10 && balloons.All(b => b.GetInstanceID() != targetId), "Shot did not hit/pop/score once.");
                Log("Existing swept projectile hit the aimed target; exactly +10 points. Natural respawn pending.");
                SessionState.SetInt(Stage, 4); return;
            }
            if (stage == 4)
            {
                if (balloons.Length != 3) return;
                // Verify the new color hook through the actual Balloon.Pop effect instance.
                var target = balloons[0]; target.GetComponent<StudioBalloonAppearance>().ApplyVariant(5, 2); Color pigment = target.GetComponent<StudioBalloonAppearance>().Pigment;
                var oldEffects = Object.FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None).Select(p => p.GetInstanceID()).ToHashSet();
                target.Hit(); target.Hit();
                var effects = Object.FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None).Where(p => !oldEffects.Contains(p.GetInstanceID())).ToArray();
                Require(effects.Length > 0 && Score.Value == 20, "Pop effect missing or duplicate score.");
                foreach (var particle in effects) Require(RGB(particle.main.startColor.Evaluate(.5f), pigment), "Pop color hook did not match balloon.");
                Log("Three-target refill passed. Actual duplicate Hit calls scored once; spawned fragment/paint particle pigments matched the rose balloon.");
                Object.FindFirstObjectByType<RoundController>().RestartRound(); started = EditorApplication.timeSinceStartup; SessionState.SetInt(Stage, 5); return;
            }
            if (stage == 5)
            {
                if (EditorApplication.timeSinceStartup - started < .6) return;
                Require(!RoundController.IsPlaying && Score.Value == 0 && balloons.Length == 0, "Retry did not restore waiting state.");
                Require(Object.FindFirstObjectByType<StudioGunAppearance>() != null, "Retry loaded the wrong scene.");
                Require(SceneManager.GetActiveScene().path == SessionState.GetString(TargetScene, StudioConceptVariantsPass.Destination), "Retry loaded a different art scene.");
                Log("Saved build-index-based retry reloaded " + SceneManager.GetActiveScene().name + ": Start gate, zero score and no targets restored; gun initialized again.");
                SessionState.SetBool(Enabled, false); Debug.Log("CONCEPT_VARIANTS_PLAY_OK"); EditorApplication.Exit(0);
            }
        }
        catch (Exception ex) { SessionState.SetBool(Enabled, false); Log("FAILED: " + ex); Debug.LogException(ex); EditorApplication.Exit(1); }
    }
}
