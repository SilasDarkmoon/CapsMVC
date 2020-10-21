using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class ForcePreserveHDRRenderTargetAlpha : ComponentBasedRenderFeature
{
    class FixHDRRenderTargetAlphaPass : ScriptableRenderPass
    {
        protected string _ProfilerTag = "FixHDRRenderTargetAlphaPass";
        protected RenderTargetHandle _CameraRenderTarget;

        public FixHDRRenderTargetAlphaPass()
        {
            _CameraRenderTarget.Init("_CameraColorTexture");
            renderPassEvent = RenderPassEvent.BeforeRendering;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.isHdrEnabled)
            {
                renderingData.cameraData.cameraTargetDescriptor.colorFormat = RenderTextureFormat.ARGBHalf;
                var cmd = CommandBufferPool.Get(_ProfilerTag);
                cmd.ReleaseTemporaryRT(_CameraRenderTarget.id);
                cmd.GetTemporaryRT(_CameraRenderTarget.id, renderingData.cameraData.cameraTargetDescriptor, FilterMode.Bilinear);
                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
        }
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.isHdrEnabled)
        {
            renderer.EnqueuePass(_Pass);
        }
    }

    FixHDRRenderTargetAlphaPass _Pass;
    protected override void Awake()
    {
        base.Awake();
        _Pass = new FixHDRRenderTargetAlphaPass();
    }
}