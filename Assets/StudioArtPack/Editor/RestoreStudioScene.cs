using System;
using System.IO;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;

// Replays the recovered art passes on a fresh checkout without changing gameplay.
public static class RestoreStudioScene
{
    public static void BuildBatch()
    {
        if (!Application.isBatchMode) throw new InvalidOperationException("Use batch mode for recovery.");
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/PrototypeScene.unity");
        if (scene.GetRootGameObjects().Any(go => go.name == "ExhibitionStudioV1"))
            throw new InvalidOperationException("Studio is already present; preserve current scene edits.");
        Directory.CreateDirectory(".local-backups/ExhibitionStudioPreview");
        ExhibitionStudioBuilder.BuildBatch();
        CartoonStudioPolisher.BuildBatch();
        StudioAtmospherePolisher.BuildBatch();
        StudioAtmospherePolisher.RefreshPreviewBatch();
        StudioReferenceFinisher.BuildBatch();
        StudioReferenceFinisher.RefreshFoliageBatch();
        Debug.Log("STUDIO_RECOVERY_OK: recovered V4 scene and soft music assembled.");
    }
}
