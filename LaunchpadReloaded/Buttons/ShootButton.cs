using LaunchpadReloaded.API.Hud;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Features.Translations;
using LaunchpadReloaded.Roles;
using LaunchpadReloaded.Utilities;
using UnityEngine;

namespace LaunchpadReloaded.Buttons;
public class ShootButton : CustomActionButton
{
    public override TranslationStringNames Name => TranslationStringNames.SheriffShoot;
    public override float Cooldown => SheriffRole.ShootCooldown.Value;
    public override float EffectDuration => 0;
    public override int MaxUses => (int)SheriffRole.Shots.Value;
    public override LoadableAsset<Sprite> Sprite => LaunchpadAssets.ShootButton;

    private PlayerControl _currentTarget;

    public override bool Enabled(RoleBehaviour role) => role is SheriffRole;

    protected override void FixedUpdate(PlayerControl playerControl)
    {
        base.FixedUpdate(playerControl);

        if (_currentTarget)
        {
            _currentTarget.cosmetics.currentBodySprite.BodySprite.material.SetColor(ShaderID.OutlineColor, Color.clear);
            _currentTarget.cosmetics.currentBodySprite.BodySprite.material.SetFloat(ShaderID.Outline, 0);
            _currentTarget = null;
        }

        if (UsesLeft <= 0)
        {
            return;
        }

        _currentTarget = playerControl.GetClosestPlayer(true, GameManager.Instance.LogicOptions.GetKillDistance());

        if (!_currentTarget)
        {
            return;
        }

        _currentTarget.cosmetics.currentBodySprite.BodySprite.material.SetFloat(ShaderID.Outline, 1);
        _currentTarget.cosmetics.currentBodySprite.BodySprite.material.SetColor(ShaderID.OutlineColor, LaunchpadPalette.SheriffColor);
    }

    public override bool CanUse() => _currentTarget;

    protected override void OnClick()
    {
        if (_currentTarget.Data.Role.TeamType == RoleTeamTypes.Impostor || (SheriffRole.ShouldCrewmateDie.Value && _currentTarget.Data.Role.TeamType == RoleTeamTypes.Crewmate))
        {
            PlayerControl.LocalPlayer.CmdCheckMurder(_currentTarget);
        }

        if (_currentTarget.Data.Role.TeamType == RoleTeamTypes.Crewmate && !TutorialManager.InstanceExists)
        {
            PlayerControl.LocalPlayer.CmdCheckMurder(PlayerControl.LocalPlayer);
        }

        _currentTarget = null;
    }
}