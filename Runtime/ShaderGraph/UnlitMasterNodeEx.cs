using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph.Drawing;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UIElements;
using System.Reflection;
using UnityEditor.Rendering.Universal;

using static System.Linq.Expressions.Expression;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    [Title("Master", "Unlit (Ex)")]
    class UnlitMasterNodeEx : UnlitMasterNode, ISerializationCallbackReceiver
    {
        [MenuItem("Assets/Create/Shader/Unlit Graph (Ex)", false, 208)]
        public static void CreateMaterialGraph()
        {
            GraphUtil.CreateNewGraph(new UnlitMasterNodeEx());
        }

        [SerializeField] public RenderStateOverride RenderState = new RenderStateOverride();

        private static FieldInfo _fi_m_UnlitPass = typeof(UniversalUnlitSubShader).GetField("m_UnlitPass", BindingFlags.NonPublic | BindingFlags.Instance);
        private static Func<UniversalUnlitSubShader, ShaderPass> Func_GetUnlitPass;
        private static Action<UniversalUnlitSubShader, ShaderPass> Func_SetUnlitPass;

        private UniversalUnlitSubShader SubShader
        {
            get
            {
                return (UniversalUnlitSubShader)subShaders.FirstOrDefault();
            }
        }
        public ShaderPass UnlitPass
        {
            get
            {
                if (Func_GetUnlitPass == null)
                {
                    var tar = Parameter(typeof(UniversalUnlitSubShader));
                    Func_GetUnlitPass = Lambda<Func<UniversalUnlitSubShader, ShaderPass>>(Field(tar, _fi_m_UnlitPass), tar).Compile();
                }
                return Func_GetUnlitPass(SubShader);
            }
            set
            {
                if (Func_SetUnlitPass == null)
                {
                    var tar = Parameter(typeof(UniversalUnlitSubShader));
                    var val = Parameter(typeof(ShaderPass));
                    Func_SetUnlitPass = Lambda<Action<UniversalUnlitSubShader, ShaderPass>>(Assign(Field(tar, _fi_m_UnlitPass), val), tar, val).Compile();
                }
                Func_SetUnlitPass(SubShader, value);
            }
        }

        public UnlitMasterNodeEx()
        {
            RenderState.OnValueChanged += FixNodeAfterDeserialization;
            UpdateNodeAfterDeserialization();
        }

        public virtual void FixNodeAfterDeserialization()
        {
            var pass = UnlitPass;
            pass.StencilOverride = RenderState.GetStencilOverrideString();
            pass.CullOverride = RenderState.GetCullOverrideString();
            pass.ZWriteOverride = RenderState.GetZWriteOverrideString();
            pass.ZTestOverride = RenderState.GetZTestOverrideString();
            pass.BlendOverride = RenderState.GetBlendOverrideString();
            pass.ColorMaskOverride = RenderState.GetColorMaskOverrideString();
            UnlitPass = pass;
        }

        public new virtual void UpdateNodeAfterDeserialization()
        {
            base.UpdateNodeAfterDeserialization();
            FixNodeAfterDeserialization();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
            FixNodeAfterDeserialization();
        }

        protected override VisualElement CreateCommonSettingsElement()
        {
            var ele = base.CreateCommonSettingsElement();
            ele.Add(RenderState.CreateSettingsElement());
            return ele;
        }
    }
}
