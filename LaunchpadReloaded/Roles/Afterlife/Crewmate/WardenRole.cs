using LaunchpadReloaded.Features;
using MiraAPI.Roles;
using System;
using UnityEngine;

namespace LaunchpadReloaded.Roles.Afterlife.Crewmate;

public class WardenRole(IntPtr ptr) : CrewmateGhostRole(ptr), ICustomRole, IAfterlifeRole
{
    public string RoleName => "Warden";
    public string RoleDescription => "Freeze Impostors to protect your Crewmates.";
    public string RoleLongDescription => RoleDescription;
    public Color RoleColor => LaunchpadPalette.WardenRole;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public CustomRoleConfiguration Configuration => new(this)
    {
        MaxRoleCount = 1,
        DefaultRoleCount = 1,
        HideSettings = false,
    };

    public RoleOptionsGroup RoleOptionsGroup { get; } = LaunchpadConstants.AfterLifeCrewGroup;

    public override bool IsDead => true;
    public override bool IsAffectedByComms => true;
}