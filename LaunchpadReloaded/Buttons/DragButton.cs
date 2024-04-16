using LaunchpadReloaded.API.Hud;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Features.Managers;
using LaunchpadReloaded.Features.Translations;
using LaunchpadReloaded.Networking;
using LaunchpadReloaded.Roles;
using UnityEngine;

namespace LaunchpadReloaded.Buttons;

public class DragButton : CustomActionButton
{
    public override TranslationStringNames Name => TranslationStringNames.BodyDrag;
    public override float Cooldown => 0;
    public override float EffectDuration => 0;
    public override int MaxUses => 0;
    public override LoadableAsset<Sprite> Sprite => LaunchpadAssets.DragButton;

    public override bool Enabled(RoleBehaviour role)
    {
        return role is JanitorRole || (MedicRole.DragBodies.Value && role is MedicRole);
    }

    public override bool CanUse()
    {
        return DeadBodyTarget && DragManager.Instance && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.inVent;
    }

    protected override void FixedUpdate(PlayerControl playerControl)
    {
        if (!DragManager.Instance || !DragManager.Instance.IsDragging(playerControl.PlayerId))
        {
            return;
        }

        // can probably be improved but whatever
        HudManager.Instance.KillButton.SetDisabled();
        HudManager.Instance.ReportButton.SetDisabled();
        HudManager.Instance.UseButton.SetDisabled();
        HudManager.Instance.SabotageButton.SetDisabled();
        HudManager.Instance.ImpostorVentButton.SetDisabled();
        HudManager.Instance.AdminButton.SetDisabled();
        HudManager.Instance.ReportButton.SetDisabled();
    }

    public void SetDrag()
    {
        OverrideName(TranslationStringNames.BodyDrag);
        OverrideSprite(Sprite.LoadAsset());
    }

    public void SetDrop()
    {
        OverrideName(TranslationStringNames.BodyDrop);
        OverrideSprite(LaunchpadAssets.DropButton.LoadAsset());
    }

    public bool CanDrop()
    {
        if (DeadBodyTarget || DragManager.Instance is null)
        {
            return false;
        }

        return !PhysicsHelpers.AnythingBetween(PlayerControl.LocalPlayer.Collider, PlayerControl.LocalPlayer.Collider.bounds.center, DeadBodyTarget.TruePosition, Constants.ShipAndAllObjectsMask, false);
    }

    protected override void OnClick()
    {
        if (DragManager.Instance.IsDragging(PlayerControl.LocalPlayer.PlayerId))
        {
            PlayerControl.LocalPlayer.RpcStopDragging();
        }
        else
        {
            PlayerControl.LocalPlayer.RpcStartDragging(DeadBodyTarget.ParentId);
        }
    }



}