using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RenderTargetHandleHolder : MonoBehaviour
{
    public string RenderTargetName;
    public Vector2 Size;
    public bool IgnoreCreation;

    private Camera _Camera;
    private RenderTargetHandle _RenderTarget;

    private void Awake()
    {
        if (!string.IsNullOrEmpty(RenderTargetName))
        {
            _RenderTarget.Init(RenderTargetName);

            if (!IgnoreCreation)
            {
                _Camera = GetComponent<Camera>();
                if (!_Camera)
                {
                    _Camera = Camera.main;
                }
                var cmd = new CommandBuffer();
                if (Size.x <= 0f || Size.y <= 0f)
                {
                    Size = new Vector2(_Camera.pixelWidth, _Camera.pixelHeight);
                }

                RenderTextureDescriptor descriptor = new RenderTextureDescriptor((int)Size.x, (int)Size.y);
                descriptor.colorFormat = RenderTextureFormat.Default;
                descriptor.sRGB = (QualitySettings.activeColorSpace == ColorSpace.Linear);
                descriptor.msaaSamples = 1;
                descriptor.depthBufferBits = 0;
                descriptor.enableRandomWrite = false;
                descriptor.bindMS = false;
                descriptor.useDynamicScale = false;

                cmd.GetTemporaryRT(_RenderTarget.id, descriptor, FilterMode.Point);
            }
        }
    }

    private void OnDestroy()
    {
        if (string.IsNullOrEmpty(RenderTargetName))
        {
            return;
        }

        var cmd = new CommandBuffer();
        cmd.ReleaseTemporaryRT(_RenderTarget.id);
        Graphics.ExecuteCommandBuffer(cmd);
    }
}
