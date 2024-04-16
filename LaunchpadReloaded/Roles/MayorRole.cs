using LaunchpadReloaded.API.GameOptions;
using LaunchpadReloaded.API.Roles;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Features.Translations;
using Reactor.Utilities.Attributes;
using System;
using UnityEngine;

namespace LaunchpadReloaded.Roles;
[RegisterInIl2Cpp]
public class MayorRole(IntPtr ptr) : CrewmateRole(ptr), ICustomRole
{
    public TranslationStringNames RoleName => TranslationStringNames.MayorRoleName;
    public ushort RoleId => (ushort)LaunchpadRoles.Mayor;
    public TranslationStringNames RoleDescription => TranslationStringNames.MayorShortDesc;
    public TranslationStringNames RoleLongDescription => TranslationStringNames.MayorLongDesc;
    public Color RoleColor => LaunchpadPalette.MayorColor;
    public RoleTeamTypes Team => RoleTeamTypes.Crewmate;
    public override bool IsDead => false;

    public static CustomNumberOption ExtraVotes;
    public static CustomOptionGroup Group;

    public void CreateOptions()
    {
        ExtraVotes = new CustomNumberOption(TranslationStringNames.MayorExtraVotes, 1, 1, 3, 1, NumberSuffixes.None, role: typeof(MayorRole));

        Group = new CustomOptionGroup(RoleName,
            numberOpt: [ExtraVotes],
            stringOpt: [],
            toggleOpt: [], role: typeof(MayorRole));
        Group.SetColor(RoleColor);
    }

}