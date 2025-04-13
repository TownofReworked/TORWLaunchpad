using LaunchpadReloaded.Features;
using MiraAPI.Roles;
using System;
using UnityEngine;

namespace LaunchpadReloaded.Roles.Crewmate;

public class MayorRole(IntPtr ptr) : CrewmateRole(ptr), ICustomRole
{
    public string RoleName => "Mayor";
    public string RoleDescription => "You get extra votes.";
    public string RoleLongDescription => "You get extra votes every round.\nUse these votes to eject the Impostor!";
    public Color RoleColor => LaunchpadPalette.MayorColor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public CustomRoleConfiguration Configuration => new(this)
    {
        OptionsScreenshot = LaunchpadAssets.MayorBanner,
    };
}
