using AmongUs.GameOptions;
using HarmonyLib;
using LaunchpadReloaded.API.Roles;

namespace LaunchpadReloaded.Patches.Roles;

[HarmonyPatch(typeof(RoleManager))]
public static class RoleManagerPatches
{
    [HarmonyPrefix, HarmonyPatch("AssignRoleOnDeath")]
    public static bool AssignRoleOnDeath(RoleManager __instance, [HarmonyArgument(0)] PlayerControl plr)
    {
        if (plr == null || !plr.Data.IsDead)
        {
            return false;
        }

        if (plr.Data.Role is ICustomRole role)
        {
            if (role.GhostRole != RoleTypes.CrewmateGhost && role.GhostRole != RoleTypes.ImpostorGhost)
            {
                plr.RpcSetRole(role.GhostRole);
                return false;
            }
        }

        return true;
    }
}