using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.ShaderGraph;
using UnityEngine.Rendering;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor.ShaderGraph.Legacy;

#if UNITY_2020_1_OR_NEWER
namespace UnityEditor.Rendering.Universal.ShaderGraph
{
    sealed class UniversalUnlitSubTargetEx : SubTarget<UniversalTarget>
    {
        [SerializeField] internal UniversalUnlitSubTarget _Inner;
        [SerializeField] public RenderStateOverride RenderState;

        public UniversalUnlitSubTargetEx()
        {
            displayName = "Unlit Ex";
            _Inner = new UniversalUnlitSubTarget();
        }

        public override void OnBeforeSerialize()
        {
            base.OnBeforeSerialize();
            if (_Inner == null)
            {
                _Inner = new UniversalUnlitSubTarget();
                if (target != null)
                {
                    _Inner.target = target;
                }
            }
        }
        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
            if (_Inner == null)
            {
                _Inner = new UniversalUnlitSubTarget();
                if (target != null)
                {
                    _Inner.target = target;
                }
            }
        }
        public override void OnAfterDeserialize(string json)
        {
            base.OnAfterDeserialize(json);
            if (_Inner == null)
            {
                _Inner = new UniversalUnlitSubTarget();
                if (target != null)
                {
                    _Inner.target = target;
                }
            }
        }
        public override void OnAfterMultiDeserialize(string json)
        {
            base.OnAfterMultiDeserialize(json);
            if (_Inner == null)
            {
                _Inner = new UniversalUnlitSubTarget();
                if (target != null)
                {
                    _Inner.target = target;
                }
            }
        }

        public override void GetActiveBlocks(ref TargetActiveBlockContext context)
        {
            if (_Inner.target == null)
            {
                _Inner.target = target;
            }
            _Inner.GetActiveBlocks(ref context);
        }
        public override void GetFields(ref TargetFieldContext context)
        {
            if (_Inner.target == null)
            {
                _Inner.target = target;
            }
            _Inner.GetFields(ref context);
        }
        public override void GetPropertiesGUI(ref TargetPropertyGUIContext context, Action onChange, Action<string> registerUndo)
        {
            if (_Inner.target == null)
            {
                _Inner.target = target;
            }
            _Inner.GetPropertiesGUI(ref context, onChange, registerUndo);

            context.Add(RenderState.CreateSettingsElement());
        }
        public override bool IsActive()
        {
            if (_Inner.target == null)
            {
                _Inner.target = target;
            }
            return _Inner.IsActive();
        }
        public override void Setup(ref TargetSetupContext context)
        {
            if (_Inner.target == null)
            {
                _Inner.target = target;
            }
            _Inner.Setup(ref context);

            for (int i = 0; i < context.subShaders.Count; ++i)
            {
                var sub = context.subShaders[i];
                var passes = new PassCollection();
                int passindex = 0;
                foreach (var pass in sub.passes)
                {
                    if (passindex == 0)
                    {
                        var newpass = pass.descriptor;
                        newpass.renderStates = RenderState.ModifyRenderStateCollection(newpass.renderStates);
                        passes.Add(newpass, pass.fieldConditions);
                    }
                    else
                    {
                        passes.Add(pass.descriptor, pass.fieldConditions);
                    }
                    ++passindex;
                }
                sub.passes = passes;
                context.subShaders[i] = sub;
            }
        }

        public override void CollectShaderProperties(PropertyCollector collector, GenerationMode generationMode)
        {
            if (_Inner.target == null)
            {
                _Inner.target = target;
            }
            _Inner.CollectShaderProperties(collector, generationMode);
        }
        public override void ProcessPreviewMaterial(Material material)
        {
            if (_Inner.target == null)
            {
                _Inner.target = target;
            }
            _Inner.ProcessPreviewMaterial(material);
        }
        public override object saveContext
        {
            get
            {
                if (_Inner.target == null)
                {
                    _Inner.target = target;
                }
                return _Inner.saveContext;
            }
        }
    }
}
#endif