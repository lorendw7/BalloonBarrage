// Run only in an isolated project, copied to Assets/Editor, without -quit.
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

[InitializeOnLoad]
public static class RoomPaintValidation
{
    private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
    private const string Key = "RoomPaintValidation.Active";
    private static string Folder => Path.GetFullPath("../../Logs/RoomPaint20261031");
    private static int stage, checks, retries, oldRound;
    private static double started, deadline;
    private static double readyAt;
    private static RoomPaintSystem system;
    private static RoundController round;
    private static PaintProjectile projectile;
    private static PaintSurface[] surfaces;
    private static List<Contact> contacts;
    private static Report report;
    private sealed class Contact { public PaintSurface surface; public RaycastHit hit; public Vector2 uv; }
    [Serializable] private sealed class Report
    {
        public string status, scope = "Controlled Unity Editor GPU/physics/source checks and natural song clock. Not physical input, venue sound or standalone frame acceptance.";
        public int assertions, surfaces, atlasWidth, persistentPaintRenderers, stressContacts, retries, pooledImpactEmissions;
        public long atlasColorBytes;
        public bool allReceiversPainted, stableObjectCount, idleHasNoPaintPasses, pausePersists, resultsPersist, naturalSong, oppositeFaceRejected, strokeInterpolation;
        public double naturalSongSeconds;
        public string[] errors;
    }
    static RoomPaintValidation() { if (SessionState.GetBool(Key, false)) EditorApplication.update += Tick; }
    public static void Run()
    {
        if (!Application.isBatchMode) throw new InvalidOperationException("Use an isolated batch project.");
        Directory.CreateDirectory(Folder); EditorSceneManager.OpenScene("Assets/Scenes/PrototypeScene.unity");
        SessionState.SetBool(Key, true); SessionState.SetInt(Key + ".stage", 0);
        EditorApplication.EnterPlaymode();
    }
    private static void Tick()
    {
        if (!EditorApplication.isPlaying) return;
        try
        {
            if (stage == 0)
            {
                if (!Application.isPlaying) return;
                if (readyAt == 0d) { readyAt = EditorApplication.timeSinceStartup; return; }
                if (EditorApplication.timeSinceStartup - readyAt < 1d) return;
                Controlled(); stage = 1; started = EditorApplication.timeSinceStartup; return;
            }
            if (stage == 1)
            {
                if (EditorApplication.timeSinceStartup - started < .3d) return;
                int flushes = system.FlushCount;
                report.idleHasNoPaintPasses = flushes == lastFlush;
                Check(report.idleHasNoPaintPasses, "Idle frames perform no paint pass.");
                Capture("full-room-controlled.png");
                foreach (Canvas canvas in Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None)) canvas.enabled = true;
                round.enabled = true;
                var spawner = Object.FindFirstObjectByType<BalloonSpawner>(); spawner.enabled = true;
                round.BeginRound(); round.StartSelectedSong();
                Check(RoundController.IsPlaying, "Natural short song starts through the actual selection API.");
                Check(system.SubmittedStampCount == 0, "Starting a new round clears the previous atlas and stroke history.");
                PaintViewport(10, 5); system.Flush();
                PaintImpactPool.Play(ImpactPrefab(),contacts[0].hit.point,contacts[0].hit.normal,Color.cyan);
                Check(Object.FindFirstObjectByType<PaintImpactPool>().GetComponentsInChildren<ParticleSystem>(true).Any(p=>p.isPlaying),
                    "A gameplay impact uses a warmed pool slot.");
                round.TogglePauseFromController();
                Check(RoundController.IsPaused, "Actual controller pause suspends the round.");
                pausedCount = system.SubmittedStampCount; started = EditorApplication.timeSinceStartup; stage = 2; return;
            }
            if (stage == 2)
            {
                if (EditorApplication.timeSinceStartup - started < .3d) return;
                Check(system.SubmittedStampCount == pausedCount && system.Atlas.IsCreated(), "Pause preserves accumulated paint and its GPU resource.");
                Check(!Object.FindFirstObjectByType<PaintImpactPool>().GetComponentsInChildren<ParticleSystem>(true).Any(p=>p.isPlaying || p.particleCount>0),
                    "Pause clears all transient pooled particles without clearing paint.");
                report.pausePersists = true; round.ResumeRound();
                deadline = EditorApplication.timeSinceStartup + 100d; stage = 3; return;
            }
            if (stage == 3)
            {
                if (RoundController.IsPlaying)
                { report.naturalSongSeconds=Math.Max(report.naturalSongSeconds,round.SongTimeSeconds);
                    if (EditorApplication.timeSinceStartup > deadline) throw new Exception("Natural short song timed out."); return; }
                report.naturalSong = true;
                Check(system.SubmittedStampCount >= pausedCount && system.Atlas.IsCreated(), "Natural results preserve the painted canvas.");
                report.resultsPersist = true; oldRound = round.GetInstanceID(); round.RestartRound(); stage = 4; return;
            }
            if (stage == 4)
            {
                RoundController next = Object.FindFirstObjectByType<RoundController>();
                if (next == null || next.GetInstanceID() == oldRound || Time.timeSinceLevelLoad < .3f) return;
                round = next; system = Object.FindFirstObjectByType<RoomPaintSystem>();
                Check(system != null && system.SubmittedStampCount == 0 && system.PendingCount == 0, "Retry creates a clean atlas, not retained marks.");
                Check(Object.FindObjectsByType<RoomPaintSystem>(FindObjectsSortMode.None).Length == 1, "Retry retains exactly one paint owner.");
                retries++; report.retries = retries;
                if (retries < 5) { oldRound = round.GetInstanceID(); round.RestartRound(); return; }
                Finish(null);
            }
        }
        catch (Exception error) { Finish(error); }
    }
    private static int lastFlush;
    private static long pausedCount;
    private static void Controlled()
    {
        report = new Report { status = "running" };
        system = Object.FindFirstObjectByType<RoomPaintSystem>(); round = Object.FindFirstObjectByType<RoundController>();
        Debug.Log("ROOM_PAINT_DIAGNOSTIC: " + string.Join(" | ", Object.FindObjectsByType<RoomPaintSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .Select(s => "owner=" + s.GetInstanceID() + " name=" + s.name + " active=" + s.gameObject.activeInHierarchy + " atlas=" +
                (s.Atlas == null ? "null" : s.Atlas.GetInstanceID() + " created=" + s.Atlas.IsCreated()))) +
            "; global=" + Shader.GetGlobalTexture("_RoomPaintAtlas"));
        Check(system != null && system.Atlas != null, "Prepared GPU atlas descriptor initializes: owner=" +
            (system != null) + " enabled=" + (system != null && system.enabled) + " frame=" + Time.frameCount);
        surfaces = (PaintSurface[])typeof(RoomPaintSystem).GetField("surfaces", Private).GetValue(system);
        report.surfaces = surfaces.Length; report.atlasWidth = system.Atlas.width; report.atlasColorBytes = system.ColorStorageBytes;
        Check(surfaces.Length > 9 && system.Atlas.width <= 4096, "Full opaque static room uses a bounded atlas.");
        Check(surfaces.Select(s => s.CoatRenderer).Distinct().Count() == 1, "All receivers share one combined paint renderer.");
        report.persistentPaintRenderers = 1;
        round.enabled = false; Object.FindFirstObjectByType<BalloonSpawner>().enabled = false;
        foreach (Canvas canvas in Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None)) canvas.enabled = false;
        var shooter = Object.FindFirstObjectByType<PlayerShooter>();
        projectile = (PaintProjectile)typeof(PlayerShooter).GetField("paintProjectilePrefab", Private).GetValue(shooter);
        int score = Score.Value;
        contacts = new List<Contact>(surfaces.Length);
        foreach (PaintSurface surface in surfaces)
        {
            Mesh source=surface.GetComponents<MeshCollider>().First(c=>c.enabled && !c.isTrigger).sharedMesh;
            Vector3[] original=source.vertices; int[] sourceIndices=source.triangles;
            Vector3[] local=sourceIndices.Select(i=>original[i]).ToArray();
            Matrix4x4 matrix = surface.TargetRenderer.transform.localToWorldMatrix;
            var vertices = local.Select(v => matrix.MultiplyPoint3x4(v)).ToArray();
            int count = local.Length;
            int best = 0; float bestArea = -1f;
            for (int i = 0; i < count; i += 3)
            {
                float area = Vector3.Cross(vertices[i+1]-vertices[i], vertices[i+2]-vertices[i]).sqrMagnitude;
                if (area > bestArea) { best = i; bestArea = area; }
            }
            Vector3 center = (vertices[best]+vertices[best+1]+vertices[best+2])/3f;
            Vector3 cross = Vector3.Cross(vertices[best+1]-vertices[best], vertices[best+2]-vertices[best]);
            Vector3 normal = cross / Mathf.Max(cross.magnitude, 1e-15f);
            Collider collider = surface.GetComponents<Collider>().FirstOrDefault(c => c.enabled && !c.isTrigger);
            Check(collider != null, "Prepared receiver has a live contact collider: " + surface.name);
            RaycastHit hit;
            float reach = collider.bounds.extents.magnitude + Vector3.Distance(collider.bounds.center, center) + .1f;
            bool found = collider.Raycast(new Ray(center + normal*reach, -normal), out hit, reach*2f+.1f);
            if (!found) found = collider.Raycast(new Ray(center - normal*reach, normal), out hit, reach*2f+.1f);
            if (!found) Debug.LogError("CONTACT_DIAGNOSTIC: " + surface.name + " center=" + center + " normal=" + normal +
                " collider=" + collider.GetType().Name + " bounds=" + collider.bounds + " renderer=" + surface.TargetRenderer.bounds +
                " transform=" + surface.transform.localToWorldMatrix + " count=" + count +
                " mesh=" + (collider is MeshCollider meshContact ? meshContact.sharedMesh.name + " " + meshContact.sharedMesh.bounds : "primitive"));
            Check(found, "Actual collider receives a mesh contact: " + surface.name);
            contacts.Add(new Contact { surface=surface,hit=hit,uv=ContactUV(surface,hit) });
        }
        Check(surfaces[0].CoatRenderer.GetComponent<MeshFilter>().sharedMesh.vertexCount <= surfaces.Sum(s=>s.StampMesh.vertexCount),
            "Combined visible geometry welds duplicates without growing the source vertex budget.");
        int objects = Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length;
        for (int i = 0; i < contacts.Count; i++) projectile.SpawnPaintMark(contacts[i].hit, i%8);
        system.Flush();
        Check(system.Atlas.IsCreated(), "Actual paint submissions allocate live GPU storage.");
        Texture2D snapshot = ReadAtlas();
        foreach (Contact contact in contacts)
        {
            if (AlphaNear(snapshot, contact.uv) <= .3f)
            {
                Rect tile=contact.surface.AtlasPixels; var crop=new Texture2D((int)tile.width,(int)tile.height,TextureFormat.RGBA32,false,true);
                crop.SetPixels(snapshot.GetPixels((int)tile.x,(int)tile.y,(int)tile.width,(int)tile.height)); crop.Apply();
                File.WriteAllBytes(Path.Combine(Folder,"failed-tile.png"),crop.EncodeToPNG()); Object.DestroyImmediate(crop);
                Debug.LogError("GPU_CONTACT_SAMPLE: "+contact.surface.name+" uv="+contact.uv.ToString("F8")+" point="+contact.hit.point.ToString("F8")+
                    " normal="+contact.hit.normal+" triangle="+contact.hit.triangleIndex+" tile="+tile+" barycentric="+contact.hit.barycentricCoordinate);
            }
            Check(AlphaNear(snapshot, contact.uv) > .3f, "GPU paint reaches its surface UV without mirroring: " + contact.surface.name);
        }
        Object.DestroyImmediate(snapshot); report.allReceiversPainted = true;
        Check(Score.Value == score, "Painting the environment grants no hits, charges or score.");
        Contact sample = contacts.First(c => c.surface.name == "MainWall");
        for (int color = 0; color < 8; color++)
        {
            system.Clear(); projectile.SpawnPaintMark(sample.hit, color); system.Flush();
            snapshot = ReadAtlas(); Color actual = PixelNear(snapshot, sample.uv); Color expected = projectile.GetPigmentColor(color);
            if (QualitySettings.activeColorSpace == ColorSpace.Linear) expected = expected.linear;
            Check(actual.a > .7f && Vector3.Distance(new Vector3(actual.r,actual.g,actual.b), new Vector3(expected.r,expected.g,expected.b)) < .12f,
                "Accumulated GPU pigment matches the selected palette entry: " + color + " actual=" + actual + " expected=" + expected);
            Object.DestroyImmediate(snapshot);
        }
        CheckBackAndStroke(sample);
        for (int i=0;i<1000;i++) PaintImpactPool.Play(ImpactPrefab(),sample.hit.point,sample.hit.normal,projectile.GetPigmentColor(i%8));
        Check(objects == Object.FindObjectsByType<Transform>(FindObjectsInactive.Include,FindObjectsSortMode.None).Length,
            "One thousand transient impacts reuse warmed objects.");
        report.pooledImpactEmissions=1000;
        system.Clear();
        for (int i = 0; i < 10000; i++) projectile.SpawnPaintMark(contacts[i%contacts.Count].hit, i%8);
        system.Flush(); report.stressContacts = 10000;
        report.stableObjectCount = objects == Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length;
        Check(report.stableObjectCount, "Ten thousand paint contacts do not create permanent GameObjects.");
        Check(system.PendingCount == 0 && system.SubmittedStampCount >= 10000, "Batches flush without discarding older pigment.");
        Check(system.ColorStorageBytes == report.atlasColorBytes, "Dense paint does not grow atlas memory.");
        Check(Score.Value == score, "Dense paint stress still leaves score unchanged.");
        PaintViewport(32, 16); system.Flush(); lastFlush = system.FlushCount;
    }
    private static Vector2 ContactUV(PaintSurface surface, RaycastHit hit)
    {
        Mesh source=((MeshCollider)hit.collider).sharedMesh;
        Vector3[] original=source.vertices, padded=surface.StampMesh.vertices;
        int[] indices=source.triangles; Vector2[] uv=surface.StampMesh.uv4; int t=hit.triangleIndex*3;
        float expansion=(padded[t+1]-padded[t]).magnitude/(original[indices[t+1]]-original[indices[t]]).magnitude;
        Vector2 center=(uv[t]+uv[t+1]+uv[t+2])/3f;
        Vector3 b=hit.barycentricCoordinate;
        return center+(uv[t]*b.x+uv[t+1]*b.y+uv[t+2]*b.z-center)/expansion;
    }
    private static ParticleSystem ImpactPrefab() => (ParticleSystem)typeof(PlayerShooter).GetField("impactSprayPrefab",Private).GetValue(Object.FindFirstObjectByType<PlayerShooter>());
    private static void CheckBackAndStroke(Contact sample)
    {
        Collider collider=sample.hit.collider; Vector3 n=sample.hit.normal; float reach=collider.bounds.size.magnitude+1f;
        Check(collider.Raycast(new Ray(sample.hit.point-n*reach,n),out RaycastHit back,reach*2f),"Opposite wall face has a real contact.");
        Check(Vector3.Dot(back.normal,n)<-.9f,"Opposite contact faces away from the brush.");
        system.Clear(); projectile.SpawnPaintMark(sample.hit,0); system.Flush();
        Texture2D image=ReadAtlas();
        Check(AlphaNear(image,ContactUV(sample.surface,back))<.05f,"A front contact never paints through the opposite face.");
        Object.DestroyImmediate(image); report.oppositeFaceRejected=true;
        Vector3 tangent=Mathf.Abs(n.y)>.9f?Vector3.right:Vector3.Cross(Vector3.up,n).normalized;
        Check(collider.Raycast(new Ray(sample.hit.point+n*2f+tangent*.7f,-n),out RaycastHit second,4f),"Continuous stroke has a second real contact.");
        system.Clear(); projectile.SpawnPaintMark(sample.hit,0); projectile.SpawnPaintMark(second,0); long discrete=system.SubmittedStampCount;
        system.Clear(); projectile.SpawnPaintMark(sample.hit,0,77); projectile.SpawnPaintMark(second,0,77);
        Check(system.SubmittedStampCount>discrete,"A rapid same-player stroke fills its intermediate pigment samples.");
        system.Clear(); projectile.SpawnPaintMark(sample.hit,0,77); projectile.SpawnPaintMark(second,0,88);
        Check(system.SubmittedStampCount==discrete,"Different players never connect their pigment strokes.");
        system.Clear(); projectile.SpawnPaintMark(sample.hit,0,77); RoomPaintSystem.EndStroke(77); projectile.SpawnPaintMark(second,0,77);
        Check(system.SubmittedStampCount==discrete,"Release breaks a continuous stroke.");
        report.strokeInterpolation=true;
    }
    private static void PaintViewport(int columns, int rows)
    {
        Camera camera = Camera.main;
        for (int y = 0; y < rows; y++) for (int x = 0; x < columns; x++)
        {
            Ray ray = camera.ViewportPointToRay(new Vector3(.08f+.84f*x/Mathf.Max(1,columns-1), .08f+.84f*y/Mathf.Max(1,rows-1), 0));
            if (Physics.Raycast(ray, out RaycastHit hit, 40f) && PaintSurface.CanReceivePaint(hit.collider))
                projectile.SpawnPaintMark(hit, (x/5+y/4)%8);
        }
    }
    private static Texture2D ReadAtlas()
    {
        var texture = new Texture2D(system.Atlas.width, system.Atlas.height, TextureFormat.RGBA32, false, true);
        RenderTexture previous = RenderTexture.active; RenderTexture.active = system.Atlas;
        texture.ReadPixels(new Rect(0,0,texture.width,texture.height),0,0); texture.Apply(); RenderTexture.active = previous; return texture;
    }
    private static Color PixelNear(Texture2D image, Vector2 uv)
    {
        Color best = Color.clear; int x = Mathf.RoundToInt(uv.x*(image.width-1)), y = Mathf.RoundToInt(uv.y*(image.height-1));
        for (int dy=-1;dy<=1;dy++) for (int dx=-1;dx<=1;dx++)
        { Color c=image.GetPixel(Mathf.Clamp(x+dx,0,image.width-1),Mathf.Clamp(y+dy,0,image.height-1)); if (c.a>best.a) best=c; }
        return best;
    }
    private static float AlphaNear(Texture2D image, Vector2 uv) => PixelNear(image,uv).a;
    private static void Capture(string name)
    {
        Camera camera=Camera.main; RenderTexture target=RenderTexture.GetTemporary(2560,1440,24);
        RenderTexture previous=RenderTexture.active; RenderTexture priorTarget=camera.targetTexture;
        try
        {
            camera.targetTexture=target; camera.aspect=16f/9f; camera.Render(); RenderTexture.active=target;
            var image=new Texture2D(2560,1440,TextureFormat.RGB24,false); image.ReadPixels(new Rect(0,0,2560,1440),0,0); image.Apply();
            File.WriteAllBytes(Path.Combine(Folder,name),image.EncodeToPNG()); Object.DestroyImmediate(image);
        }
        finally { camera.targetTexture=priorTarget; RenderTexture.active=previous; RenderTexture.ReleaseTemporary(target); }
    }
    private static void Check(bool result, string message) { checks++; if (!result) throw new InvalidOperationException(message); }
    private static void Finish(Exception error)
    {
        EditorApplication.update -= Tick; SessionState.SetBool(Key,false);
        if (report==null) report=new Report(); report.assertions=checks; report.status=error==null?"passed":"failed";
        report.errors=error==null?new string[0]:new[]{error.ToString()};
        File.WriteAllText(Path.Combine(Folder,"gpu-validation.json"),JsonUtility.ToJson(report,true));
        if (error!=null) Debug.LogException(error); else Debug.Log("ROOM_PAINT_GPU_OK: "+checks+" assertions; "+report.surfaces+" receivers; 10,000 contacts; natural song and five Retry resets.");
        EditorApplication.Exit(error==null?0:1);
    }
}
#endif
