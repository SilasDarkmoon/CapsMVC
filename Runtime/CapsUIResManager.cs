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
        private const int SceneAndDialogCacheLayer = 17;
        private const string UICameraName = "UICameraAndEventSystem(Clone)";

        //public static Camera GetUICamera()
        //{
        //    return _UICamera;
        //}

        public static Camera FindUICamera()
        {
            if (_UICamera != null && !_UICamera.isActiveAndEnabled) _UICamera = null;
            //if (_UICamera == null) _UICamera = Camera.main;
            //if (_UICamera == null) _UICamera = GameObject.FindObjectOfType<Camera>();

            if (_UICamera == null) CreateCameraAndEventSystem();
            return _UICamera;
        }

        private static GameObject _UICameraAndEventSystemTemplate = null;
        private static GameObject _UICameraAndEventSystemGo = null;
        public static Camera CreateCameraAndEventSystem()
        {
            if (_UICameraAndEventSystemTemplate == null) _UICameraAndEventSystemTemplate = ResManager.LoadResDeep("UICameraAndEventSystem.prefab") as GameObject;
            if (_UICameraAndEventSystemGo != null && _UICamera != null) return _UICamera;
            _UICameraAndEventSystemGo = GameObject.Instantiate(_UICameraAndEventSystemTemplate);
            _UICamera = _UICameraAndEventSystemGo.GetComponentInChildren<Camera>();
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
                if ((obj.layer == SceneAndDialogCacheLayer) ||
                    (UICameraName.Equals(obj.name)))
                {
                    continue;
                }
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

        public static void ChangeGameObjectLayer(GameObject go, int layer)
        {
            //遍历当前物体及其所有子物体
            foreach (Transform tran in go.GetComponentsInChildren<Transform>(true))
            {
                tran.gameObject.layer = layer;//更改物体的Layer层
            }
        }
    }
}