using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

using Object = UnityEngine.Object;

namespace Capstones.UnityEngineEx
{
    [InitializeOnLoad]
    public static class StackingCameraFixEditor
    {
        static StackingCameraFixEditor()
        {
            EditorSceneManager.sceneSaving += (scene, path) =>
            {
                FixScene(scene);
            };
        }

        private static bool FixScene(Scene scene)
        {
            bool changed = false;
            var roots = scene.GetRootGameObjects();
            for (int j = 0; j < roots.Length; ++j)
            {
                var root = roots[j];
                if (PrefabUtility.GetPrefabInstanceStatus(root) == PrefabInstanceStatus.NotAPrefab)
                {
                    var cams = root.GetComponentsInChildren<Camera>();
                    for (int k = 0; k < cams.Length; ++k)
                    {
                        var cam = cams[k];
                        if (PrefabUtility.GetPrefabInstanceStatus(cam) == PrefabInstanceStatus.NotAPrefab)
                        {
                            if (!cam.GetComponent<StackingMainCamera>() && !cam.GetComponent<StackingDynamicCamera>())
                            {
                                if (cam.targetTexture == null)
                                {
                                    cam.gameObject.AddComponent<StackingDynamicCamera>();
                                    changed = true;
                                }
                            }
                        }
                    }
                }
            }
            return changed;
        }
        private static bool FixPrefab(GameObject root)
        {
            bool changed = false;
            var cams = root.GetComponentsInChildren<Camera>();
            for (int k = 0; k < cams.Length; ++k)
            {
                var cam = cams[k];
                if (PrefabUtility.GetPrefabInstanceStatus(cam) == PrefabInstanceStatus.NotAPrefab)
                {
                    if (!cam.GetComponent<StackingMainCamera>() && !cam.GetComponent<StackingDynamicCamera>())
                    {
                        if (cam.targetTexture == null)
                        {
                            cam.gameObject.AddComponent<StackingDynamicCamera>();
                            changed = true;
                        }
                    }
                }
            }
            if (changed)
            {
                PrefabUtility.SavePrefabAsset(root);
            }
            return changed;
        }
        private static bool FixPrefab(string path)
        {
            var prefab = AssetDatabase.LoadMainAssetAtPath(path) as GameObject;
            if (prefab)
            {
                return FixPrefab(prefab);
            }
            return false;
        }

        [MenuItem("Mods/Client Update Fix - Stacking Camera", priority = 100020)]
        private static void UpdateFixStackingCamera()
        {
            var allassets = AssetDatabase.GetAllAssetPaths();
            for (int i = 0; i < allassets.Length; ++i)
            {
                var path = allassets[i];
                if (path.EndsWith(".unity", StringComparison.InvariantCultureIgnoreCase)
                    || path.EndsWith(".u3d", StringComparison.InvariantCultureIgnoreCase)
                    || path.EndsWith(".prefab", StringComparison.InvariantCultureIgnoreCase)
                    )
                {
                    var asset = AssetDatabase.LoadMainAssetAtPath(path);
                    if (asset is SceneAsset)
                    {
                        var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(path, UnityEditor.SceneManagement.OpenSceneMode.Additive);
                        if (FixScene(scene))
                        {
                            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
                        }
                        UnityEditor.SceneManagement.EditorSceneManager.CloseScene(scene, true);
                        Resources.UnloadAsset(asset);
                    }
                    else if (asset is GameObject)
                    {
                        var root = asset as GameObject;
                        FixPrefab(root);
                    }
                    else
                    {
                        Resources.UnloadAsset(asset);
                    }
                }
            }
        }

        private class StackingCameraFixEditorPostprocessor : AssetPostprocessor
        {
            private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
            {
                if (importedAssets != null)
                {
                    for (int i = 0; i < importedAssets.Length; ++i)
                    {
                        var asset = importedAssets[i];
                        if (asset.EndsWith(".prefab"))
                        {
                            FixPrefab(asset);
                        }
                    }
                }
            }
        }
    }
}