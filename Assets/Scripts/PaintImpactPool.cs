using System.Collections.Generic;
using UnityEngine;

/// <summary>Short-lived droplets are reused; accumulated surface pigment lives in the atlas.</summary>
public sealed class PaintImpactPool : MonoBehaviour
{
    private const int SlotsPerEffect = 12;
    private static PaintImpactPool current;
    private sealed class Slot { public ParticleSystem root; public ParticleSystem[] parts; public bool active; }
    private sealed class Group { public readonly Slot[] slots = new Slot[SlotsPerEffect]; public int next; }
    private readonly Dictionary<ParticleSystem, Group> groups = new Dictionary<ParticleSystem, Group>(2);
    public static void Warm(ParticleSystem prefab)
    {
        if (prefab == null) return;
        if (current == null) current = new GameObject("PooledPaintImpacts").AddComponent<PaintImpactPool>();
        if (current.groups.ContainsKey(prefab)) return;
        var group = new Group(); current.groups.Add(prefab, group);
        for (int i = 0; i < group.slots.Length; i++)
        {
            ParticleSystem effect = Instantiate(prefab, current.transform);
            ParticleSystem[] parts = effect.GetComponentsInChildren<ParticleSystem>(true);
            foreach (ParticleSystem part in parts) { var main = part.main; main.stopAction = ParticleSystemStopAction.None; }
            effect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); effect.gameObject.SetActive(false);
            group.slots[i] = new Slot { root = effect, parts = parts };
        }
    }
    public static void Play(ParticleSystem prefab, Vector3 point, Vector3 normal, Color pigment)
    {
        if (prefab == null) return;
        Warm(prefab); Group group = current.groups[prefab];
        Slot slot = group.slots[group.next]; group.next = (group.next + 1) % group.slots.Length;
        slot.root.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        slot.root.transform.SetPositionAndRotation(point + normal * .01f, Quaternion.LookRotation(normal));
        foreach (ParticleSystem part in slot.parts) { var main = part.main; main.startColor = pigment; }
        slot.root.gameObject.SetActive(true); slot.active = true; slot.root.Play(true);
    }
    private void Update()
    {
        foreach (Group group in groups.Values)
            foreach (Slot slot in group.slots)
                if (slot.active && (!RoundController.IsPlaying || RoundController.IsPaused || !slot.root.IsAlive(true)))
                { slot.root.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); slot.root.gameObject.SetActive(false); slot.active = false; }
    }
    private void OnApplicationFocus(bool focused)
    {
        if (focused) return;
        foreach (Group group in groups.Values)
            foreach (Slot slot in group.slots)
            { slot.root.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); slot.root.gameObject.SetActive(false); slot.active = false; }
    }
    private void OnDestroy() { if (current == this) current = null; }
}
