using AmongUs.GameOptions;
using HarmonyLib;
using LaunchpadReloaded.Modifiers.Game;
using LaunchpadReloaded.Options.Modifiers;
using MiraAPI.GameOptions;
using MiraAPI.Utilities;

namespace LaunchpadReloaded.Patches.Modifiers;

[HarmonyPatch(typeof(AirshipStatus), nameof(AirshipStatus.CalculateLightRadius))]
[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
public static class TorchPatches
{
    public static void Postfix(ShipStatus __instance, NetworkedPlayerInfo player, ref float __result)
    {
        if (!player.Object.AmOwner || player.Role.IsImpostor || !player.Object.HasModifier<TorchModifier>())
        {
            return;
        }

        __result = __instance.MaxLightRadius * GameOptionsManager.Instance.CurrentGameOptions.GetFloat(FloatOptionNames.CrewLightMod);
        if (!OptionGroupSingleton<GameModifierOptions>.Instance.TorchUseFlashlight)
        {
            return;
        }

        var flashSize = OptionGroupSingleton<GameModifierOptions>.Instance.TorchFlashlightSize;
        if (__instance.Systems.TryGetValue(SystemTypes.Electrical, out var system) &&
            system.TryCast<SwitchSystem>() is { } switchSystem &&
            switchSystem.Value < SwitchSystem.MaxValue)
        {
            player.Object.lightSource.SetupLightingForGameplay(true, flashSize.Value, player.Object.TargetFlashlight.transform);
        }
        else
        {
            player.Object.lightSource.SetupLightingForGameplay(false, flashSize.Value, player.Object.TargetFlashlight.transform);
        }
    }
}