using LaunchpadReloaded.Components;
using LaunchpadReloaded.Options.Modifiers;
using MiraAPI.GameOptions;
using UnityEngine;

namespace LaunchpadReloaded.Modifiers.Fun;

public sealed class KingModifier : LPModifier
{
    public override string ModifierName => "VIP";
    public override int GetAssignmentChance() => (int)OptionGroupSingleton<GameModifierOptions>.Instance.KingChance;
    public override int GetAmountPerGame() => 1;

    public override void OnActivate()
    {
        PlayerControl.LocalPlayer.RpcSetHat("hat_NewYear2024");
    }
    public override void OnDeactivate()
    {
        PlayerControl.LocalPlayer.RpcSetHat("");
    }
}