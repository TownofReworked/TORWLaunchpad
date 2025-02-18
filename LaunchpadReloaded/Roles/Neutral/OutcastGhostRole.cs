using MiraAPI.Roles;
using System;
using UnityEngine;

namespace LaunchpadReloaded.Roles.Neutral;

public class OutcastGhostRole(IntPtr ptr) : BaseOutcastRole(ptr)
{
    public override string RoleName => "Outcast Ghost";
    public override string RoleDescription => string.Empty;
    public override string RoleLongDescription => string.Empty;
    public override Color RoleColor => Color.gray;

    public override CustomRoleConfiguration Configuration => new(this)
    {
        HideSettings = true,
        RoleHintType = RoleHintType.TaskHint,
        TasksCountForProgress = false,
        CanUseVent = false,
    };

    public override bool IsDead => true;
    public override bool IsAffectedByComms => false;

    public override void SpawnTaskHeader(PlayerControl playerControl)
    {
        playerControl.ClearTasks();
        PlayerTask.GetOrCreateTask<ImportantTextTask>(playerControl).Text = $"{Color.gray.ToTextColor()}You are dead, you cannot do tasks.\nThere is no way to win. You have lost.";
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return false;
    }

    public bool CanLocalPlayerSeeRole(PlayerControl player)
    {
        return false;
    }
}