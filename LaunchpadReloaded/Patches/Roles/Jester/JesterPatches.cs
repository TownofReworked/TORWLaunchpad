using HarmonyLib;
using LaunchpadReloaded.API.Roles;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Roles;
using LaunchpadReloaded.Utilities;
using System.Linq;

namespace LaunchpadReloaded.Patches.Roles.Jester;
[HarmonyPatch]
public static class JesterPatches
{
    /// <summary>
    /// Custom end screen with jester color and text
    /// </summary>
    [HarmonyPostfix, HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
    public static void SetUp(EndGameManager __instance)
    {
        var didWin = TempData.winners.ToArray().Any(h => h.IsYou);

        if (TempData.EndReason != (GameOverReason)GameOverReasons.JesterWins)
        {
            return;
        }

        __instance.WinText.text += didWin ? "\n<size=30%>You Win.</size>" : "\n<size=30%>Jester Wins.</size>";
        __instance.BackgroundBar.material.SetColor(ShaderID.Color, LaunchpadPalette.JesterColor);
        __instance.WinText.color = LaunchpadPalette.JesterColor;
        SoundManager.Instance.PlaySound(__instance.DisconnectStinger, false);
    }
    /// <summary>
    /// Custom ejection text for roles
    /// </summary>
    [HarmonyPostfix, HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
    public static void Begin(ExileController __instance)
    {
        if (!GameManager.Instance.LogicOptions.GetConfirmImpostor())
        {
            return;
        }

        if (!__instance.exiled?.Role || __instance.exiled.Role is not ICustomRole role)
        {
            return;
        }

        if (role.GetCustomEjectionMessage(__instance.exiled) == null)
        {
            return;
        }
        __instance.completeString = role.GetCustomEjectionMessage(__instance.exiled);
    }

    /// <summary>
    /// If Jester gets voted out, end game
    /// </summary>
    [HarmonyPrefix, HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public static bool WrapUp(ExileController __instance)
    {
        if (TutorialManager.InstanceExists)
        {
            return true;
        }

        if (__instance.exiled is not null && __instance.exiled.Role is not null && __instance.exiled.Role is JesterRole)
        {
            GameManager.Instance.RpcEndGame((GameOverReason)GameOverReasons.JesterWins, false);
            return false;
        }
        return true;
    }
}
