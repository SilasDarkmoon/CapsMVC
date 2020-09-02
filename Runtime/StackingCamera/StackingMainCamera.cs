using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Capstones.UnityEngineEx;

[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
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
        _Instance = this;
        ManageCameraStackRaw();
    }
    //private void Update()
    //{
    //    if (_Camera)
    //    {
    //        GetSceneCameras();
    //        ManageCameraStackRaw();
    //    }
    //}
    private void OnDestroy()
    {
        if (_Instance == this)
        {
            _Instance = null;
        }
    }

    private void ManageCameraStackRaw()
    {
        if (_Camera)
        {
            _SceneCameras.RemoveWhere(scenecam => !scenecam);
            var cameras = _SceneCameras;
            var stack = _CameraEx.cameraStack;
            for (int i = stack.Count - 1; i >= 0; --i)
            {
                var old = stack[i];
                if (!old || !cameras.Contains(old))
                {
                    stack.RemoveAt(i);
                }
            }

            var oldcameras = new HashSet<Camera>(stack);
            bool newcam = false;
            foreach (var cam in cameras)
            {
                if (cam != _Camera && !oldcameras.Contains(cam))
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
    public static void ManageCameraStack()
    {
        var instance = Instance;
        if (instance)
        {
            instance.ManageCameraStackRaw();
        }
    }

    private static StackingMainCamera _Instance;
    private static bool _IsInstanceCreating;
    public static StackingMainCamera Instance
    {
        get
        {
            if (!_Instance && !_IsInstanceCreating)
            {
                _IsInstanceCreating = true;
                UIResManager.FindUICamera();
                _IsInstanceCreating = false;
            }
            return _Instance;
        }
    }

    private static HashSet<Camera> _SceneCameras = new HashSet<Camera>();
    public static HashSet<Camera> GetSceneCameras()
    {
        var cams = _SceneCameras;
        cams.Clear();
        cams.UnionWith(Object.FindObjectsOfType<Camera>());
        return cams;
    }
    public static void RegSceneCamera(Camera cam)
    {
        if (cam)
        {
            _SceneCameras.Add(cam);
            ManageCameraStack();
        }
    }
    public static void UnregSceneCamera(Camera cam)
    {
        _SceneCameras.Remove(cam);
        ManageCameraStack();
    }
}