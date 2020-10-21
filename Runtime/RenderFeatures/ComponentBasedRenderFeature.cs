using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public abstract class ComponentBasedRenderFeature : MonoBehaviour
{
    protected Camera _Camera;
    public Camera Camera { get { return _Camera; } }

    protected virtual void Awake()
    {
        _Camera = GetComponent<Camera>();
    }
    private void OnEnable()
    {
        if (_Camera)
        {
            UniversalRenderFeature.RegRenderFeature(this);
        }
    }
    private void OnDisable()
    {
        if (_Camera)
        {
            UniversalRenderFeature.UnregRenderFeature(this);
        }
    }

    public abstract void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData);
}