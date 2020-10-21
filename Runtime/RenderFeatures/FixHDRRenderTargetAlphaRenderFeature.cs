using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class FixHDRRenderTargetAlphaRenderFeature : ComponentBasedRenderFeature
{
    class FixHDRRenderTargetAlphaRecreatePass : ScriptableRenderPass
    {
        protected string _ProfilerTag = "FixHDRRenderTargetAlphaPass";
        protected RenderTargetHandle _CameraRenderTarget;
        public ScriptableRenderer _Renderer;

        public FixHDRRenderTargetAlphaRecreatePass()
        {
            renderPassEvent = RenderPassEvent.BeforeRendering;
            _CameraRenderTarget.Init("_CameraColorTexture");
            ConfigureTarget(RenderTargetHandle.CameraTarget.Identifier());
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            renderingData.cameraData.cameraTargetDescriptor.colorFormat = RenderTextureFormat.ARGBHalf;
            var cmd = CommandBufferPool.Get(_ProfilerTag);
            cmd.ReleaseTemporaryRT(_CameraRenderTarget.id);
            cmd.GetTemporaryRT(_CameraRenderTarget.id, renderingData.cameraData.cameraTargetDescriptor, FilterMode.Bilinear);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
    class FixHDRRenderTargetAlphaApplyPass : ScriptableRenderPass
    {
        protected RenderTargetHandle _CameraRenderTarget;

        public FixHDRRenderTargetAlphaApplyPass()
        {
            renderPassEvent = RenderPassEvent.BeforeRendering;
            _CameraRenderTarget.Init("_CameraColorTexture");
            ConfigureTarget(_CameraRenderTarget.Identifier());
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
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
            _RecreatePass._Renderer = renderer;
            renderer.EnqueuePass(_RecreatePass);
            renderer.EnqueuePass(_ApplyPass);
        }
    }

    FixHDRRenderTargetAlphaRecreatePass _RecreatePass;
    FixHDRRenderTargetAlphaApplyPass _ApplyPass;
    protected override void Awake()
    {
        base.Awake();
        _RecreatePass = new FixHDRRenderTargetAlphaRecreatePass();
        _ApplyPass = new FixHDRRenderTargetAlphaApplyPass();
    }
}