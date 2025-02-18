using LaunchpadReloaded.Components;
using LaunchpadReloaded.Roles.Crewmate;
using LaunchpadReloaded.Utilities;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using UnityEngine;
using Helpers = MiraAPI.Utilities.Helpers;

namespace LaunchpadReloaded.Networking;
public static class GenericRpc
{
    [MethodRpc((uint)LaunchpadRpc.PlaceLantern)]
    public static void RpcPlaceLantern(this PlayerControl playerControl)
    {
        if (playerControl.Data.Role is not IlluminatorRole)
        {
            playerControl.KickForCheating();
            return;
        }
        var light = new GameObject("Light");
        light.transform.SetParent(playerControl.transform.parent);
        light.transform.position = playerControl.transform.position;

        var lightComp = light.AddComponent<OgLightSource>();
        lightComp.LightRadius = 2f;
    }

    [MethodRpc((uint)LaunchpadRpc.Revive)]
    public static void RpcRevive(this PlayerControl playerControl, byte bodyId)
    {
        if (playerControl.Data.Role is not MedicRole)
        {
            playerControl.KickForCheating();
            return;
        }

        var body = Helpers.GetBodyById(bodyId);
        if (body != null)
        {
            body.Revive(playerControl);
        }
        else
        {
            Logger<LaunchpadReloadedPlugin>.Warning($"Body for id {bodyId} not found");
        }
    }
}