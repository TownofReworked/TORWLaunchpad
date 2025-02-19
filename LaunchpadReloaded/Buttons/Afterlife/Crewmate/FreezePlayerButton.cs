using Il2CppSystem;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Modifiers;
using LaunchpadReloaded.Options.Roles.Impostor;
using LaunchpadReloaded.Roles.Afterlife.Crewmate;
using MiraAPI.GameOptions;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace LaunchpadReloaded.Buttons.Crewmate;


public class FreezePlayerButton : BaseLaunchpadButton<PlayerControl>
{
    public override string Name => "Freeze";
    public override float Cooldown => OptionGroupSingleton<WardenOptions>.Instance.FreezeCooldown;
    public override float EffectDuration => OptionGroupSingleton<WardenOptions>.Instance.FreezeDuration;
    public override int MaxUses => 0;
    public override LoadableAsset<Sprite> Sprite => LaunchpadAssets.UseButton;
    public override bool TimerAffectedByPlayer => true;
    public override bool AffectedByHack => false;

    public override bool Enabled(RoleBehaviour? role) => role is WardenRole;

    private PlayerControl? _frozenPlayer;

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestPlayer(true, 1.1f);
    }

    public override bool IsTargetValid(PlayerControl? target)
    {
        return base.IsTargetValid(target) && !target.HasModifier<FrozenModifier>();
    }

    public override void SetOutline(bool active)
    {
        Target?.cosmetics.SetOutline(active, new Nullable<Color>(LaunchpadPalette.WardenRole));
    }

    public override bool CanUse()
    {
        return base.CanUse() && _frozenPlayer == null;
    }

    public override void OnEffectEnd()
    {
        if (_frozenPlayer is null || _frozenPlayer.Data.IsDead || !_frozenPlayer.HasModifier<FrozenModifier>())
        {
            _frozenPlayer = null;
            return;
        }

        _frozenPlayer = null;
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        _frozenPlayer = Target;
        _frozenPlayer.RpcAddModifier<FrozenModifier>();

        ResetTarget();
    }
}