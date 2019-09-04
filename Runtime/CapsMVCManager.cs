using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Capstones.UnityEngineEx
{
    public static class MVCManager
    {
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
    }
}