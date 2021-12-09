using Capstones.UnityEngineEx;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Capstones.UnityEditorEx
{
    [InitializeOnLoad]
    public class AnimOnlyModelResBuilder : CapsResBuilder.IResBuilderEx
    {
        private static AnimOnlyModelResBuilder _Instance = new AnimOnlyModelResBuilder();
        static AnimOnlyModelResBuilder()
        {
            CapsResBuilder.ResBuilderEx.Add(_Instance);
        }

        public void Prepare(string output)
        {
        }
        public void Cleanup()
        {
        }
        public void OnSuccess()
        {
        }
        public string FormatBundleName(string asset, string mod, string dist, string norm)
        {
            if (asset.EndsWith(".animonly.txt"))
            {
                if (System.IO.Path.GetFileName(asset) == "builder.animonly.txt")
                {
                    var dir = System.IO.Path.GetDirectoryName(asset);
                    var files = System.IO.Directory.GetFiles(dir);
                    for (int i = 0; i < files.Length; ++i)
                    {
                        var file = files[i].Replace('\\', '/');
                        if (System.IO.File.Exists(file))
                        {
                            if (AssetImporter.GetAtPath(file) is ModelImporter)
                            {
                                DeleteAllSubAssetsExceptAnim(file);
                            }
                        }
                    }
                }
                else if (asset.EndsWith(".builder.animonly.txt"))
                {
                    var file = asset.Substring(0, asset.Length - ".builder.animonly.txt".Length);
                    if (System.IO.File.Exists(file))
                    {
                        DeleteAllSubAssetsExceptAnim(file);
                    }
                }
            }
            return null;
        }
        public bool CreateItem(CapsResManifestNode node)
        {
            return false;
        }
        public void ModifyItem(CapsResManifestItem item)
        {
        }

        public void GenerateBuildWork(string bundleName, IList<string> assets, ref AssetBundleBuild abwork, CapsResBuilder.CapsResBuildWork modwork, int abindex)
        {
        }

        public static void DeleteAllSubAssetsExceptAnim(string assetpath)
        {
            var assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetpath);
            for (int i = 0; i < assets.Length; ++i)
            {
                var asset = assets[i];
                if (!(asset is AnimationClip))
                {
                    GameObject.DestroyImmediate(asset, true);
                }
            }
        }
    }
}