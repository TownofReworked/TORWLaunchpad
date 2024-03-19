using LaunchpadReloaded.Components;
using LaunchpadReloaded.Utilities;
using Reactor.Utilities.Attributes;
using System;
using UnityEngine;

namespace LaunchpadReloaded.Weapons;

[RegisterInIl2Cpp]
public class FastRifle(IntPtr ptr) : GunComponent(ptr)
{
    public override string Name => "Rifle";
    public override LoadableAsset<Sprite> Sprite => LaunchpadAssets.RifleSprite;
    public override float Range => 4f;
    public override float Cooldown => 0.2f;
    public override int DefaultBullets => 5;
}
