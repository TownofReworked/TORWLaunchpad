using Il2CppSystem;
using LaunchpadReloaded.API.Settings;
using MiraAPI.PluginLoading;
using System.Linq;
using LaunchpadReloaded.Components;
using LaunchpadReloaded.Utilities;
using Reactor.Utilities;
using Object = Il2CppSystem.Object;

namespace LaunchpadReloaded.Features;

public class LaunchpadSettings
{
    public static LaunchpadSettings? Instance { get; private set; }

    public readonly CustomSetting Bloom;
    public readonly CustomSetting LockedCamera;
    public readonly CustomSetting UniqueDummies;
#if !ANDROID
    public readonly CustomSetting ButtonLocation;
#endif

    private LaunchpadSettings()
    {
        var configFile = PluginSingleton<LaunchpadReloadedPlugin>.Instance.GetConfigFile();
        var buttonConfig = configFile.Bind("LP Settings", "Button Location", true, "Move buttons to the left side of the screen");
        var bloomConfig = configFile.Bind("LP Settings", "Bloom", true, "Enable bloom effect");
        var lockedCameraConfig = configFile.Bind("LP Settings", "Locked Camera", false, "Lock camera to player");
        var uniqueDummiesConfig = configFile.Bind("LP Settings", "Unique Freeplay Dummies", true, "Give each dummy a unique name");
        
#if !ANDROID
        // TODO: update button location mid-game
        ButtonLocation = new CustomSetting("Buttons On Left", buttonConfig.Value)
        {
            ChangedEvent = val =>
            {
                var plugin = MiraPluginManager.GetPluginByGuid(LaunchpadReloadedPlugin.Id);
                foreach (var button in plugin.GetButtons())
                {
                    button.SetButtonLocation(val ? MiraAPI.Hud.ButtonLocation.BottomLeft : MiraAPI.Hud.ButtonLocation.BottomRight);
                }
            }
        };
#endif
        Bloom = new CustomSetting("Bloom", bloomConfig.Value)
        {
            ChangedEvent = enabled =>
            {
                if (!HudManager.InstanceExists)
                {
                    return;
                }
                var bloom = HudManager.Instance.PlayerCam.GetComponent<Bloom>();
                if (bloom == null)
                {
                    bloom = HudManager.Instance.PlayerCam.gameObject.AddComponent<Bloom>();
                }
                bloom.enabled = enabled;
                bloom.SetBloomByMap();
            }
        };
        
        LockedCamera = new CustomSetting("Locked Camera", lockedCameraConfig.Value);
        UniqueDummies = new CustomSetting("Unique Freeplay Dummies", uniqueDummiesConfig.Value)
        {
            ChangedEvent = val =>
            {
                if (!TutorialManager.InstanceExists || !AccountManager.InstanceExists)
                {
                    return;
                }

                var dummies = UnityEngine.Object.FindObjectsOfType<DummyBehaviour>().ToArray().Reverse().ToList();

                for (var i = 0; i < dummies.Count; i++)
                {
                    var dummy = dummies[i];
                    if (!dummy.myPlayer)
                    {
                        continue;
                    }

                    dummy.myPlayer.SetName(val ? AccountManager.Instance.GetRandomName() :
                        DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.Dummy, Array.Empty<Object>()) + " " + i);
                }
            }
        };
    }

    public static void Initialize()
    {
        Instance = new LaunchpadSettings();
    }
}