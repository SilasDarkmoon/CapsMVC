using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Capstones.UnityEngineEx.UI
{
    [ExecuteInEditMode]
    public class DynamicLayerCamera : DynamicLayer
    {
        private Camera _Camera;

        protected override void OnEnable()
        {
            if (AssignedLayer > 0)
            {
                if (!_Camera)
                {
                    _Camera = GetComponent<Camera>();
                }
                if (_Camera)
                {
                    _Camera.cullingMask &= ~(1 << AssignedLayer);
                }
            }
            base.OnEnable();
        }

        protected override void ApplyLayer()
        {
            base.ApplyLayer();
            if (AssignedLayer > 0)
            {
                if (!_Camera)
                {
                    _Camera = GetComponent<Camera>();
                }
                if (_Camera)
                {
                    _Camera.cullingMask |= 1 << AssignedLayer;
                }
            }
        }
        protected override void RestoreLayer()
        {
            if (AssignedLayer > 0 && _Camera)
            {
                var mask = 1 << AssignedLayer;
                if ((_Camera.cullingMask & mask) != 0)
                {
                    _Camera.cullingMask &= ~mask;
                }
            }
            base.RestoreLayer();
        }
    }
}