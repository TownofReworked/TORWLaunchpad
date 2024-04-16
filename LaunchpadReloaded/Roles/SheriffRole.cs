using LaunchpadReloaded.API.GameOptions;
using LaunchpadReloaded.API.Roles;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Features.Translations;
using Reactor.Utilities.Attributes;
using System;
using UnityEngine;

namespace LaunchpadReloaded.Roles;
[RegisterInIl2Cpp]
public class SheriffRole(IntPtr ptr) : CrewmateRole(ptr), ICustomRole
{
    public TranslationStringNames RoleName => TranslationStringNames.SheriffRoleName;

    public ushort RoleId => (ushort)LaunchpadRoles.Sheriff;

    public TranslationStringNames RoleDescription => TranslationStringNames.SheriffShortDesc;

    public TranslationStringNames RoleLongDescription => TranslationStringNames.SheriffLongDesc;

    public Color RoleColor => LaunchpadPalette.SheriffColor;

    public RoleTeamTypes Team => RoleTeamTypes.Crewmate;
    public bool CanKill => true;
    public LoadableAsset<Sprite> Icon => LaunchpadAssets.ShootButton;

    public static CustomNumberOption ShootCooldown;
    public static CustomNumberOption Shots;
    public static CustomToggleOption ShouldCrewmateDie;
    public static CustomOptionGroup Group;

    public void CreateOptions()
    {
        ShootCooldown = new CustomNumberOption(TranslationStringNames.SheriffShootCooldown,
            defaultValue: 45,
            0, 120,
            increment: 5,
            suffixType: NumberSuffixes.Seconds,
            role: typeof(SheriffRole));

        Shots = new CustomNumberOption(TranslationStringNames.SheriffShotsPerGame,
            defaultValue: 3,
            1, 10,
            increment: 1,
            suffixType: NumberSuffixes.None,
            role: typeof(SheriffRole));

        ShouldCrewmateDie = new CustomToggleOption(TranslationStringNames.SheriffCrewmateDies, false, typeof(SheriffRole));

        Group = new CustomOptionGroup(RoleName,
            numberOpt: [ShootCooldown, Shots],
            stringOpt: [],
            toggleOpt: [ShouldCrewmateDie], role: typeof(SheriffRole));
        Group.SetColor(RoleColor);
    }
}
