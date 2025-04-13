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

    [ModdedNumberOption("Child Chance", 0f, 100f, 10f, suffixType: MiraNumberSuffixes.Percent)]
    public float ChildChance { get; set; } = 0f;

    [ModdedNumberOption("VIP Chance", 0f, 100f, 10f, suffixType: MiraNumberSuffixes.Percent)]
    public float KingChance { get; set; } = 0f;
    
    [ModdedNumberOption("Flash Chance", 0f, 100f, 10f, suffixType: MiraNumberSuffixes.Percent)]
    public float FlashChance { get; set; } = 0f;
}