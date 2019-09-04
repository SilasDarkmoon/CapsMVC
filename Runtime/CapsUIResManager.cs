using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Capstones.UnityEngineEx
{
    public static class UIResManager
    {
        private static Camera _UICamera;
        public static Camera GetUICamera()
        {
            return _UICamera;
        }

        public static Camera FindUICamera()
        {
            if (_UICamera != null && !_UICamera.isActiveAndEnabled) _UICamera = null;
            if (_UICamera == null) _UICamera = Camera.main;
            if (_UICamera == null) _UICamera = GameObject.FindObjectOfType<Camera>();
            return _UICamera;
        }

        private static GameObject _UICameraAndEventSystemTemplate = null;
        public static Camera CreateCameraAndEventSystem()
        {
            if (_UICameraAndEventSystemTemplate == null) _UICameraAndEventSystemTemplate = ResManager.LoadResDeep("UICameraAndEventSystem.prefab") as GameObject;
            var container = GameObject.Instantiate(_UICameraAndEventSystemTemplate);
            _UICamera = container.GetComponentInChildren<Camera>();
            return _UICamera;
        }
    }
}