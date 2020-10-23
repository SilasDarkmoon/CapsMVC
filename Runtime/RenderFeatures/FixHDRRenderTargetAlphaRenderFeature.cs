using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class FixHDRRenderTargetAlphaRenderFeature : ComponentBasedRenderFeature
{
    class FixHDRRenderTargetAlphaRecreatePass : ScriptableRenderPass
    {
        protected string _ProfilerTag = "FixHDRRenderTargetAlphaRecreatePass";
        protected RenderTargetHandle _CameraRenderTarget;

        public FixHDRRenderTargetAlphaRecreatePass()
        {
            renderPassEvent = RenderPassEvent.BeforeRendering;
            _CameraRenderTarget.Init("_CameraColorTexture");
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            ConfigureTarget(BuiltinRenderTextureType.CameraTarget, BuiltinRenderTextureType.CameraTarget);
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            renderingData.cameraData.cameraTargetDescriptor.colorFormat = RenderTextureFormat.ARGBHalf;
            var desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;
            var cmd = CommandBufferPool.Get(_ProfilerTag);
            cmd.ReleaseTemporaryRT(_CameraRenderTarget.id);
            cmd.GetTemporaryRT(_CameraRenderTarget.id, desc, FilterMode.Bilinear);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.isHdrEnabled)
        {
            var curFormat = renderingData.cameraData.cameraTargetDescriptor.colorFormat;
            if (curFormat == RenderTextureFormat.ARGBHalf
                //|| curFormat == RenderTextureFormat.BGRA10101010_XR
                )
            {
                return;
            }
            renderer.EnqueuePass(_RecreatePass);
        }
    }

    FixHDRRenderTargetAlphaRecreatePass _RecreatePass;
    protected override void Awake()
    {
        base.Awake();
        _RecreatePass = new FixHDRRenderTargetAlphaRecreatePass();
    }
}