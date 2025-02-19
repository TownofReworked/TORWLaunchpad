using HarmonyLib;
using LaunchpadReloaded.Components;

namespace LaunchpadReloaded.Patches.Roles;
[HarmonyPatch]
public static class GhostSelectionPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.AssignRoleOnDeath))]
    public static bool AssignRoleOnDeathPatch(PlayerControl player)
    {
        if (player == null || !player.Data.IsDead)
        {
            return false;
        }

        player.RpcSetRole(player.Data.Role.DefaultGhostRole, false);

        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Minigame), nameof(Minigame.ForceClose))]
    public static bool ForceClosePatch(Minigame __instance)
    {
        if (__instance is RoleSelectionMinigame roleSelect)
        {
            roleSelect.Close();
            return false;
        }

        return true;
    }
}
