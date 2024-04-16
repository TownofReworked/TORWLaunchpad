using LaunchpadReloaded.API.Hud;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Features.Translations;
using LaunchpadReloaded.Roles;
using UnityEngine;

namespace LaunchpadReloaded.Buttons;
public class MapButton : CustomActionButton
{
    public override TranslationStringNames Name => TranslationStringNames.HackerMap;
    public override float Cooldown => (int)HackerRole.MapCooldown.Value;
    public override float EffectDuration => (int)HackerRole.MapDuration.Value;
    public override int MaxUses => 0;
    public override LoadableAsset<Sprite> Sprite => LaunchpadAssets.MapButton;

    private readonly MapOptions _mapOptions = new()
    {
        IncludeDeadBodies = true,
        AllowMovementWhileMapOpen = true,
        ShowLivePlayerPosition = true,
        Mode = MapOptions.Modes.CountOverlay,
    };

    public override bool Enabled(RoleBehaviour role) => role is HackerRole;

    protected override void OnClick()
    {
        HudManager.Instance.ToggleMapVisible(_mapOptions);
    }

    protected override void OnEffectEnd()
    {
        base.OnEffectEnd();
        if (MapBehaviour.Instance.IsOpen)
        {
            HudManager.Instance.ToggleMapVisible(null);
        }
    }
}