using LaunchpadReloaded.Features;
using MiraAPI.Roles;
using System;
using UnityEngine;

namespace LaunchpadReloaded.Roles.Afterlife.Crewmate;

public class TaskmasterRole(IntPtr ptr) : CrewmateGhostRole(ptr), ICustomRole, IAfterlifeRole
{
    public string RoleName => "Taskmaster";
    public string RoleDescription => "Steal tasks from the crew.";
    public string RoleLongDescription => "You are faster than the average ghost.\nSteal tasks from the crew and complete them quicker.";
    public Color RoleColor => LaunchpadPalette.TaskmasterRole;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleOptionsGroup RoleOptionsGroup { get; } = LaunchpadConstants.AfterLifeCrewGroup;

    public override bool IsDead => true;
    public override bool IsAffectedByComms => CommsSabotaged;
}