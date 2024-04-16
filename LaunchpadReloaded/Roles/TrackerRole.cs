using LaunchpadReloaded.API.GameOptions;
using LaunchpadReloaded.API.Roles;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Features.Managers;
using LaunchpadReloaded.Features.Translations;
using LaunchpadReloaded.Utilities;
using Reactor.Utilities.Attributes;
using System;
using System.Text;
using UnityEngine;

namespace LaunchpadReloaded.Roles;
[RegisterInIl2Cpp]
public class TrackerRole(IntPtr ptr) : CrewmateRole(ptr), ICustomRole
{
    public TranslationStringNames RoleName => TranslationStringNames.TrackerRoleName;
    public ushort RoleId => (ushort)LaunchpadRoles.Tracker;
    public TranslationStringNames RoleDescription => TranslationStringNames.TrackerShortDesc;
    public TranslationStringNames RoleLongDescription => TranslationStringNames.TrackerLongDesc;
    public Color RoleColor => LaunchpadPalette.TrackerColor;
    public RoleTeamTypes Team => RoleTeamTypes.Crewmate;
    public LoadableAsset<Sprite> Icon => LaunchpadAssets.TrackButton;
    public StringBuilder SetTabText()
    {
        var taskStringBuilder = Helpers.CreateForRole(this);

        if (TrackingManager.Instance.TrackedPlayer)
        {
            if (TrackingManager.Instance.TrackerDisconnected)
            {
                taskStringBuilder.AppendLine(TranslationController.Instance.GetString((StringNames)TranslationStringNames.TrackerDisconnectedText));
            }
            else
            {
                taskStringBuilder.AppendLine(TranslationController.Instance.GetString((StringNames)TranslationStringNames.TrackingPlayerText, new Il2CppSystem.Object[]
                {
                    TrackingManager.Instance.TrackedPlayer.Data.Color.ToTextColor() + TrackingManager.Instance.TrackedPlayer.Data.PlayerName
                }));

                taskStringBuilder.AppendLine(TranslationController.Instance.GetString((StringNames)TranslationStringNames.NextPingText, new Il2CppSystem.Object[]
                {
                    (int)TrackingManager.Instance.Timer
                }));
            }
        }

        if (ScannerManager.Instance.scanners.Count > 0)
        {
            taskStringBuilder.AppendLine(TranslationController.Instance.GetString((StringNames)TranslationStringNames.CreatedScannersText));
        }

        foreach (var component in ScannerManager.Instance.scanners)
        {
            if (component.room)
            {
                taskStringBuilder.AppendLine(TranslationController.Instance.GetString((StringNames)TranslationStringNames.ScannerText, new Il2CppSystem.Object[]
                {
                    component.id, component.room.RoomId.ToString()
                }));
            }
        }
        return taskStringBuilder;
    }

    public static CustomNumberOption PingTimer;
    public static CustomNumberOption ScannerCooldown;
    public static CustomNumberOption MaxScanners;
    public static CustomOptionGroup Group;
    public void CreateOptions()
    {
        PingTimer = new CustomNumberOption(TranslationStringNames.TrackerPingTimer,
            defaultValue: 7,
            3, 30,
            increment: 1,
            suffixType: NumberSuffixes.Seconds,
            role: typeof(TrackerRole));

        MaxScanners = new CustomNumberOption(TranslationStringNames.TrackerMaxScanners,
            defaultValue: 3,
            1, 15,
            increment: 1,
            suffixType: NumberSuffixes.None,
            role: typeof(TrackerRole));

        ScannerCooldown = new CustomNumberOption(TranslationStringNames.TrackerScannerPlaceCooldown,
            defaultValue: 5,
            1, 20,
            increment: 2,
            suffixType: NumberSuffixes.Seconds,
            role: typeof(TrackerRole));

        Group = new CustomOptionGroup(RoleName,
            numberOpt: [PingTimer, MaxScanners, ScannerCooldown],
            stringOpt: [],
            toggleOpt: [], role: typeof(TrackerRole));
        Group.SetColor(RoleColor);
    }
}
