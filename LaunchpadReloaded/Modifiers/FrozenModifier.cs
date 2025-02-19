using LaunchpadReloaded.Features;
using LaunchpadReloaded.Options.Roles.Impostor;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers.Types;
using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace LaunchpadReloaded.Modifiers;

public class FrozenModifier : TimedModifier
{
    public override string ModifierName => "Frozen";
    public override bool HideOnUi => false;
    public override bool RemoveOnComplete => true;
    public override bool AutoStart => true;
    public override float Duration => OptionGroupSingleton<WardenOptions>.Instance.FreezeDuration;

    public override string GetHudString()
    {
        return base.GetHudString() + $"\n<size=65%>You were frozen by the {LaunchpadPalette.WardenRole.ToTextColor()}Warden.</color></size>";
    }
    public override void OnActivate()
    {
        if (Player?.AmOwner == true)
        {
            Utilities.Helpers.AddMessage($"You have been frozen by the {LaunchpadPalette.WardenRole.ToTextColor()}Warden!</color>",
                 MiraAssets.Empty.LoadAsset(), null, Color.white, new Vector3(0f, 1.4f, -2f), out _);

            Player.moveable = false;
        }
    }

    public override void OnDeactivate()
    {
        if (Player?.AmOwner == true)
        {
            Player.moveable = true;
        }
    }

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }

    public override void OnTimerComplete()
    {
    }
}
