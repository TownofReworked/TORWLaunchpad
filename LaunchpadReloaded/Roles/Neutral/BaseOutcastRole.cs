using System;
using MiraAPI.PluginLoading;
using MiraAPI.Roles;
using UnityEngine;

namespace LaunchpadReloaded.Roles.Neutral;

[MiraIgnore]
public abstract class BaseOutcastRole(IntPtr cppPtr) : RoleBehaviour(cppPtr), ICustomRole
{
    public abstract string RoleName { get; }
    public abstract string RoleDescription { get; }
    public abstract string RoleLongDescription { get; }
    public abstract Color RoleColor { get; }
    public abstract CustomRoleConfiguration Configuration { get; }

    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public RoleOptionsGroup RoleOptionsGroup { get; } = new("Outcast Roles", Color.gray);

    public TeamIntroConfiguration? IntroConfiguration { get; } =
        new(Color.gray, "OUTCAST", "You are an Outcast. You do not have a team.");

    public override bool IsDead => false;
}