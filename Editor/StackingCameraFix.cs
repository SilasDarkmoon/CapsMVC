using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Object = UnityEngine.Object;

namespace Capstones.UnityEngineEx
{
    public static class StackingCameraFixEditor
    {
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
                        bool changed = false;
                        var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(path, UnityEditor.SceneManagement.OpenSceneMode.Additive);
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
                        if (changed)
                        {
                            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
                        }
                        UnityEditor.SceneManagement.EditorSceneManager.CloseScene(scene, true);
                        Resources.UnloadAsset(asset);
                    }
                    else if (asset is GameObject)
                    {
                        bool changed = false;
                        var root = asset as GameObject;
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
                    }
                    else
                    {
                        Resources.UnloadAsset(asset);
                    }
                }
            }
        }
    }
}