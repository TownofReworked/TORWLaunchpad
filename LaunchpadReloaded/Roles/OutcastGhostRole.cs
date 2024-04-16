using LaunchpadReloaded.API.Roles;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Features.Translations;
using Reactor.Utilities.Attributes;
using System;
using System.Text;
using UnityEngine;

namespace LaunchpadReloaded.Roles;
[RegisterInIl2Cpp]
public class OutcastGhostRole(IntPtr ptr) : CrewmateGhostRole(ptr), ICustomRole
{
    public TranslationStringNames RoleName => TranslationStringNames.OutcastGhostName;
    public ushort RoleId => (ushort)LaunchpadRoles.OutcastGhost;
    public TranslationStringNames RoleDescription => (TranslationStringNames)(-1);
    public TranslationStringNames RoleLongDescription => (TranslationStringNames)(-1);
    public Color RoleColor => Color.gray;
    public RoleTeamTypes Team => RoleTeamTypes.Crewmate;
    public bool IsOutcast => true;
    public bool TasksCount => false;
    public bool CanUseVent => false;
    public bool IsGhostRole => true;
    public override bool IsDead => true;
    public override bool IsAffectedByComms => false;

    public override void SpawnTaskHeader(PlayerControl playerControl)
    {
        playerControl.ClearTasks();
        PlayerTask.GetOrCreateTask<ImportantTextTask>(playerControl).Text = $"{Color.gray.ToTextColor()}You are dead, you cannot do tasks.\nThere is no way to win. You have lost.";
    }
    public StringBuilder SetTabText() => null;
    public override bool DidWin(GameOverReason gameOverReason)
    {
        return false;
    }
}