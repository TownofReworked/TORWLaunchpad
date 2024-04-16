using LaunchpadReloaded.API.Hud;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Features.Managers;
using LaunchpadReloaded.Features.Translations;
using LaunchpadReloaded.Roles;
using LaunchpadReloaded.Utilities;
using UnityEngine;

namespace LaunchpadReloaded.Buttons;
public class TrackButton : CustomActionButton
{
    public override TranslationStringNames Name => TranslationStringNames.TrackerTrack;
    public override float Cooldown => 0;
    public override float EffectDuration => 0;
    public override int MaxUses => 1;
    public override LoadableAsset<Sprite> Sprite => LaunchpadAssets.TrackButton;

    private PlayerControl _currentTarget;

    public override bool Enabled(RoleBehaviour role) => role is TrackerRole;

    protected override void FixedUpdate(PlayerControl playerControl)
    {
        base.FixedUpdate(playerControl);

        if (TrackingManager.Instance.TrackedPlayer && !TrackingManager.Instance.TrackerDisconnected && !PlayerControl.LocalPlayer.Data.IsHacked())
        {
            TrackingManager.Instance.TrackingUpdate();
            return;
        }

        if (_currentTarget)
        {
            _currentTarget.cosmetics.currentBodySprite.BodySprite.material.SetFloat(ShaderID.Outline, 0);
            _currentTarget = null;
        }

        if (UsesLeft <= 0)
        {
            return;
        }

        _currentTarget = playerControl.GetClosestPlayer(true, 1.5f);

        if (!_currentTarget)
        {
            return;
        }

        _currentTarget.cosmetics.currentBodySprite.BodySprite.material.SetFloat(ShaderID.Outline, 1);
        _currentTarget.cosmetics.currentBodySprite.BodySprite.material.SetColor(ShaderID.OutlineColor, Palette.CrewmateBlue);
    }

    public override bool CanUse() => _currentTarget && !HackingManager.Instance.AnyPlayerHacked();

    protected override void OnClick()
    {
        TrackingManager.Instance.TrackedPlayer = _currentTarget;
        _currentTarget = null;
    }
}