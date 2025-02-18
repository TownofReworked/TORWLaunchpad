using LaunchpadReloaded.Features;
using LaunchpadReloaded.Networking;
using LaunchpadReloaded.Roles.Crewmate;
using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace LaunchpadReloaded.Buttons.Crewmate;

public class PlaceLanternButton : BaseLaunchpadButton
{
    public override string Name => "Place Lantern";
    public override float Cooldown => 3;
    public override float EffectDuration => 0;
    public override int MaxUses => 0;
    public override LoadableAsset<Sprite> Sprite => LaunchpadAssets.CallButton;
    public override bool TimerAffectedByPlayer => true;
    public override bool AffectedByHack => true;

    public override bool Enabled(RoleBehaviour? role)
    {
        return role is IlluminatorRole;
    }

    protected override void OnClick()
    {
        PlayerControl.LocalPlayer.RpcPlaceLantern();
    }
}