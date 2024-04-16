using LaunchpadReloaded.API.Hud;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Features.Managers;
using LaunchpadReloaded.Features.Translations;
using LaunchpadReloaded.Networking;
using LaunchpadReloaded.Roles;
using UnityEngine;

namespace LaunchpadReloaded.Buttons;

public class ReviveButton : CustomActionButton
{
    public override TranslationStringNames Name => TranslationStringNames.MedicRevive;

    public override float Cooldown => MedicRole.ReviveCooldown.Value;

    public override float EffectDuration => 0;

    public override int MaxUses => (int)MedicRole.MaxRevives.Value;

    public override LoadableAsset<Sprite> Sprite => LaunchpadAssets.ReviveButton;

    public override bool Enabled(RoleBehaviour role) => role is MedicRole;

    public override bool CanUse()
    {
        return RevivalManager.Instance && DeadBodyTarget && CanRevive() && !PlayerControl.LocalPlayer.Data.IsDead && PlayerControl.LocalPlayer.CanMove &&
            !DragManager.Instance.DraggingPlayers.ContainsValue(DeadBodyTarget.ParentId);
    }

    public bool CanRevive()
    {
        if (!MedicRole.OnlyAllowInMedbay.Value)
        {
            return true;
        }

        try
        {
            return ShipStatus.Instance.FastRooms[SystemTypes.MedBay].roomArea
            .OverlapPoint(PlayerControl.LocalPlayer.GetTruePosition());
        }
        catch
        {
            try
            {
                return ShipStatus.Instance.FastRooms[SystemTypes.Laboratory].roomArea
                .OverlapPoint(PlayerControl.LocalPlayer.GetTruePosition());
            }
            catch
            {
                return ShipStatus.Instance.FastRooms[SystemTypes.Medical].roomArea
                .OverlapPoint(PlayerControl.LocalPlayer.GetTruePosition());
            }
        }
    }

    protected override void OnClick()
    {
        PlayerControl.LocalPlayer.RpcRevive(DeadBodyTarget.ParentId);
        DeadBodyTarget = null;
    }
}