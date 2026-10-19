using System.Collections.Generic;
using UnityEngine;

/// <summary>Cosmetic tinting only; retains shared assets and particle timing/alpha.</summary>
public static class StudioPaintTint
{
    static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
    static readonly int ColorId = Shader.PropertyToID("_Color");
    // Unity copies property blocks on SetPropertyBlock. These main-thread buffers
    // avoid allocating wrappers/component arrays for each projectile and burst.
    static readonly MaterialPropertyBlock TintBlock = new MaterialPropertyBlock();
    static readonly List<Renderer> Renderers = new List<Renderer>(8);
    static readonly List<ParticleSystem> Particles = new List<ParticleSystem>(4);
    static ParticleSystem.Particle[] liveParticles = new ParticleSystem.Particle[32];

    public static void Set(Renderer renderer, Color color, int materialIndex = -1)
    {
        var block = TintBlock;
        block.Clear();
        if (materialIndex < 0) renderer.GetPropertyBlock(block);
        else renderer.GetPropertyBlock(block, materialIndex);
        block.SetColor(BaseColor, color);
        block.SetColor(ColorId, color);
        if (materialIndex < 0) renderer.SetPropertyBlock(block);
        else renderer.SetPropertyBlock(block, materialIndex);
    }

    static Gradient Recolor(Gradient source, Color color)
    {
        var result = new Gradient { mode = source.mode };
        result.SetKeys(new[] { new GradientColorKey(color, 0), new GradientColorKey(color, 1) }, source.alphaKeys);
        return result;
    }

    static ParticleSystem.MinMaxGradient Recolor(ParticleSystem.MinMaxGradient source, Color color)
    {
        switch (source.mode)
        {
            case ParticleSystemGradientMode.Color:
                color.a = source.color.a;
                return new ParticleSystem.MinMaxGradient(color);
            case ParticleSystemGradientMode.TwoColors:
                var low = color; low.a = source.colorMin.a;
                var high = color; high.a = source.colorMax.a;
                return new ParticleSystem.MinMaxGradient(low, high);
            case ParticleSystemGradientMode.TwoGradients:
                return new ParticleSystem.MinMaxGradient(Recolor(source.gradientMin, color), Recolor(source.gradientMax, color));
            default:
                var gradient = new ParticleSystem.MinMaxGradient(Recolor(source.gradient, color));
                gradient.mode = source.mode;
                return gradient;
        }
    }

    public static void Effect(GameObject root, Color color)
    {
        root.GetComponentsInChildren(true, Renderers);
        foreach (var renderer in Renderers)
            if (!(renderer is ParticleSystemRenderer)) Set(renderer, color);
        Renderers.Clear();
        root.GetComponentsInChildren(true, Particles);
        foreach (var particle in Particles)
        {
            var main = particle.main;
            main.startColor = Recolor(main.startColor, color);
            var lifetime = particle.colorOverLifetime;
            if (lifetime.enabled) lifetime.color = Recolor(lifetime.color, Color.white);
            // Burst prefabs can emit on Awake, before their owner supplies the pigment.
            // Tint any already-emitted particles as well without restarting the effect.
            if (particle.particleCount > 0)
            {
                if (liveParticles.Length < particle.particleCount)
                    System.Array.Resize(ref liveParticles, Mathf.NextPowerOfTwo(particle.particleCount));
                var live = liveParticles;
                int count = particle.GetParticles(live);
                for (int i = 0; i < count; i++)
                {
                    var pigment = color; pigment.a = live[i].startColor.a / 255f;
                    live[i].startColor = pigment;
                }
                particle.SetParticles(live, count);
            }
        }
        Particles.Clear();
    }
}
