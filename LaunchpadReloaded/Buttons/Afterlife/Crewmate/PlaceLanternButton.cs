using LaunchpadReloaded.Features;
using LaunchpadReloaded.Networking;
using LaunchpadReloaded.Options.Roles.Impostor;
using LaunchpadReloaded.Roles.Crewmate;
using MiraAPI.GameOptions;
using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace LaunchpadReloaded.Buttons.Crewmate;

public class PlaceLanternButton : BaseLaunchpadButton
{
    public override string Name => "Place Lantern";
    public override float Cooldown => OptionGroupSingleton<IlluminatorOptions>.Instance.LanternCooldown;
    public override float EffectDuration => 0;
    public override int MaxUses => 0;
    public override LoadableAsset<Sprite> Sprite => LaunchpadAssets.UseButton;
    public override bool TimerAffectedByPlayer => true;
    public override bool AffectedByHack => true;

    public override bool CanUse()
    {
        var role = PlayerControl.LocalPlayer.Data.Role as IlluminatorRole;
        if (role == null)
        {
            return false;
        }

        return base.CanUse() && role.PlacedLanterns.Count < OptionGroupSingleton<IlluminatorOptions>.Instance.MaxLanterns;
    }
    public override bool Enabled(RoleBehaviour? role)
    {
        return role is IlluminatorRole;
    }

    protected override void OnClick()
    {
        PlayerControl.LocalPlayer.RpcPlaceLantern();
    }
}