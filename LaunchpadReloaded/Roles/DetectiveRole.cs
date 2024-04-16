using LaunchpadReloaded.API.GameOptions;
using LaunchpadReloaded.API.Roles;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Features.Translations;
using Reactor.Utilities.Attributes;
using System;
using UnityEngine;

namespace LaunchpadReloaded.Roles;

[RegisterInIl2Cpp]
public class DetectiveRole(IntPtr ptr) : CrewmateRole(ptr), ICustomRole
{
    public TranslationStringNames RoleName => TranslationStringNames.DetectiveRoleName;
    public ushort RoleId => (ushort)LaunchpadRoles.Detective;
    public TranslationStringNames RoleDescription => TranslationStringNames.DetectiveShortDesc;
    public TranslationStringNames RoleLongDescription => TranslationStringNames.DetectiveLongDesc;
    public Color RoleColor => LaunchpadPalette.DetectiveColor;
    public RoleTeamTypes Team => RoleTeamTypes.Crewmate;
    public override bool IsDead => false;
    public bool TargetsBodies => true;
    public LoadableAsset<Sprite> Icon => LaunchpadAssets.ZoomButton;

    public static CustomToggleOption HideSuspects;
    public static CustomNumberOption FootstepsDuration;
    public static CustomNumberOption InstinctDuration;
    public static CustomNumberOption InstinctUses;
    public static CustomNumberOption InstinctCooldown;
    public static CustomOptionGroup Group;

    public void CreateOptions()
    {
        HideSuspects = new CustomToggleOption(TranslationStringNames.DetectiveHideSuspects, false, role: typeof(DetectiveRole));
        FootstepsDuration = new CustomNumberOption(TranslationStringNames.DetectiveFootstepsDuration, 3, 1, 10, 1, NumberSuffixes.Seconds, role: typeof(DetectiveRole));
        InstinctDuration = new CustomNumberOption(TranslationStringNames.DetectiveInstinctDuration, 10, 3, 76, 3, NumberSuffixes.Seconds, role: typeof(DetectiveRole));
        InstinctUses = new CustomNumberOption(TranslationStringNames.DetectiveInstinctUses, 3, 0, 10, 1, NumberSuffixes.None, zeroInfinity: true, role: typeof(DetectiveRole));
        InstinctCooldown = new CustomNumberOption(TranslationStringNames.DetectiveInstinctCooldown, 15, 0, 45, 1, NumberSuffixes.Seconds, role: typeof(DetectiveRole));

        Group = new CustomOptionGroup(RoleName,
            numberOpt: [FootstepsDuration, InstinctDuration, InstinctUses, InstinctCooldown],
            stringOpt: [],
            toggleOpt: [HideSuspects], role: typeof(DetectiveRole));
        Group.SetColor(RoleColor);
    }
}