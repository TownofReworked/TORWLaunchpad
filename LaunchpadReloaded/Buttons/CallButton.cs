using LaunchpadReloaded.API.Hud;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Features.Managers;
using LaunchpadReloaded.Features.Translations;
using LaunchpadReloaded.Roles;
using UnityEngine;

namespace LaunchpadReloaded.Buttons;

public class CallButton : CustomActionButton
{
    public override TranslationStringNames Name => TranslationStringNames.CaptainCall;
    public override float Cooldown => CaptainRole.CaptainMeetingCooldown.Value;
    public override float EffectDuration => 0;
    public override int MaxUses => (int)CaptainRole.CaptainMeetingCount.Value;
    public override LoadableAsset<Sprite> Sprite => LaunchpadAssets.CallButton;

    public override bool Enabled(RoleBehaviour role)
    {
        return role is CaptainRole;
    }

    public override bool CanUse() => !ZoomButton.IsZoom && !HackingManager.Instance.AnyPlayerHacked();

    protected override void OnClick()
    {
        PlayerControl.LocalPlayer.CmdReportDeadBody(null);
    }
}