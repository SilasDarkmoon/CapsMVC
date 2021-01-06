﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor.Graphing.Util;
using UnityEditor.ShaderGraph.Drawing;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.ShaderGraph
{
    [System.Serializable]
    public sealed class RenderStateOverride
    {
        public bool OverrideStencil;
        public byte StencilRef;
        public byte StencilReadMask;
        public byte StencilWriteMask;
        public UnityEngine.Rendering.CompareFunction StencilComp;
        public UnityEngine.Rendering.StencilOp StencilPass;
        public UnityEngine.Rendering.StencilOp StencilFail;
        public UnityEngine.Rendering.StencilOp StencilZFail;

        public bool OverrideCull;
        public UnityEngine.Rendering.CullMode Cull;

        public bool OverrideZWrite;
        public bool ZWrite;

        public bool OverrideZTest;
        public UnityEngine.Rendering.CompareFunction ZTest;

        public bool OverrideOffset;
        public float OffsetFactor;
        public float OffsetUnits;

        public bool OverrideBlend;
        public bool BlendEnabled;
        public UnityEngine.Rendering.BlendMode SrcFactor;
        public UnityEngine.Rendering.BlendMode DstFactor;
        public bool OverrideBlendAlpha;
        public UnityEngine.Rendering.BlendMode SrcFactorA;
        public UnityEngine.Rendering.BlendMode DstFactorA;

        public bool OverrideBlendOp;
        public UnityEngine.Rendering.BlendOp BlendOp;
        public bool OverrideBlendOpAlpha;
        public UnityEngine.Rendering.BlendOp BlendOpA;

        public bool OverrideColorMask;
        public UnityEngine.Rendering.ColorWriteMask ColorMask;

        public static string[] CompareFunctionStrings = new[]
        {
            null, // 0
            "Never", // 1
            "Less", // 2
            "Equal", // 3
            "LEqual", // 4
            "Greater", // 5
            "NotEqual", // 6
            "GEqual", // 7
            "Always", // 8
        };
        public static string GetCompareFunctionString(UnityEngine.Rendering.CompareFunction comp)
        {
            int index = (int)comp;
            if (index >= 0 && index < CompareFunctionStrings.Length)
            {
                return CompareFunctionStrings[index];
            }
            return null;
        }
        public static string[] StencilOpStrings = new[]
        {
            "Keep",
            "Zero",
            "Replace",
            "IncrSat",
            "DecrSat",
            "Invert",
            "IncrWrap",
            "DecrWrap",
        };
        public static string GetStencilOpString(UnityEngine.Rendering.StencilOp op)
        {
            int index = (int)op;
            if (index >= 0 && index < StencilOpStrings.Length)
            {
                return StencilOpStrings[index];
            }
            return null;
        }
        public static string[] BlendOpStrings = new[]
        {
            "Add",
            "Sub",
            "RevSub",
            "Min",
            "Max",
            "LogicalClear",
            "LogicalSet",
            "LogicalCopy",
            "LogicalCopyInverted",
            "LogicalNoop",
            "LogicalInvert",
            "LogicalAnd",
            "LogicalNand",
            "LogicalOr",
            "LogicalNor",
            "LogicalXor",
            "LogicalEquiv",
            "LogicalAndReverse",
            "LogicalAndInverted",
            "LogicalOrReverse",
            "LogicalOrInverted",
        };
        public static string GetBlendOpString(UnityEngine.Rendering.BlendOp op)
        {
            int index = (int)op;
            if (index >= 0 && index < BlendOpStrings.Length)
            {
                return BlendOpStrings[index];
            }
            return null;
        }
        public static string GetColorMaskString(UnityEngine.Rendering.ColorWriteMask mask)
        {
            string strmask = "";
            if ((mask & UnityEngine.Rendering.ColorWriteMask.Red) != 0)
            {
                strmask += "R";
            }
            if ((mask & UnityEngine.Rendering.ColorWriteMask.Green) != 0)
            {
                strmask += "G";
            }
            if ((mask & UnityEngine.Rendering.ColorWriteMask.Blue) != 0)
            {
                strmask += "B";
            }
            if ((mask & UnityEngine.Rendering.ColorWriteMask.Alpha) != 0)
            {
                strmask += "A";
            }
            if (strmask.Length > 0)
            {
                return strmask;
            }
            else
            {
                return "0";
            }
        }

        public List<string> GetStencilOverrideString()
        {
            List<string> lines = new List<string>();
            if (OverrideStencil)
            {
                lines.Add("Stencil");
                lines.Add("{");
                lines.Add("    Ref " + StencilRef);
                lines.Add("    ReadMask " + StencilReadMask);
                lines.Add("    WriteMask " + StencilWriteMask);
                var comp = GetCompareFunctionString(StencilComp);
                if (comp != null)
                {
                    lines.Add("    Comp " + comp);
                }
                var op = GetStencilOpString(StencilPass);
                if (op != null)
                {
                    lines.Add("    Pass " + op);
                }
                op = GetStencilOpString(StencilFail);
                if (op != null)
                {
                    lines.Add("    Fail " + op);
                }
                op = GetStencilOpString(StencilZFail);
                if (op != null)
                {
                    lines.Add("    ZFail " + op);
                }
                lines.Add("}");
            }
            if (OverrideOffset)
            {
                lines.Add("Offset " + OffsetFactor + ", " + OffsetUnits);
            }
            if (OverrideBlend && BlendEnabled && OverrideBlendOp)
            {
                var op = GetBlendOpString(BlendOp);
                if (op != null)
                {
                    string line = "BlendOp " + op;
                    if (OverrideBlendOpAlpha)
                    {
                        op = GetBlendOpString(BlendOpA);
                        if (op != null)
                        {
                            line += ", " + op;
                        }
                    }
                    lines.Add(line);
                }
            }

            if (lines.Count > 0)
            {
                return lines;
            }
            else
            {
                return null;
            }
        }
        public string GetCullOverrideString()
        {
            if (OverrideCull)
            {
                return "Cull " + Cull.ToString();
            }
            else
            {
                return null;
            }
        }
        public string GetBlendOverrideString()
        {
            if (OverrideBlend)
            {
                if (!BlendEnabled)
                {
                    return "Blend Off";
                }
                else
                {
                    string line = "Blend " + SrcFactor.ToString() + " " + DstFactor.ToString();
                    if (OverrideBlendAlpha)
                    {
                        line += ", " + SrcFactorA.ToString() + " " + DstFactorA.ToString();
                    }
                    return line;
                }
            }
            else
            {
                return null;
            }
        }
        public string GetZTestOverrideString()
        {
            if (OverrideZTest)
            {
                var comp = GetCompareFunctionString(ZTest);
                if (comp != null)
                {
                    return "ZTest " + comp;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        public string GetZWriteOverrideString()
        {
            if (OverrideZWrite)
            {
                return "ZWrite " + (ZWrite ? "On" : "Off");
            }
            else
            {
                return null;
            }
        }
        public string GetColorMaskOverrideString()
        {
            if (OverrideColorMask)
            {
                return "ColorMask " + GetColorMaskString(ColorMask);
            }
            else
            {
                return null;
            }
        }

        public event System.Action OnValueChanged;
        private void FireValueChanged()
        {
            if (OnValueChanged != null)
            {
                OnValueChanged();
            }
        }
        public static List<string> MaskChoices = new List<string>()
        {
            "1, 0x01",
            "2, 0x02",
            "4, 0x04",
            "8, 0x08",
            "16, 0x10",
            "32, 0x20",
            "64, 0x40",
            "128, 0x80",
        };

        public VisualElement CreateSettingsElement()
        {
            PropertySheet ps = new PropertySheet();

            #region Stencil
            PropertyRow StencilRefRow = new PropertyRow(new Label("StencilRef"));
            StencilRefRow.Add(new MaskField(MaskChoices, StencilRef), field =>
            {
            //field.value = StencilRef;
            field.RegisterValueChangedCallback(valuechange =>
                {
                    StencilRef = (byte)valuechange.newValue;
                    FireValueChanged();
                });
            });
            StencilRefRow.SetEnabled(OverrideStencil);

            PropertyRow StencilReadMaskRow = new PropertyRow(new Label("StencilReadMask"));
            StencilReadMaskRow.Add(new MaskField(MaskChoices, StencilReadMask), field =>
            {
            //field.value = StencilReadMask;
            field.RegisterValueChangedCallback(valuechange =>
                {
                    StencilReadMask = (byte)valuechange.newValue;
                    FireValueChanged();
                });
            });
            StencilReadMaskRow.SetEnabled(OverrideStencil);

            PropertyRow StencilWriteMaskRow = new PropertyRow(new Label("StencilWriteMask"));
            StencilWriteMaskRow.Add(new MaskField(MaskChoices, StencilWriteMask), field =>
            {
            //field.value = StencilWriteMask;
            field.RegisterValueChangedCallback(valuechange =>
                {
                    StencilWriteMask = (byte)valuechange.newValue;
                    FireValueChanged();
                });
            });
            StencilWriteMaskRow.SetEnabled(OverrideStencil);

            PropertyRow StencilCompRow = new PropertyRow(new Label("StencilComp"));
            StencilCompRow.Add(new EnumField(StencilComp), field =>
            {
                //field.value = StencilComp;
                field.RegisterValueChangedCallback(valuechange =>
                {
                    StencilComp = (UnityEngine.Rendering.CompareFunction)valuechange.newValue;
                    FireValueChanged();
                });
            });
            StencilCompRow.SetEnabled(OverrideStencil);

            PropertyRow StencilPassRow = new PropertyRow(new Label("StencilPass"));
            StencilPassRow.Add(new EnumField(StencilPass), field =>
            {
                //field.value = StencilPass;
                field.RegisterValueChangedCallback(valuechange =>
                {
                    StencilPass = (UnityEngine.Rendering.StencilOp)valuechange.newValue;
                    FireValueChanged();
                });
            });
            StencilPassRow.SetEnabled(OverrideStencil);

            PropertyRow StencilFailRow = new PropertyRow(new Label("StencilFail"));
            StencilFailRow.Add(new EnumField(StencilFail), field =>
            {
                //field.value = StencilFail;
                field.RegisterValueChangedCallback(valuechange =>
                {
                    StencilFail = (UnityEngine.Rendering.StencilOp)valuechange.newValue;
                    FireValueChanged();
                });
            });
            StencilFailRow.SetEnabled(OverrideStencil);

            PropertyRow StencilZFailRow = new PropertyRow(new Label("StencilZFail"));
            StencilZFailRow.Add(new EnumField(StencilZFail), field =>
            {
                //field.value = StencilZFail;
                field.RegisterValueChangedCallback(valuechange =>
                {
                    StencilZFail = (UnityEngine.Rendering.StencilOp)valuechange.newValue;
                    FireValueChanged();
                });
            });
            StencilZFailRow.SetEnabled(OverrideStencil);

            PropertyRow OverrideStencilRow = new PropertyRow(new Label("OverrideStencil"));
            OverrideStencilRow.Add(new Toggle(), field =>
            {
                field.value = OverrideStencil;
                field.RegisterValueChangedCallback(valuechange =>
                {
                    OverrideStencil = valuechange.newValue;
                    StencilRefRow.SetEnabled(OverrideStencil);
                    StencilReadMaskRow.SetEnabled(OverrideStencil);
                    StencilWriteMaskRow.SetEnabled(OverrideStencil);
                    StencilCompRow.SetEnabled(OverrideStencil);
                    StencilPassRow.SetEnabled(OverrideStencil);
                    StencilFailRow.SetEnabled(OverrideStencil);
                    StencilZFailRow.SetEnabled(OverrideStencil);
                    FireValueChanged();
                });
            });

            ps.Add(OverrideStencilRow);
            ps.Add(StencilRefRow);
            ps.Add(StencilReadMaskRow);
            ps.Add(StencilWriteMaskRow);
            ps.Add(StencilCompRow);
            ps.Add(StencilPassRow);
            ps.Add(StencilFailRow);
            ps.Add(StencilZFailRow);
            #endregion

            #region Cull
            PropertyRow CullRow = new PropertyRow(new Label("Cull"));
            CullRow.Add(new EnumField(Cull), field =>
            {
                //field.value = Cull;
                field.RegisterValueChangedCallback(valuechange =>
                {
                    Cull = (UnityEngine.Rendering.CullMode)valuechange.newValue;
                    FireValueChanged();
                });
            });
            CullRow.SetEnabled(OverrideCull);

            PropertyRow OverrideCullRow = new PropertyRow(new Label("OverrideCull"));
            OverrideCullRow.Add(new Toggle(), field =>
            {
                field.value = OverrideCull;
                field.RegisterValueChangedCallback(valuechange =>
                {
                    OverrideCull = valuechange.newValue;
                    CullRow.SetEnabled(OverrideCull);
                    FireValueChanged();
                });
            });

            ps.Add(OverrideCullRow);
            ps.Add(CullRow);
            #endregion

            #region ZWrite
            PropertyRow ZWriteRow = new PropertyRow(new Label("ZWrite"));
            ZWriteRow.Add(new Toggle(), field =>
            {
                field.value = ZWrite;
                field.RegisterValueChangedCallback(valuechange =>
                {
                    ZWrite = valuechange.newValue;
                    FireValueChanged();
                });
            });
            ZWriteRow.SetEnabled(OverrideZWrite);

            PropertyRow OverrideZWriteRow = new PropertyRow(new Label("OverrideZWrite"));
            OverrideZWriteRow.Add(new Toggle(), field =>
            {
                field.value = OverrideZWrite;
                field.RegisterValueChangedCallback(valuechange =>
                {
                    OverrideZWrite = valuechange.newValue;
                    ZWriteRow.SetEnabled(OverrideZWrite);
                    FireValueChanged();
                });
            });

            ps.Add(OverrideZWriteRow);
            ps.Add(ZWriteRow);
            #endregion

            #region ZTest
            PropertyRow ZTestRow = new PropertyRow(new Label("ZTest"));
            ZTestRow.Add(new EnumField(ZTest), field =>
            {
                //field.value = ZTest;
                field.RegisterValueChangedCallback(valuechange =>
                {
                    ZTest = (UnityEngine.Rendering.CompareFunction)valuechange.newValue;
                    FireValueChanged();
                });
            });
            ZTestRow.SetEnabled(OverrideZTest);

            PropertyRow OverrideZTestRow = new PropertyRow(new Label("OverrideZTest"));
            OverrideZTestRow.Add(new Toggle(), field =>
            {
                field.value = OverrideZTest;
                field.RegisterValueChangedCallback(valuechange =>
                {
                    OverrideZTest = valuechange.newValue;
                    ZTestRow.SetEnabled(OverrideZTest);
                    FireValueChanged();
                });
            });

            ps.Add(OverrideZTestRow);
            ps.Add(ZTestRow);
            #endregion

            #region Offset
            PropertyRow OffsetFactorRow = new PropertyRow(new Label("OffsetFactor"));
            OffsetFactorRow.Add(new UnityEditor.UIElements.FloatField(), field =>
            {
                field.value = OffsetFactor;
                field.RegisterValueChangedCallback(valuechange =>
                {
                    OffsetFactor = valuechange.newValue;
                    FireValueChanged();
                });
            });
            OffsetFactorRow.SetEnabled(OverrideOffset);

            PropertyRow OffsetUnitsRow = new PropertyRow(new Label("OffsetUnits"));
            OffsetUnitsRow.Add(new UnityEditor.UIElements.FloatField(), field =>
            {
                field.value = OffsetUnits;
                field.RegisterValueChangedCallback(valuechange =>
                {
                    OffsetUnits = valuechange.newValue;
                    FireValueChanged();
                });
            });
            OffsetUnitsRow.SetEnabled(OverrideOffset);

            PropertyRow OverrideOffsetRow = new PropertyRow(new Label("OverrideOffset"));
            OverrideOffsetRow.Add(new Toggle(), field =>
            {
                field.value = OverrideOffset;
                field.RegisterValueChangedCallback(valuechange =>
                {
                    OverrideOffset = valuechange.newValue;
                    OffsetFactorRow.SetEnabled(OverrideOffset);
                    OffsetUnitsRow.SetEnabled(OverrideOffset);
                    FireValueChanged();
                });
            });

            ps.Add(OverrideOffsetRow);
            ps.Add(OffsetFactorRow);
            ps.Add(OffsetUnitsRow);
            #endregion

            #region Blend
            PropertyRow SrcFactorRow = new PropertyRow(new Label("SrcFactor"));
            SrcFactorRow.Add(new EnumField(SrcFactor), field =>
            {
                //field.value = SrcFactor;
                field.RegisterValueChangedCallback(valuechange =>
                {
                    SrcFactor = (UnityEngine.Rendering.BlendMode)valuechange.newValue;
                    FireValueChanged();
                });
            });
            SrcFactorRow.SetEnabled(OverrideBlend && BlendEnabled);

            PropertyRow DstFactorRow = new PropertyRow(new Label("DstFactor"));
            DstFactorRow.Add(new EnumField(DstFactor), field =>
            {
                //field.value = DstFactor;
                field.RegisterValueChangedCallback(valuechange =>
                {
                    DstFactor = (UnityEngine.Rendering.BlendMode)valuechange.newValue;
                    FireValueChanged();
                });
            });
            DstFactorRow.SetEnabled(OverrideBlend && BlendEnabled);

            PropertyRow SrcFactorARow = new PropertyRow(new Label("SrcFactorA"));
            SrcFactorARow.Add(new EnumField(SrcFactorA), field =>
            {
                //field.value = SrcFactorA;
                field.RegisterValueChangedCallback(valuechange =>
                {
                    SrcFactorA = (UnityEngine.Rendering.BlendMode)valuechange.newValue;
                    FireValueChanged();
                });
            });
            SrcFactorARow.SetEnabled(OverrideBlend && BlendEnabled && OverrideBlendAlpha);

            PropertyRow DstFactorARow = new PropertyRow(new Label("DstFactorA"));
            DstFactorARow.Add(new EnumField(DstFactorA), field =>
            {
                //field.value = DstFactorA;
                field.RegisterValueChangedCallback(valuechange =>
                {
                    DstFactorA = (UnityEngine.Rendering.BlendMode)valuechange.newValue;
                    FireValueChanged();
                });
            });
            DstFactorARow.SetEnabled(OverrideBlend && BlendEnabled && OverrideBlendAlpha);

            PropertyRow OverrideBlendAlphaRow = new PropertyRow(new Label("OverrideBlendAlpha"));
            OverrideBlendAlphaRow.Add(new Toggle(), field =>
            {
                field.value = OverrideBlendAlpha;
                field.RegisterValueChangedCallback(valuechange =>
                {
                    OverrideBlendAlpha = valuechange.newValue;
                    SrcFactorARow.SetEnabled(OverrideBlend && BlendEnabled && OverrideBlendAlpha);
                    DstFactorARow.SetEnabled(OverrideBlend && BlendEnabled && OverrideBlendAlpha);
                    FireValueChanged();
                });
            });
            OverrideBlendAlphaRow.SetEnabled(OverrideBlend && BlendEnabled);

            #region BlendOp
            PropertyRow BlendOpRow = new PropertyRow(new Label("BlendOp"));
            BlendOpRow.Add(new EnumField(BlendOp), field =>
            {
                //field.value = BlendOp;
                field.RegisterValueChangedCallback(valuechange =>
                {
                    BlendOp = (UnityEngine.Rendering.BlendOp)valuechange.newValue;
                    FireValueChanged();
                });
            });
            BlendOpRow.SetEnabled(OverrideBlend && BlendEnabled && OverrideBlendOp);

            PropertyRow BlendOpARow = new PropertyRow(new Label("BlendOpA"));
            BlendOpARow.Add(new EnumField(BlendOpA), field =>
            {
                //field.value = BlendOpA;
                field.RegisterValueChangedCallback(valuechange =>
                {
                    BlendOpA = (UnityEngine.Rendering.BlendOp)valuechange.newValue;
                    FireValueChanged();
                });
            });
            BlendOpARow.SetEnabled(OverrideBlend && BlendEnabled && OverrideBlendOp && OverrideBlendOpAlpha);

            PropertyRow OverrideBlendOpAlphaRow = new PropertyRow(new Label("OverrideBlendOpAlpha"));
            OverrideBlendOpAlphaRow.Add(new Toggle(), field =>
            {
                field.value = OverrideBlendOpAlpha;
                field.RegisterValueChangedCallback(valuechange =>
                {
                    OverrideBlendOpAlpha = valuechange.newValue;
                    BlendOpARow.SetEnabled(OverrideBlend && BlendEnabled && OverrideBlendOp && OverrideBlendOpAlpha);
                    FireValueChanged();
                });
            });
            OverrideBlendOpAlphaRow.SetEnabled(OverrideBlend && BlendEnabled && OverrideBlendOp);

            PropertyRow OverrideBlendOpRow = new PropertyRow(new Label("OverrideBlendOp"));
            OverrideBlendOpRow.Add(new Toggle(), field =>
            {
                field.value = OverrideBlendOp;
                field.RegisterValueChangedCallback(valuechange =>
                {
                    OverrideBlendOp = valuechange.newValue;
                    BlendOpRow.SetEnabled(OverrideBlend && BlendEnabled && OverrideBlendOp);
                    OverrideBlendOpAlphaRow.SetEnabled(OverrideBlend && BlendEnabled && OverrideBlendOp);
                    BlendOpARow.SetEnabled(OverrideBlend && BlendEnabled && OverrideBlendOp && OverrideBlendOpAlpha);
                    FireValueChanged();
                });
            });
            OverrideBlendOpRow.SetEnabled(OverrideBlend && BlendEnabled);
            #endregion

            PropertyRow BlendEnabledRow = new PropertyRow(new Label("BlendEnabled"));
            BlendEnabledRow.Add(new Toggle(), field =>
            {
                field.value = BlendEnabled;
                field.RegisterValueChangedCallback(valuechange =>
                {
                    BlendEnabled = valuechange.newValue;
                    SrcFactorRow.SetEnabled(OverrideBlend && BlendEnabled);
                    DstFactorRow.SetEnabled(OverrideBlend && BlendEnabled);
                    OverrideBlendAlphaRow.SetEnabled(OverrideBlend && BlendEnabled);
                    SrcFactorARow.SetEnabled(OverrideBlend && BlendEnabled && OverrideBlendAlpha);
                    DstFactorARow.SetEnabled(OverrideBlend && BlendEnabled && OverrideBlendAlpha);

                    OverrideBlendOpRow.SetEnabled(OverrideBlend && BlendEnabled);
                    BlendOpRow.SetEnabled(OverrideBlend && BlendEnabled && OverrideBlendOp);
                    OverrideBlendOpAlphaRow.SetEnabled(OverrideBlend && BlendEnabled && OverrideBlendOp);
                    BlendOpARow.SetEnabled(OverrideBlend && BlendEnabled && OverrideBlendOp && OverrideBlendOpAlpha);
                    FireValueChanged();
                });
            });
            BlendEnabledRow.SetEnabled(OverrideBlend);

            PropertyRow OverrideBlendRow = new PropertyRow(new Label("OverrideBlend"));
            OverrideBlendRow.Add(new Toggle(), field =>
            {
                field.value = OverrideBlend;
                field.RegisterValueChangedCallback(valuechange =>
                {
                    OverrideBlend = valuechange.newValue;
                    BlendEnabledRow.SetEnabled(OverrideBlend);
                    SrcFactorRow.SetEnabled(OverrideBlend && BlendEnabled);
                    DstFactorRow.SetEnabled(OverrideBlend && BlendEnabled);
                    OverrideBlendAlphaRow.SetEnabled(OverrideBlend && BlendEnabled);
                    SrcFactorARow.SetEnabled(OverrideBlend && BlendEnabled && OverrideBlendAlpha);
                    DstFactorARow.SetEnabled(OverrideBlend && BlendEnabled && OverrideBlendAlpha);

                    OverrideBlendOpRow.SetEnabled(OverrideBlend && BlendEnabled);
                    BlendOpRow.SetEnabled(OverrideBlend && BlendEnabled && OverrideBlendOp);
                    OverrideBlendOpAlphaRow.SetEnabled(OverrideBlend && BlendEnabled && OverrideBlendOp);
                    BlendOpARow.SetEnabled(OverrideBlend && BlendEnabled && OverrideBlendOp && OverrideBlendOpAlpha);
                    FireValueChanged();
                });
            });

            ps.Add(OverrideBlendRow);
            ps.Add(BlendEnabledRow);
            ps.Add(SrcFactorRow);
            ps.Add(DstFactorRow);
            ps.Add(OverrideBlendAlphaRow);
            ps.Add(SrcFactorARow);
            ps.Add(DstFactorARow);
            ps.Add(OverrideBlendOpRow);
            ps.Add(BlendOpRow);
            ps.Add(OverrideBlendOpAlphaRow);
            ps.Add(BlendOpARow);
            #endregion

            #region ColorMask
            PropertyRow ColorMaskRow = new PropertyRow(new Label("ColorMask"));
            ColorMaskRow.Add(new EnumFlagsField(ColorMask), field =>
            {
                //field.value = ColorMask;
                field.RegisterValueChangedCallback(valuechange =>
                {
                    ColorMask = (UnityEngine.Rendering.ColorWriteMask)valuechange.newValue;
                    FireValueChanged();
                });
            });
            ColorMaskRow.SetEnabled(OverrideColorMask);

            PropertyRow OverrideColorMaskRow = new PropertyRow(new Label("OverrideColorMask"));
            OverrideColorMaskRow.Add(new Toggle(), field =>
            {
                field.value = OverrideColorMask;
                field.RegisterValueChangedCallback(valuechange =>
                {
                    OverrideColorMask = valuechange.newValue;
                    ColorMaskRow.SetEnabled(OverrideColorMask);
                    FireValueChanged();
                });
            });

            ps.Add(OverrideColorMaskRow);
            ps.Add(ColorMaskRow);
            #endregion

            return ps;
        }
    }
}