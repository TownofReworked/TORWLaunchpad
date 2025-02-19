using LaunchpadReloaded.Components;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Options.Roles.Impostor;
using LaunchpadReloaded.Roles.Crewmate;
using LaunchpadReloaded.Utilities;
using MiraAPI.GameOptions;
using MiraAPI.Utilities.Assets;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using System.Collections;
using System.Linq;
using UnityEngine;
using Helpers = MiraAPI.Utilities.Helpers;

namespace LaunchpadReloaded.Networking;
public static class GenericRpc
{
    public static IEnumerator LanternRemove(GameObject obj)
    {
        yield return new WaitForSeconds(OptionGroupSingleton<IlluminatorOptions>.Instance.LanternDuration);
        Object.Destroy(obj);
    }

    [MethodRpc((uint)LaunchpadRpc.PlaceLantern)]
    public static void RpcPlaceLantern(this PlayerControl playerControl)
    {
        if (playerControl.Data.Role is not IlluminatorRole illuminator)
        {
            playerControl.KickForCheating();
            return;
        }

        var light = new GameObject("Light");
        light.transform.SetParent(playerControl.transform.parent);
        light.transform.position = playerControl.transform.position;

        var lightComp = light.AddComponent<OgLightSource>();
        lightComp.LightRadius = OptionGroupSingleton<IlluminatorOptions>.Instance.LightRadius;

        illuminator.PlacedLanterns.Add(lightComp);
        Coroutines.Start(LanternRemove(light));
    }

    [MethodRpc((uint)LaunchpadRpc.StealTask)]
    public static void RpcStealTask(this PlayerControl source, PlayerControl victim, uint id)
    {
        var task = victim.myTasks.ToArray().ToList().FirstOrDefault(task => task.Id == id);

        if (task == null)
        {
            return;
        }

        PlayerTask playerTask = Object.Instantiate(task, source.transform);
        playerTask.Id = task.Id;
        playerTask.Index = task.Index;
        playerTask.Owner = source;
        source.myTasks.Add(playerTask);

        victim.RemoveTask(task);

        if (victim.AmOwner)
        {
            Utilities.Helpers.AddMessage($"The {LaunchpadPalette.TaskmasterRole.ToTextColor()}Taskmaster</color> has taken one of your tasks.",
                MiraAssets.Empty.LoadAsset(), null, UnityEngine.Color.white, new Vector3(0f, 0f, -2f), out _);
        }
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