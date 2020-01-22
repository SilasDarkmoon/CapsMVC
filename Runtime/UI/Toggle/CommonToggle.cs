using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Capstones.UnityEngineEx;

namespace Capstones.UnityEngineEx.UI
{
    public enum ButtonGroupType
    {
        Static,
        Dynamic
    }

    public class CommonToggle : ToggleGroup
    {
        public ButtonGroupType ButtonGroupType;
        public GameObject ToggleObj;
        public string TogglePrefab;
        public int TogglesCount;
        public List<GameObject> Toggles;
        public bool AllowReselect;
        private bool isTriggerLuaListener = true;
        private CapsUnityLuaBehav luaBehav;
        private int m_cache_tag;

        protected override void Awake()
        {
            base.Awake();

            if (ButtonGroupType == ButtonGroupType.Dynamic)
            {
                Toggles = new List<GameObject>();

                GameObject prefabObj = null;
                if (!string.IsNullOrEmpty(TogglePrefab))
                {
                    prefabObj = ResManager.LoadRes(TogglePrefab) as GameObject;
                }

                for (int i = 0; i < TogglesCount; i++)
                {
                    var obj = GameObject.Instantiate(prefabObj != null ? prefabObj : ToggleObj, transform);

                    if (obj)
                    {
                        if (!obj.activeSelf)
                        {
                            obj.SetActive(true);
                        }

                        Toggles.Add(obj);
                    }
                }

            }
            for (int i = 0; i < Toggles.Count; i++)
            {
                int tag = i + 1;
                GameObject obj = Toggles[i];
                Debug.Assert(obj.GetComponent<UnityEngine.UI.Toggle>(), "Toggle object must has a toggle compoment");
                CapsUnityLuaBehav btnLua = obj.GetComponent<CapsUnityLuaBehav>();
                luaBehav.CallLuaFunc<CapsUnityLuaBehav, int>("onToggleCreated", btnLua, tag);
                Toggle t = obj.GetComponent<Toggle>();
                t.group = this;
                t.onValueChanged = new Toggle.ToggleEvent();
                t.onValueChanged.AddListener((bool active) =>
                {
                    if (isTriggerLuaListener)
                    {
                        CapsUnityLuaBehav btnLua2 = Toggles[tag - 1].GetComponent<CapsUnityLuaBehav>();
                        if (active)
                        {
                            if (AllowReselect)
                                m_cache_tag = tag;
                            else if (tag == m_cache_tag)
                                return;
                            else
                                m_cache_tag = tag;

                            luaBehav.CallLuaFunc<CapsUnityLuaBehav, int>("onToggleSelected", btnLua2, tag);
                            luaBehav.CallLuaFunc<CapsUnityLuaBehav, int>("onTagSwitched", btnLua2, tag);
                        }
                        else
                        {
                            luaBehav.CallLuaFunc<CapsUnityLuaBehav, int>("onToggleDeselected", btnLua2, tag);
                        }
                    }
                });
            }
        }

        public void SelectDefaultToggle(int tag)
        {
            m_cache_tag = tag;
            isTriggerLuaListener = false;
            for (int i = 0; i < Toggles.Count; i++)
            {
                int toggleTag = i + 1;
                GameObject obj = Toggles[i];
                CapsUnityLuaBehav btnLua = obj.GetComponent<CapsUnityLuaBehav>();
                if (toggleTag == tag)
                {
                    obj.GetComponent<Toggle>().isOn = true;
                    luaBehav.CallLuaFunc<CapsUnityLuaBehav, int>("onToggleSelected", btnLua, tag);
                    luaBehav.CallLuaFunc<CapsUnityLuaBehav, int>("onTagSwitched", btnLua, tag);
                }
                else
                {
                    obj.GetComponent<Toggle>().isOn = false;
                    luaBehav.CallLuaFunc<CapsUnityLuaBehav, int>("onToggleDeselected", btnLua, tag);
                }
            }
            isTriggerLuaListener = true;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}