using HarmonyLib;
using LaunchpadReloaded.Features;

namespace LaunchpadReloaded.Patches.Generic;
[HarmonyPatch]
public static class MainMenuPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public static void MainMenuFetchSound(MainMenuManager __instance)
    {
        if (__instance == null || LaunchpadConstants.HoverSound != null)
        {
            return;
        }

        LaunchpadConstants.HoverSound = __instance.playLocalButton.HoverSound;
    }
}