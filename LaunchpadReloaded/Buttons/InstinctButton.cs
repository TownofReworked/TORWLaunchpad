using LaunchpadReloaded.API.Hud;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Features.Translations;
using LaunchpadReloaded.Roles;
using UnityEngine;

namespace LaunchpadReloaded.Buttons;

public class InstinctButton : CustomActionButton
{
    public override TranslationStringNames Name => TranslationStringNames.DetectiveInstinct;
    public override float Cooldown => DetectiveRole.InstinctCooldown.Value;
    public override float EffectDuration => DetectiveRole.InstinctDuration.Value;
    public override int MaxUses => (int)DetectiveRole.InstinctUses.Value;
    public override LoadableAsset<Sprite> Sprite => LaunchpadAssets.InstinctButton;

    public override bool Enabled(RoleBehaviour role)
    {
        return role is DetectiveRole;
    }

    protected override void OnEffectEnd()
    {
        LaunchpadPlayer.LocalPlayer.ShowFootsteps = false;
    }
    protected override void OnClick()
    {
        LaunchpadPlayer.LocalPlayer.ShowFootsteps = true;
    }
}