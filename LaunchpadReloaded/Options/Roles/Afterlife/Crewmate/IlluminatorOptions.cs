using LaunchpadReloaded.Roles.Crewmate;
using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using System;

namespace LaunchpadReloaded.Options.Roles.Impostor;

public class IlluminatorOptions : AbstractOptionGroup
{
    public override string GroupName => "Illuminator";

    public override Type AdvancedRole => typeof(IlluminatorRole);

    [ModdedNumberOption("Lantern Cooldown", 0, 120, 5, MiraNumberSuffixes.Seconds)]
    public float LanternCooldown { get; set; } = 30;

    [ModdedNumberOption("Lantern Duration", 5, 180, 5, MiraNumberSuffixes.Seconds)]
    public float LanternDuration { get; set; } = 45;

    [ModdedNumberOption("Lantern Light Radius", 0.1f, 5f, 0.1f)]
    public float LightRadius { get; set; } = 2.5f;

    [ModdedNumberOption("Max Lanterns", 1, 5, 1)]
    public float MaxLanterns { get; set; } = 2;
}