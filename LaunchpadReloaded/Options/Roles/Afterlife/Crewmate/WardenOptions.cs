using LaunchpadReloaded.Roles.Afterlife.Crewmate;
using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using System;

namespace LaunchpadReloaded.Options.Roles.Impostor;

public class WardenOptions : AbstractOptionGroup
{
    public override string GroupName => "Warden";

    public override Type AdvancedRole => typeof(WardenRole);

    [ModdedNumberOption("Freeze Cooldown", 0, 120, 5, MiraNumberSuffixes.Seconds)]
    public float FreezeCooldown { get; set; } = 30;

    [ModdedNumberOption("Freeze Duration", 2, 30, 2)]
    public float FreezeDuration { get; set; } = 8;
}