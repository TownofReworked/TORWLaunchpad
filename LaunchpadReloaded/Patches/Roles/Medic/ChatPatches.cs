using HarmonyLib;
using LaunchpadReloaded.Components;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Modifiers;
using MiraAPI.Utilities;
using UnityEngine;

namespace LaunchpadReloaded.Patches.Roles.Medic;

/// <summary>
/// Disable chat if revived
/// </summary>
[HarmonyPatch(typeof(ChatController))]
public static class ChatPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(ChatController.Toggle))]
    public static bool TogglePatch(ChatController __instance)
    {
        if (Minigame.Instance != null && Minigame.Instance.TryCast<RoleSelectionMinigame>())
        {
            return false;
        }

        return true;
    }

    //[HarmonyPostfix]
    [HarmonyPatch(nameof(ChatController.Update))]
    public static void UpdatePatch(ChatController __instance)
    {
        if (PlayerControl.LocalPlayer?.HasModifier<RevivedModifier>() == true)
        {
            __instance.sendRateMessageText.gameObject.SetActive(true);
            __instance.sendRateMessageText.text = "You have been revived. You can no longer speak.";
            __instance.sendRateMessageText.color = LaunchpadPalette.MedicColor;
            __instance.quickChatButton.gameObject.SetActive(false);
            __instance.freeChatField.textArea.gameObject.SetActive(false);
            __instance.openKeyboardButton.gameObject.SetActive(false);
        }
        else
        {
            __instance.sendRateMessageText.color = Color.red;
            __instance.quickChatButton.gameObject.SetActive(true);
            __instance.freeChatField.textArea.gameObject.SetActive(true);
            __instance.openKeyboardButton.gameObject.SetActive(true);
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(ChatController.SendChat))]
    public static bool SendChatPatch()
    {
        return !PlayerControl.LocalPlayer.HasModifier<RevivedModifier>();
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(ChatController.AddChat))]
    public static bool AddChatPatch([HarmonyArgument(0)] PlayerControl player)
    {
        return !player.HasModifier<RevivedModifier>();
    }
}