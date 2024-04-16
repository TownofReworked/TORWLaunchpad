using LaunchpadReloaded.API.GameOptions;
using LaunchpadReloaded.API.Roles;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Features.Translations;
using Reactor.Utilities.Attributes;
using System;
using UnityEngine;

namespace LaunchpadReloaded.Roles;

[RegisterInIl2Cpp]
public class MedicRole(IntPtr ptr) : CrewmateRole(ptr), ICustomRole
{
    public TranslationStringNames RoleName => TranslationStringNames.MedicRoleName;
    public ushort RoleId => (ushort)LaunchpadRoles.Medic;
    public TranslationStringNames RoleDescription => TranslationStringNames.MedicShortDesc;
    public TranslationStringNames RoleLongDescription => TranslationStringNames.MedicLongDesc;
    public Color RoleColor => LaunchpadPalette.MedicColor;
    public RoleTeamTypes Team => RoleTeamTypes.Crewmate;
    public override bool IsDead => false;
    public bool TargetsBodies => true;
    public LoadableAsset<Sprite> Icon => LaunchpadAssets.ReviveButton;
    public static CustomToggleOption OnlyAllowInMedbay;
    public static CustomToggleOption DragBodies;
    public static CustomNumberOption MaxRevives;
    public static CustomNumberOption ReviveCooldown;
    public static CustomOptionGroup Group;

    public void CreateOptions()
    {
        MaxRevives = new CustomNumberOption(TranslationStringNames.MedicMaxRevives,
            defaultValue: 2,
            1, 9,
            increment: 1,
            suffixType: NumberSuffixes.None,
            role: typeof(MedicRole));

        ReviveCooldown = new CustomNumberOption(TranslationStringNames.MedicReviveCooldown,
            defaultValue: 20,
            1, 50,
            increment: 2,
            suffixType: NumberSuffixes.Seconds,
            role: typeof(MedicRole));

        OnlyAllowInMedbay = new CustomToggleOption(TranslationStringNames.MedicOnlyAllowReviveMedbay,
            defaultValue: false,
            role: typeof(MedicRole));

        DragBodies = new CustomToggleOption(TranslationStringNames.MedicDragBodies,
            defaultValue: false,
            role: typeof(MedicRole));

        Group = new CustomOptionGroup(RoleName,
            numberOpt: [MaxRevives, ReviveCooldown],
            stringOpt: [],
            toggleOpt: [OnlyAllowInMedbay, DragBodies],
            role: typeof(MedicRole));
        Group.SetColor(RoleColor);
    }
}