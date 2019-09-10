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

        public class PackedSceneObjs
        {
            public readonly List<GameObject> SceneObjs = new List<GameObject>();
            public readonly List<GameObject> DialogObjs = new List<GameObject>();
            public readonly List<bool> InitialSceneObjsActive = new List<bool>();
            public readonly List<bool> InitialDialogObjsActive = new List<bool>();
        }

        public static PackedSceneObjs PackSceneAndDialogs(GameObject[] dialogObjs)
        {
            var oldObjs = ResManager.FindAllGameObject();

            var ret = new PackedSceneObjs();
            foreach (var obj in oldObjs)
            {
                var canvas = obj.GetComponent<Canvas>();
                if (canvas != null && canvas.sortingLayerName == "Dialog")
                {
                    bool find = false;
                    if (dialogObjs != null)
                    {
                        foreach (var dialogObj in dialogObjs)
                        {
                            if (dialogObj != obj) continue;
                            find = true;
                            break;
                        }
                    }
                    if (find)
                    {
                        ret.DialogObjs.Add(obj);
                        //ret.InitialDialogObjsActive.Add(obj.activeSelf);
                        continue;
                    }
                    ret.SceneObjs.Add(obj);
                    ret.InitialSceneObjsActive.Add(obj.activeSelf);
                    continue;
                }
                var canvasGroup = obj.GetComponent<CanvasGroup>();
                if (canvasGroup != null && canvasGroup.blocksRaycasts == false)
                {
                    GameObject.Destroy(obj);
                    continue;
                }
                ret.SceneObjs.Add(obj);
                ret.InitialSceneObjsActive.Add(obj.activeSelf);
            }
            // 按order从小到大排序，这样才能和lua中的记录对应上
            ret.DialogObjs.Sort((x, y) =>
            {
                var orderX = x.transform.GetComponent<Canvas>().sortingOrder;
                var orderY = y.transform.GetComponent<Canvas>().sortingOrder;
                return orderX - orderY;
            });
            for (int i = 0; i < ret.DialogObjs.Count; ++i)
            {
                ret.InitialDialogObjsActive.Add(ret.DialogObjs[i].activeSelf);
            }
            return ret;
        }

        public static List<GameObject> PackSceneObj()
        {
            return ResManager.FindAllGameObject();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnUnityStart()
        {
            // This force the appdomain to load this assembly.
        }
    }
}