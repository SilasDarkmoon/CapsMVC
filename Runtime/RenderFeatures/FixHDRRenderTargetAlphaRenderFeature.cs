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
    //class FixHDRRenderTargetAlphaApplyPass : ScriptableRenderPass
    //{
    //    protected RenderTargetHandle _CameraRenderTarget;
    //    public RenderTargetIdentifier _CameraDepthTarget;
    //    public Color _ClearColor;

    //    public FixHDRRenderTargetAlphaApplyPass()
    //    {
    //        renderPassEvent = RenderPassEvent.AfterRenderingPrePasses + 1;
    //        _CameraRenderTarget.Init("_CameraColorTexture");
    //    }

    //    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    //    {
    //        ConfigureTarget(_CameraRenderTarget.Identifier(), _CameraDepthTarget);
    //        ConfigureClear(ClearFlag.Color, _ClearColor);
    //    }
    //    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    //    {
    //    }
    //}

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
            //_ApplyPass._CameraDepthTarget = renderer.cameraDepth;
            //_ApplyPass._ClearColor = CoreUtils.ConvertSRGBToActiveColorSpace(renderingData.cameraData.camera.backgroundColor);
            renderer.EnqueuePass(_RecreatePass);
            //renderer.EnqueuePass(_ApplyPass);
        }
    }

    FixHDRRenderTargetAlphaRecreatePass _RecreatePass;
    //FixHDRRenderTargetAlphaApplyPass _ApplyPass;
    protected override void Awake()
    {
        base.Awake();
        _RecreatePass = new FixHDRRenderTargetAlphaRecreatePass();
        //_ApplyPass = new FixHDRRenderTargetAlphaApplyPass();
    }
}