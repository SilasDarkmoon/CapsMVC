using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class StackingMainCamera : MonoBehaviour
{
    private Camera _Camera;
    private UniversalAdditionalCameraData _CameraEx;
    private Camera _FirstCameraInStack;

    private void Awake()
    {
        _Camera = GetComponent<Camera>();
        _CameraEx = GetComponent<UniversalAdditionalCameraData>();
        if (_Camera && !_CameraEx)
        {
            _CameraEx = gameObject.AddComponent<UniversalAdditionalCameraData>();
        }
        _CameraEx.renderType = CameraRenderType.Base;
    }

    private void Update()
    {
        if (_Camera)
        {
            var stack = _CameraEx.cameraStack;
            for (int i = stack.Count - 1; i >= 0; --i)
            {
                var old = stack[i];
                if (!old)
                {
                    stack.RemoveAt(i);
                }
            }

            var cameras = GetSceneCameras();
            bool newcam = false;
            for (int i = 0; i < cameras.Count; ++i)
            {
                var cam = cameras[i];
                if (cam != _Camera && !stack.Contains(cam))
                {
                    if (cam.targetTexture == null)
                    {
                        var camex = cam.gameObject.GetComponent<UniversalAdditionalCameraData>();
                        if (!camex)
                        {
                            camex = cam.gameObject.AddComponent<UniversalAdditionalCameraData>();
                        }
                        camex.renderType = CameraRenderType.Overlay;
                        stack.Add(cam);
                        newcam = true;
                    }
                }
            }

            if (newcam)
            {
                stack.Sort((cam1, cam2) =>
                {
                    if (cam1.depth == cam2.depth) return 0;
                    else if (cam1.depth > cam2.depth) return 1;
                    else return -1;
                });
            }

            Camera first = null;
            if (stack.Count > 0)
            {
                first = stack[0];
            }
            if (first != _FirstCameraInStack)
            {
                _FirstCameraInStack = first;
                if (_FirstCameraInStack.clearFlags == CameraClearFlags.Skybox)
                {
                    _Camera.clearFlags = CameraClearFlags.Skybox;
                }
                else
                {
                    _Camera.clearFlags = CameraClearFlags.SolidColor;
                }
            }
        }
    }

    private static List<Camera> _SceneCameras = new List<Camera>();
    public static List<Camera> GetSceneCameras()
    {
        var cams = _SceneCameras;
        cams.Clear();
        cams.AddRange(Object.FindObjectsOfType<Camera>());
        return cams;
    }
}