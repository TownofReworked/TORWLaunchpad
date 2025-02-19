using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using System;

namespace LaunchpadReloaded.Options.Roles.Impostor;

public class TaskmasterOptions : AbstractOptionGroup
{
    public override string GroupName => "Taskmaster";

    public override Type AdvancedRole => typeof(TaskmasterOptions);

    [ModdedNumberOption("Steal Task Cooldown", 5, 120, 5, MiraNumberSuffixes.Seconds)]
    public float TaskmasterCooldown { get; set; } = 20;
}