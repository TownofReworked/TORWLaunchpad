using MiraAPI.Roles;
using System;
using UnityEngine;

namespace LaunchpadReloaded.Roles.Crewmate;

public class WardenRole(IntPtr ptr) : CrewmateGhostRole(ptr), ICustomRole
{
    public string RoleName => "Warden";
    public string RoleDescription => "Freeze Impostors to protect your Crewmates.";
    public string RoleLongDescription => RoleDescription;
    public Color RoleColor => Palette.Blue;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public CustomRoleConfiguration Configuration => new(this)
    {
        MaxRoleCount = 0,
        DefaultRoleCount = 1,
        HideSettings = false,
    };

    public override bool IsDead => true;
    public override bool IsAffectedByComms => true;
}