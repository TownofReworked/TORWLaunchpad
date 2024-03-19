using HarmonyLib;
using LaunchpadReloaded.API.GameModes;
using LaunchpadReloaded.API.Utilities;
using LaunchpadReloaded.Components;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Roles;
using LaunchpadReloaded.Weapons;
using Reactor.Utilities.Extensions;
using System.Linq;
using UnityEngine;

namespace LaunchpadReloaded.Patches;

[HarmonyPatch(typeof(PlayerControl))]
public static class PlayerControlPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(PlayerControl.IsKillTimerEnabled), MethodType.Getter)]
    public static void GetKillTimerEnabledPostfix(PlayerControl __instance, ref bool __result)
    {
        switch (__instance.Data.Role)
        {
            case JanitorRole:
                __result = __result && !DragManager.Instance.IsDragging(__instance.PlayerId);
                break;
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(PlayerControl.Die))]
    public static void OnPlayerDie(PlayerControl __instance)
    {
        CustomGameModeManager.ActiveMode.OnDeath(__instance);
        if (__instance.Data.IsHacked())
            HackingManager.RpcUnHackPlayer(__instance);

        __instance.GetWeaponManager().Destroy();
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(PlayerControl.FixedUpdate))]
    public static void UpdatePatch(PlayerControl __instance)
    {
        var knife = __instance.gameObject.transform.FindChild("BodyForms/Seeker/KnifeHand");
        if (knife is null) return;

        knife.gameObject.SetActive(!__instance.Data.IsDead && __instance.CanMove);
    }
    [HarmonyPrefix]
    [HarmonyPatch("Start")]
    public static void StartPrefix(PlayerControl __instance)
    {
        __instance.gameObject.AddComponent<WeaponManager>();
        __instance.GetWeaponManager().GiveGun<FastRifle>();

        var gradColorComponent = __instance.gameObject.AddComponent<PlayerGradientData>();
        if (__instance.AmOwner)
        {
            gradColorComponent.gradientColor = GradientManager.LocalGradientId;
            GradientManager.RpcSetGradient(__instance, GradientManager.LocalGradientId);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(PlayerControl.SetPlayerMaterialColors))]
    public static void SetPlayerMaterialColorsPostfix(PlayerControl __instance, [HarmonyArgument(0)] Renderer renderer)
    {
        var playerGradient = __instance.GetComponent<PlayerGradientData>();
        if (playerGradient)
        {
            renderer.GetComponent<GradientColorComponent>().SetColor(__instance.Data.DefaultOutfit.ColorId, playerGradient.gradientColor);
        }
    }


    // TODO: Finish custom check color
    //[HarmonyPrefix]
    //[HarmonyPatch(nameof(PlayerControl.CheckColor))]
    public static bool CheckColorPrefix(PlayerControl __instance, [HarmonyArgument(0)] byte bodyColor)
    {
        var allPlayers = GameData.Instance.AllPlayers.ToArray();
        var num = 0;
        while (num++ < 100 &&
               (bodyColor >= Palette.PlayerColors.Length ||
                allPlayers.Any(p => !p.Disconnected &&
                                      p.PlayerId != __instance.PlayerId &&
                                      p.DefaultOutfit.ColorId == bodyColor)))
        {
            bodyColor = (byte)((bodyColor + 1) % Palette.PlayerColors.Length);
        }

        return false;
    }
}