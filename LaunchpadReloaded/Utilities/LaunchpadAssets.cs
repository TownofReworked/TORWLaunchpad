using LaunchpadReloaded.API.Utilities;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LaunchpadReloaded.Utilities;

public static class LaunchpadAssets
{
    public static AssetBundle Bundle = AssetBundleManager.Load("assets");
    public static LoadableAsset<Sprite> NoImage = new("", false);

    // Materials
    public static LoadableAsset<Material> GradientMaterial = new("GradientPlayerMaterial");

    // Sprites
    public static LoadableAsset<Sprite> BlankButton = new("BlankButton");
    public static LoadableAsset<Sprite> CallButton = new("CallMeeting.png");
    public static LoadableAsset<Sprite> DragButton = new("Drag.png");
    public static LoadableAsset<Sprite> DropButton = new("Drop.png");
    public static LoadableAsset<Sprite> ZoomButton = new("Zoom.png");
    public static LoadableAsset<Sprite> ReviveButton = new("Revive.png");
    public static LoadableAsset<Sprite> HideButton = new("Clean.png");
    public static LoadableAsset<Sprite> HackButton = new("Hack.png", false);
    public static LoadableAsset<Sprite> MapButton = new("Map.png", false);
    public static LoadableAsset<Sprite> ScannerButton = new("Place_Scanner.png", false);
    public static LoadableAsset<Sprite> TrackButton = new("Track.png", false);

    // Object Sprites
    public static LoadableAsset<Sprite> ScannerSprite = new("Scanner.png", false);
    public static LoadableAsset<Sprite> NodeSprite = new("Node.png", false);
    public static LoadableAsset<Sprite> KnifeHandSprite = new("KnifeHand.png", false);
    public static LoadableAsset<Sprite> RifleSprite = new("Rifle.png", false);
    public static LoadableAsset<Sprite> BulletSprite = new("Bullet.png", false);

    // Sounds
    public static LoadableAsset<AudioClip> BeepSound = new("Beep.wav");
    public static LoadableAsset<AudioClip> PingSound = new("Ping.mp3");
}

public class LoadableAsset<T>(string name, bool useBundle = true)
    where T : Object
{
    private const string ResourcesFolder = "LaunchpadReloaded.Resources.";
    public string Name { get; } = name;
    public bool UseBundle { get; } = useBundle;

    private T _loadedAsset;

    public T LoadAsset()
    {
        if (_loadedAsset != null)
        {
            return _loadedAsset;
        }

        if (UseBundle)
        {
            return _loadedAsset = LaunchpadAssets.Bundle.LoadAsset<T>(Name);
        }

        if (typeof(T) == typeof(Sprite))
        {
            return _loadedAsset = SpriteTools.LoadSpriteFromPath(ResourcesFolder + Name) as T;
        }

        throw new Exception($"INVALID ASSET: {Name}");
    }
}