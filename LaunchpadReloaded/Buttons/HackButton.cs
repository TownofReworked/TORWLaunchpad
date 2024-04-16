using LaunchpadReloaded.API.Hud;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Features.Managers;
using LaunchpadReloaded.Features.Translations;
using LaunchpadReloaded.Networking;
using LaunchpadReloaded.Roles;
using UnityEngine;

namespace LaunchpadReloaded.Buttons;
public class HackButton : CustomActionButton
{
    public override TranslationStringNames Name => TranslationStringNames.HackerHack;
    public override float Cooldown => (int)HackerRole.HackCooldown.Value;
    public override float EffectDuration => 0;
    public override int MaxUses => (int)HackerRole.HackUses.Value;
    public override LoadableAsset<Sprite> Sprite => LaunchpadAssets.HackButton;
    public override bool Enabled(RoleBehaviour role) => role is HackerRole;
    public override bool CanUse() => !HackingManager.Instance.AnyPlayerHacked();

    protected override void OnClick()
    {
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player.Data.Role.IsImpostor || player.Data.IsDead || player.Data.Disconnected)
            {
                continue;
            }

            PlayerControl.LocalPlayer.RpcHackPlayer(player);
        }

        PlayerControl.LocalPlayer.RawSetColor(15);
    }
}