using Il2CppSystem;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Networking;
using LaunchpadReloaded.Options.Roles.Impostor;
using LaunchpadReloaded.Roles.Afterlife.Crewmate;
using MiraAPI.GameOptions;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using System.Linq;
using UnityEngine;

namespace LaunchpadReloaded.Buttons.Crewmate;

public class TaskButton : BaseLaunchpadButton<PlayerControl>
{
    public override string Name => "Steal Task";
    public override float Cooldown => OptionGroupSingleton<TaskmasterOptions>.Instance.TaskmasterCooldown;
    public override float EffectDuration => 0;
    public override int MaxUses => 0;
    public override LoadableAsset<Sprite> Sprite => LaunchpadAssets.UseButton;
    public override bool TimerAffectedByPlayer => true;
    public override bool AffectedByHack => false;

    public override bool Enabled(RoleBehaviour? role) => role is TaskmasterRole;

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestPlayer(false, 1.1f);
    }

    public override bool IsTargetValid(PlayerControl? target)
    {
        return base.IsTargetValid(target) && target!.myTasks != null && target.myTasks.Count > 0;
    }

    public override void SetOutline(bool active)
    {
        Target?.cosmetics.SetOutline(active, new Nullable<Color>(LaunchpadPalette.WardenRole));
    }

    public override bool CanUse()
    {
        return base.CanUse();
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        PlayerControl.LocalPlayer.RpcStealTask(Target, Target.myTasks.ToArray().First().Id);

        ResetTarget();
    }
}