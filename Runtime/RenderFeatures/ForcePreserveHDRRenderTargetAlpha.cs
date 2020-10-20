using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ForcePreserveHDRRenderTargetAlpha : ScriptableRendererFeature
{
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.isHdrEnabled)
        {
            renderingData.cameraData.cameraTargetDescriptor.colorFormat = RenderTextureFormat.ARGBHalf;
        }
    }

    public override void Create()
    {
    }
}