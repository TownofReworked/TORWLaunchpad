using LaunchpadReloaded.Features.Translations;
using UnityEngine;

namespace LaunchpadReloaded.Features.Colors;
public static class LaunchpadColors
{
    public static CustomColor PureBlack => new(Color.black, Color.black, TranslationStringNames.PureBlack);
    public static CustomColor PureWhite => new(Color.white, Color.white, TranslationStringNames.PureWhite);
    public static CustomColor HotPink => new(new Color32(238, 0, 108, 255), TranslationStringNames.HotPink);
    public static CustomColor Blueberry => new(new Color32(85, 151, 207, 255), TranslationStringNames.Blueberry);
    public static CustomColor Mint => new(new Color32(91, 190, 140, 255), TranslationStringNames.Mint);
    public static CustomColor Lavender => new(new Color32(181, 176, 255, 255), TranslationStringNames.Lavender);
    public static CustomColor Iris => new(new Color32(90, 79, 207, 255), TranslationStringNames.Iris);
    public static CustomColor Viridian => new(new Color32(64, 130, 109, 255), TranslationStringNames.Viridian);
    public static CustomColor Blurple => new(new Color32(114, 137, 218, 255), new Color32(80, 96, 153, 255), TranslationStringNames.Blurple);
}