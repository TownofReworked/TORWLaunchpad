using LaunchpadReloaded.API.Hud;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Features.Managers;
using LaunchpadReloaded.Features.Translations;
using LaunchpadReloaded.Networking;
using LaunchpadReloaded.Roles;
using UnityEngine;

namespace LaunchpadReloaded.Buttons;
public class ScannerButton : CustomActionButton
{
    public override TranslationStringNames Name => TranslationStringNames.TrackerScanner;

    public override float Cooldown => (int)TrackerRole.ScannerCooldown.Value;

    public override float EffectDuration => 0;

    public override int MaxUses => (int)TrackerRole.MaxScanners.Value;

    public override LoadableAsset<Sprite> Sprite => LaunchpadAssets.ScannerButton;

    public override bool Enabled(RoleBehaviour role) => role is TrackerRole;

    public override bool CanUse() => !HackingManager.Instance.AnyPlayerHacked();

    protected override void OnClick()
    {
        PlayerControl.LocalPlayer.RpcCreateScanner(PlayerControl.LocalPlayer.GetTruePosition().x, PlayerControl.LocalPlayer.GetTruePosition().y);
    }
}
