using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;

namespace LaunchpadReloaded.Options.Modifiers;

public class GameModifierOptions : AbstractOptionGroup
{
    public override string GroupName => "Game Modifiers";

    [ModdedNumberOption("Player Modifier Limit", 0f, 10, 1, suffixType: MiraNumberSuffixes.None, zeroInfinity: true)]
    public float ModifierLimit { get; set; } = 1f;

    [ModdedNumberOption("Giant Chance", 0f, 100f, 10f, suffixType: MiraNumberSuffixes.Percent)]
    public float GiantChance { get; set; } = 0f;

    [ModdedNumberOption("Smol Chance", 0f, 100f, 10f, suffixType: MiraNumberSuffixes.Percent)]
    public float SmolChance { get; set; } = 0f;

    [ModdedNumberOption("Gravity Field Chance", 0f, 100f, 10f, suffixType: MiraNumberSuffixes.Percent)]
    public float GravityChance { get; set; } = 0f;

    public ModdedNumberOption GravityFieldRadius { get; } = new("Gravity Field Radius", 2f, 0.5f, 10f, 0.5f, MiraNumberSuffixes.None)
    {
        Visible = () => OptionGroupSingleton<GameModifierOptions>.Instance.GravityChance > 0,
    };
}