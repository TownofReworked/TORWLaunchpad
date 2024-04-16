using LaunchpadReloaded.API.Hud;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Features.Translations;
using LaunchpadReloaded.Roles;
using UnityEngine;

namespace LaunchpadReloaded.Buttons;

public class InvestigateButton : CustomActionButton
{
    public override TranslationStringNames Name => TranslationStringNames.DetectiveInvestigate;
    public override float Cooldown => 1;
    public override float EffectDuration => 0;
    public override int MaxUses => 0;
    public override LoadableAsset<Sprite> Sprite => LaunchpadAssets.InvestigateButton;

    public override bool Enabled(RoleBehaviour role)
    {
        return role is DetectiveRole;
    }

    public override bool CanUse()
    {
        return DeadBodyTarget;
    }

    protected override void OnClick()
    {
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(LaunchpadAssets.DetectiveGame.LoadAsset(), HudManager.Instance.transform);
        JournalMinigame minigame = gameObject.GetComponent<JournalMinigame>();
        minigame.Open(LaunchpadPlayer.GetById(DeadBodyTarget.ParentId));
    }
}