using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class UniversalRenderFeature : ScriptableRendererFeature
{
    protected static readonly Dictionary<Camera, List<ComponentBasedRenderFeature>> _RegisteredFeatures = new Dictionary<Camera, List<ComponentBasedRenderFeature>>();
    protected static readonly List<List<ComponentBasedRenderFeature>> _RegisteredTypedFeatures = new List<List<ComponentBasedRenderFeature>>();
    public static void RegRenderFeature(ComponentBasedRenderFeature feature)
    {
        var cam = feature.Camera;
        if (cam)
        {
            List<ComponentBasedRenderFeature> list;
            if (!_RegisteredFeatures.TryGetValue(cam, out list))
            {
                list = new List<ComponentBasedRenderFeature>();
                _RegisteredFeatures.Add(cam, list);
            }
            if (!list.Contains(feature))
            {
                list.Add(feature);
            }
        }
        else
        {
            var type = feature.TargetCameraType;
            int tindex = (int)type;
            while (_RegisteredTypedFeatures.Count <= tindex)
            {
                _RegisteredTypedFeatures.Add(new List<ComponentBasedRenderFeature>());
            }
            var list = _RegisteredTypedFeatures[tindex];
            if (!list.Contains(feature))
            {
                list.Add(feature);
            }
        }
    }
    public static void UnregRenderFeature(ComponentBasedRenderFeature feature)
    {
        {
            List<ComponentBasedRenderFeature> list;
            var cam = feature.Camera;
            if (_RegisteredFeatures.TryGetValue(cam, out list))
            {
                list.Remove(feature);
                if (list.Count == 0)
                {
                    _RegisteredFeatures.Remove(cam);
                }
            }
        }
        var type = feature.TargetCameraType;
        int tindex = (int)type;
        if (_RegisteredTypedFeatures.Count > tindex)
        {
            var list = _RegisteredTypedFeatures[tindex];
            list.Remove(feature);
        }
        RemoveDeadRenderFeatures();
    }
    public static void RemoveRenderFeature(ComponentBasedRenderFeature feature)
    {
        var cams = _RegisteredFeatures.Keys.ToArray();
        for (int i = 0; i < cams.Length; ++i)
        {
            var cam = cams[i];
            if (!cam)
            {
                _RegisteredFeatures.Remove(cam);
            }
            else
            {
                var list = _RegisteredFeatures[cam];
                for (int j = list.Count - 1; j >= 0; --j)
                {
                    if (feature == list[j])
                    {
                        list.RemoveAt(j);
                    }
                }
                if (list.Count == 0)
                {
                    _RegisteredFeatures.Remove(cam);
                }
            }
        }
        for (int i = 0; i < _RegisteredTypedFeatures.Count; ++i)
        {
            var list = _RegisteredTypedFeatures[i];
            for (int j = list.Count - 1; j >= 0; --j)
            {
                if (feature == list[j])
                {
                    list.RemoveAt(j);
                }
            }
        }
    }
    public static void RemoveDeadRenderFeatures()
    {
        var cams = _RegisteredFeatures.Keys.ToArray();
        for (int i = 0; i < cams.Length; ++i)
        {
            var cam = cams[i];
            if (!cam)
            {
                _RegisteredFeatures.Remove(cam);
            }
            else
            {
                var list = _RegisteredFeatures[cam];
                for (int j = list.Count - 1; j >= 0; --j)
                {
                    var feature = list[j];
                    if (!feature)
                    {
                        list.RemoveAt(j);
                    }
                }
                if (list.Count == 0)
                {
                    _RegisteredFeatures.Remove(cam);
                }
            }
        }
        for (int i = 0; i < _RegisteredTypedFeatures.Count; ++i)
        {
            var list = _RegisteredTypedFeatures[i];
            for (int j = list.Count - 1; j >= 0; --j)
            {
                var feature = list[j];
                if (!feature)
                {
                    list.RemoveAt(j);
                }
            }
        }
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        {
            var cam = renderingData.cameraData.camera;
            List<ComponentBasedRenderFeature> list;
            if (_RegisteredFeatures.TryGetValue(cam, out list))
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    var feature = list[i];
                    feature.AddRenderPasses(renderer, ref renderingData);
                }
            }
        }

        var type = renderingData.cameraData.camera.cameraType;
        int tindex = (int)type;
        if (_RegisteredTypedFeatures.Count > tindex)
        {
            var list = _RegisteredTypedFeatures[tindex];
            for (int i = 0; i < list.Count; ++i)
            {
                var feature = list[i];
                feature.AddRenderPasses(renderer, ref renderingData);
            }
        }
    }

    public override void Create()
    {
    }
}