using LaunchpadReloaded.API.GameOptions;
using LaunchpadReloaded.API.Roles;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Features.Translations;
using Reactor.Utilities.Attributes;
using System;
using UnityEngine;

namespace LaunchpadReloaded.Roles;

[RegisterInIl2Cpp]
public class CaptainRole(IntPtr ptr) : CrewmateRole(ptr), ICustomRole
{
    public TranslationStringNames RoleName => TranslationStringNames.CaptainRoleName;
    public ushort RoleId => (ushort)LaunchpadRoles.Captain;
    public TranslationStringNames RoleDescription => TranslationStringNames.CaptainShortDesc;
    public TranslationStringNames RoleLongDescription => TranslationStringNames.CaptainLongDesc;
    public Color RoleColor => LaunchpadPalette.CaptainColor;
    public RoleTeamTypes Team => RoleTeamTypes.Crewmate;
    public override bool IsDead => false;
    public LoadableAsset<Sprite> Icon => LaunchpadAssets.ZoomButton;

    public static CustomNumberOption CaptainMeetingCooldown;
    public static CustomNumberOption CaptainMeetingCount;
    public static CustomNumberOption ZoomCooldown;
    public static CustomNumberOption ZoomDuration;
    public static CustomNumberOption ZoomDistance;
    public static CustomOptionGroup Group;

    public void CreateOptions()
    {
        CaptainMeetingCooldown = new CustomNumberOption(TranslationStringNames.CaptainMeetingCooldown,
            defaultValue: 45,
            0, 120,
            increment: 5,
            suffixType: NumberSuffixes.Seconds,
            role: typeof(CaptainRole));

        CaptainMeetingCount = new CustomNumberOption(TranslationStringNames.CaptainMeetingCount,
            defaultValue: 3,
            1, 5,
            increment: 1,
            suffixType: NumberSuffixes.None,
            role: typeof(CaptainRole));

        ZoomCooldown = new CustomNumberOption(TranslationStringNames.ZoomCooldown,
            defaultValue: 30,
            5, 60,
            increment: 2,
            suffixType: NumberSuffixes.Seconds,
            role: typeof(CaptainRole));

        ZoomDuration = new CustomNumberOption(TranslationStringNames.ZoomDuration,
            defaultValue: 5,
            5, 25,
            increment: 1,
            suffixType: NumberSuffixes.Seconds,
            role: typeof(CaptainRole));

        ZoomDistance = new CustomNumberOption(TranslationStringNames.ZoomDistance,
            defaultValue: 6,
            4, 15,
            increment: 1,
            suffixType: NumberSuffixes.None,
            role: typeof(CaptainRole));

        Group = new CustomOptionGroup(RoleName,
            numberOpt: [CaptainMeetingCooldown, CaptainMeetingCount, ZoomCooldown, ZoomDuration, ZoomDistance],
            stringOpt: [],
            toggleOpt: [], role: typeof(CaptainRole));
        Group.SetColor(RoleColor);
    }
}