using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capstones.UnityEngineEx.UI
{
    [ExecuteInEditMode]
    public class DynamicLayer : MonoBehaviour
    {
        public string LayerName;
        public bool AffectChildren;
        [SerializeField, HideInInspector] protected int AssignedLayer;

        protected virtual void OnEnable()
        {
            if (AllUsableLayers.Count > 0 && !string.IsNullOrEmpty(LayerName))
            {
                int layer = 0;
                if (!UsingNameToID.TryGetValue(LayerName, out layer))
                {
                    if (AssignedLayer > 0 && AllUsableLayers.Contains(AssignedLayer) && LayersInUsing[AssignedLayer] == null)
                    {
                        layer = AssignedLayer;
                        LayersInUsing[layer] = LayerName;
                        UsingNameToID[LayerName] = layer;
                    }
                    else
                    {
                        foreach (var candiLayer in AllUsableLayers)
                        {
                            if (LayersInUsing[candiLayer] == null)
                            {
                                layer = candiLayer;
                                LayersInUsing[layer] = LayerName;
                                UsingNameToID[LayerName] = layer;
                                break;
                            }
                        }
                    }
                }
                if (layer > 0)
                {
                    AssignedLayer = layer;
                    ComponentsInLayer[layer].Add(this);
                    ApplyLayer();
                }
            }
        }
        protected virtual void OnDisable()
        {
            if (AssignedLayer > 0 && AssignedLayer < 32)
            {
                RestoreLayer();
                ComponentsInLayer[AssignedLayer].Remove(this);
                if (ComponentsInLayer[AssignedLayer].Count == 0)
                {
                    UsingNameToID.Remove(LayerName);
                    LayersInUsing[AssignedLayer] = null;
                }
                AssignedLayer = 0;
            }
        }
        protected virtual void ApplyLayer()
        {
            ApplyLayer(gameObject, AssignedLayer, AffectChildren);
        }
        protected virtual void RestoreLayer()
        {
            RestoreLayer(gameObject, AssignedLayer, AffectChildren);
        }

        private static readonly string[] LayersInUsing = new string[32];
        private static readonly Dictionary<string, int> UsingNameToID = new Dictionary<string, int>();
        private static readonly HashSet<DynamicLayer>[] ComponentsInLayer = new HashSet<DynamicLayer>[32];
        static DynamicLayer()
        {
            for (int i = 0; i < ComponentsInLayer.Length; ++i)
            {
                ComponentsInLayer[i] = new HashSet<DynamicLayer>();
            }
        }
        private static readonly HashSet<int> AllUsableLayers = new HashSet<int>();
        public static void ClearUsableLayers()
        {
            AllUsableLayers.Clear();
        }
        public static void AddUsableLayer(int layer)
        {
            if (layer > 0 && layer < 32)
            {
                AllUsableLayers.Add(layer);
            }
        }
        public static void AddUsableLayers(params int[] layers)
        {
            for (int i = 0; i < layers.Length; ++i)
            {
                AddUsableLayer(layers[i]);
            }
        }

        public static void ApplyLayer(GameObject go, int layer, bool recursive)
        {
            go.layer = layer;
            if (recursive)
            {
                var trans = go.transform;
                for (int i = 0; i < trans.childCount; ++i)
                {
                    var child = trans.GetChild(i).gameObject;
                    if ((child.layer == 0 || AllUsableLayers.Contains(child.layer)) && !child.GetComponent<DynamicLayer>())
                    {
                        ApplyLayer(child, layer, true);
                    }
                }
            }
        }
        public static void RestoreLayer(GameObject go, int fromLayer, bool recursive)
        {
            go.layer = 0;
            if (recursive)
            {
                var trans = go.transform;
                for (int i = 0; i < trans.childCount; ++i)
                {
                    var child = trans.GetChild(i).gameObject;
                    if (child.layer == fromLayer && !child.GetComponent<DynamicLayer>())
                    {
                        RestoreLayer(child, fromLayer, true);
                    }
                }
            }
        }
    }
}