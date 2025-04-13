using MiraAPI.Colors;
using MiraAPI.Utilities;
using UnityEngine;

namespace LaunchpadReloaded.Features.Colors;

[RegisterCustomColors]
public static class LaunchpadColors
{
    public static CustomColor PureBlack { get; } = new("Pure Black", Color.black, Color.black);
    public static CustomColor PureWhite { get; } = new("Pure White", Color.white, Color.white.DarkenColor(.05f));
    public static CustomColor HotPink { get; } = new("Hot Pink", new Color32(238, 0, 108, 255));
    public static CustomColor Blueberry { get; } = new("Blueberry", new Color32(85, 151, 207, 255));
    public static CustomColor Mint { get; } = new("Mint", new Color32(91, 190, 140, 255));
    public static CustomColor Lavender { get; } = new("Lavender", new Color32(181, 176, 255, 255));
    public static CustomColor Iris { get; } = new("Iris", new Color32(90, 79, 207, 255));
    public static CustomColor Viridian { get; } = new("Viridian", new Color32(64, 130, 109, 255));
    public static CustomColor Blurple { get; } = new("Blurple", new Color32(114, 137, 218, 255), new Color32(80, 96, 153, 255));
    public static CustomColor SkyBlue { get; } = new CustomColor("Sky Blue", new Color32(135, 206, 235, 255), new Color32(70, 130, 180, 255));
    public static CustomColor Salmon { get; } = new CustomColor("Salmon", new Color32(250, 128, 114, 255), new Color32(233, 150, 122, 255));
    public static CustomColor Teal { get; } = new CustomColor("Teal", new Color32(0, 128, 128, 255), new Color32(0, 100, 100, 255));
    public static CustomColor Amber { get; } = new CustomColor("Amber", new Color32(255, 191, 0, 255), new Color32(255, 165, 0, 255));
    public static CustomColor Turquoise { get; } = new CustomColor("Turquoise", new Color32(64, 224, 208, 255), new Color32(72, 209, 204, 255));
    public static CustomColor SlateGray { get; } = new CustomColor("Slate Gray", new Color32(112, 128, 144, 255), new Color32(47, 79, 79, 255));
    public static CustomColor Periwinkle { get; } = new CustomColor("Periwinkle", new Color32(204, 204, 255, 255), new Color32(196, 196, 255, 255));
    public static CustomColor LimeGreen { get; } = new CustomColor("Lime Green", new Color32(50, 205, 50, 255), new Color32(34, 139, 34, 255));
    public static CustomColor Indigo { get; } = new CustomColor("Indigo", new Color32(75, 0, 130, 255), new Color32(54, 0, 102, 255));
    public static CustomColor Apricot { get; } = new CustomColor("Apricot", new Color32(251, 206, 177, 255), new Color32(255, 160, 122, 255));
    public static CustomColor Charcoal { get; } = new CustomColor("Charcoal", new Color32(54, 69, 79, 255), new Color32(70, 70, 70, 255));
    public static CustomColor Burgundy { get; } = new CustomColor("Burgundy", new Color32(128, 0, 32, 255), new Color32(100, 0, 20, 255));
    public static CustomColor Mustard { get; } = new CustomColor("Mustard", new Color32(255, 219, 88, 255), new Color32(255, 215, 0, 255));
    public static CustomColor Emerald { get; } = new CustomColor("Emerald", new Color32(80, 200, 120, 255), new Color32(0, 201, 87, 255));
    public static CustomColor Fuchsia { get; } = new CustomColor("Fuchsia", new Color32(255, 119, 255, 255), new Color32(255, 0, 255, 255));
    public static CustomColor NavyBlue { get; } = new CustomColor("Navy Blue", new Color32(0, 0, 128, 255), new Color32(0, 0, 102, 255));
    public static CustomColor RoyalBlue { get; } = new CustomColor("Royal Blue", new Color32(65, 105, 225, 255), new Color32(50, 80, 180, 255));
    public static CustomColor EmeraldGreen { get; } = new CustomColor("Emerald Green", new Color32(80, 200, 120, 255), new Color32(60, 150, 90, 255));
    public static CustomColor SandstoneBeige { get; } = new CustomColor("Sandstone Beige", new Color32(194, 178, 128, 255), new Color32(165, 145, 105, 255));
    public static CustomColor DeepForestBrown { get; } = new CustomColor("Deep Forest Brown", new Color32(92, 64, 51, 255), new Color32(75, 50, 40, 255));
    public static CustomColor CelestialBlue { get; } = new CustomColor("Celestial Blue", new Color32(46, 26, 71, 255), new Color32(36, 20, 56, 255));
    public static CustomColor DragonBloodRed { get; } = new CustomColor("Dragon Blood Red", new Color32(126, 25, 27, 255), new Color32(100, 20, 22, 255));
    public static CustomColor ShadowPurple { get; } = new CustomColor("Shadow Purple", new Color32(75, 0, 130, 255), new Color32(60, 0, 100, 255));
    public static CustomColor ObsidianBlack { get; } = new CustomColor("Obsidian Black", new Color32(11, 11, 11, 255), new Color32(5, 5, 5, 255));
    public static CustomColor VoidGray { get; } = new CustomColor("Void Gray", new Color32(52, 52, 52, 255), new Color32(40, 40, 40, 255));
    public static CustomColor SpectralWhite { get; } = new CustomColor("Spectral White", new Color32(237, 237, 237, 255), new Color32(200, 200, 200, 255));
    public static CustomColor NeonCyan { get; } = new CustomColor("Neon Cyan", new Color32(0, 255, 255, 255), new Color32(0, 200, 200, 255));
    public static CustomColor HotMagenta { get; } = new CustomColor("Hot Magenta", new Color32(255, 0, 127, 255), new Color32(200, 0, 100, 255));
    public static CustomColor PlasmaYellow { get; } = new CustomColor("Plasma Yellow", new Color32(255, 255, 51, 255), new Color32(200, 200, 40, 255));
    public static CustomColor GalaxyPurple { get; } = new CustomColor("Galaxy Purple", new Color32(106, 13, 173, 255), new Color32(85, 10, 140, 255));
    public static CustomColor StarlightSilver { get; } = new CustomColor("Starlight Silver", new Color32(192, 192, 192, 255), new Color32(160, 160, 160, 255));
    public static CustomColor NebulaOrange { get; } = new CustomColor("Nebula Orange", new Color32(255, 117, 24, 255), new Color32(200, 90, 20, 255));
}