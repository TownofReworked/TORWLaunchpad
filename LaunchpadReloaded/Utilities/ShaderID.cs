using UnityEngine;

namespace LaunchpadReloaded.Utilities;

public static class ShaderID
{
    public static readonly int GradientAngle = Shader.PropertyToID("_GradientAngle");
    public static readonly int GradientBlend = Shader.PropertyToID("_GradientBlend");
    public static readonly int GradientOffset = Shader.PropertyToID("_GradientOffset");
    public static readonly int SecondaryBodyColor = Shader.PropertyToID("_BodyColor2");
    public static readonly int SecondaryShadowColor = Shader.PropertyToID("_BackColor2");
    public static readonly int Outline = Shader.PropertyToID("_Outline");
    public static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");
    public static readonly int AddColor = Shader.PropertyToID("_AddColor");
    public static readonly int Flip = Shader.PropertyToID("_Flip");
    public static readonly int Color = Shader.PropertyToID("_Color");
    public static readonly int VisorColor = Shader.PropertyToID("_VisorColor");
}