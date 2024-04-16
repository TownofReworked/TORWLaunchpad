using LaunchpadReloaded.API.Hud;
using LaunchpadReloaded.Components;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Features.Managers;
using LaunchpadReloaded.Features.Translations;
using LaunchpadReloaded.Networking;
using LaunchpadReloaded.Roles;
using UnityEngine;

namespace LaunchpadReloaded.Buttons;

public class HideButton : CustomActionButton
{
    public override TranslationStringNames Name => TranslationStringNames.JanitorHide;
    public override float Cooldown => 5;
    public override float EffectDuration => 0;
    public override int MaxUses => 3;
    public override LoadableAsset<Sprite> Sprite => LaunchpadAssets.HideButton;

    private Vent VentTarget => HudManager.Instance.ImpostorVentButton.currentTarget;

    public override bool CanUse()
    {
        return DeadBodyTarget &&
               DragManager.Instance.IsDragging(PlayerControl.LocalPlayer.PlayerId) &&
               VentTarget && !VentTarget.gameObject.GetComponent<VentBodyComponent>();
    }

    protected override void OnClick()
    {
        PlayerControl.LocalPlayer.RpcStopDragging();
        PlayerControl.LocalPlayer.RpcHideBodyInVent(DeadBodyTarget.ParentId, VentTarget.Id);
    }

    public override bool Enabled(RoleBehaviour role)
    {
        return role is JanitorRole;
    }
}