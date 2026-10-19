using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BalloonBarrage.EditorTools
{
    /// <summary>Run in an isolated batch project; never saves assets or scenes.</summary>
    public static class PaintTintAcceptance
    {
        static void Require(bool value, string message)
        {
            if (!value) throw new InvalidOperationException(message);
        }

        public static void Run()
        {
            if (!Application.isBatchMode) throw new InvalidOperationException("Use an isolated batch project.");
            var a = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var b = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var burst = new GameObject("TintAcceptanceBurst");
            try
            {
                var ra = a.GetComponent<Renderer>();
                var rb = b.GetComponent<Renderer>();
                var material = ra.sharedMaterial;
                var originalColor = material.GetColor("_BaseColor");
                var block = new MaterialPropertyBlock();
                block.SetFloat("_AcceptanceValue", 7); ra.SetPropertyBlock(block, 0);
                block.Clear(); block.SetFloat("_AcceptanceValue", 13); rb.SetPropertyBlock(block);
                StudioPaintTint.Set(ra, Color.red, 0);
                StudioPaintTint.Set(rb, Color.blue);
                ra.GetPropertyBlock(block, 0);
                Require(block.GetFloat("_AcceptanceValue") == 7 && block.GetColor("_BaseColor") == Color.red,
                    "Indexed tint lost unrelated properties or renderer isolation.");
                rb.GetPropertyBlock(block);
                Require(block.GetFloat("_AcceptanceValue") == 13 && block.GetColor("_Color") == Color.blue,
                    "Default tint lost unrelated properties or inherited another renderer's block.");
                Require(material.GetColor("_BaseColor") == originalColor, "Shared material changed.");

                var particle = burst.AddComponent<ParticleSystem>();
                particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                var main = particle.main;
                main.maxParticles = 128; main.startLifetime = 10; main.startSpeed = 0;
                main.startColor = new Color(1, 1, 1, .4f);
                var emission = particle.emission; emission.enabled = false;
                var gradient = new Gradient();
                gradient.SetKeys(new[] { new GradientColorKey(Color.red, 0), new GradientColorKey(Color.blue, 1) },
                    new[] { new GradientAlphaKey(.2f, 0), new GradientAlphaKey(.8f, 1) });
                var lifetime = particle.colorOverLifetime; lifetime.enabled = true; lifetime.color = gradient;
                particle.Play(); particle.Emit(65);
                var live = new ParticleSystem.Particle[128];
                int count = particle.GetParticles(live);
                Require(count == 65, "Particle fixture failed.");
                var alpha = live[0].startColor.a;
                var remaining = live[0].remainingLifetime;
                StudioPaintTint.Effect(burst, Color.green);
                count = particle.GetParticles(live);
                Require(count == 65 && live[0].startColor.g == 255 && live[0].startColor.r == 0 &&
                    live[0].startColor.a == alpha && live[0].remainingLifetime == remaining,
                    "Resized particle buffer changed color, count, alpha or lifetime.");
                Require(Mathf.Abs(particle.main.startColor.color.a - .4f) < .001f &&
                    Mathf.Abs(particle.colorOverLifetime.color.gradient.Evaluate(0).a - .2f) < .001f &&
                    Mathf.Abs(particle.colorOverLifetime.color.gradient.Evaluate(1).a - .8f) < .001f,
                    "Particle alpha gradient changed.");
                particle.Clear(); particle.Emit(3); StudioPaintTint.Effect(burst, Color.magenta);
                Require(particle.GetParticles(live) == 3 && live[0].startColor.b == 255 && live[0].startColor.g == 0,
                    "Reused larger buffer introduced stale particles.");

                string report = "PAINT_TINT_OK: renderer isolation, indexed/default property preservation, shared material, " +
                    "65-to-3 particle buffer reuse, particle alpha/lifetime passed.\n" +
                    "Source reuses the property block, component lists and particle buffer. No GC-byte or standalone FPS claim.\n";
                Debug.Log(report);
                var reportPath = Environment.GetEnvironmentVariable("BALLOON_TINT_REPORT");
                if (!string.IsNullOrEmpty(reportPath)) File.WriteAllText(reportPath, report);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception); EditorApplication.Exit(1); return;
            }
            finally
            {
                Object.DestroyImmediate(a); Object.DestroyImmediate(b); Object.DestroyImmediate(burst);
            }
            EditorApplication.Exit(0);
        }
    }
}
