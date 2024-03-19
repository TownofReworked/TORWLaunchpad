using LaunchpadReloaded.API.Utilities;
using LaunchpadReloaded.Components;
using LaunchpadReloaded.Networking;
using Reactor.Networking.Attributes;
using Reactor.Utilities.Attributes;
using Reactor.Utilities.Extensions;
using System;
using UnityEngine;

namespace LaunchpadReloaded.Features;

[RegisterInIl2Cpp]
public class WeaponManager(IntPtr ptr) : MonoBehaviour(ptr)
{
    private PlayerControl _player;
    private GunComponent _activeGun;

    private void Awake()
    {
        _player = gameObject.GetComponent<PlayerControl>();
        _activeGun = null;
    }

    private void OnDestroy()
    {
        if (_activeGun != null) RemoveGun();
    }

    public void RemoveGun()
    {
        _activeGun.gameObject.Destroy();
        _activeGun = null;
    }

    [MethodRpc((uint)LaunchpadRPC.SyncWeaponPos)]
    public static void RpcSyncPosition(PlayerControl player, float x, float y, float z)
    {
        if (player.GetWeaponManager() is null) return;
        player.GetWeaponManager()._activeGun.TargetPosition = new Vector3(x, y, z);
    }

    public void GiveGun<T>() where T : GunComponent
    {
        if (_activeGun != null) RemoveGun();

        var gun = new GameObject("Gun");
        gun.transform.SetParent(_player.transform);
        gun.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
        gun.layer = LayerMask.NameToLayer("Players");
        gun.transform.localPosition = _player.transform.localPosition;
        gun.transform.position = _player.transform.position;

        GunComponent gunComp = gun.AddComponent<T>();
        gun.name = gunComp.Name;
        gunComp.Owner = _player;

        var rend = gun.AddComponent<SpriteRenderer>();
        rend.sprite = gunComp.Sprite.LoadAsset();

        gun.SetActive(true);

        _activeGun = gunComp;
    }
}