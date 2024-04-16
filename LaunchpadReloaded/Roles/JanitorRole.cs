using LaunchpadReloaded.API.GameOptions;
using LaunchpadReloaded.API.Roles;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Features.Translations;
using Reactor.Utilities.Attributes;
using System;
using UnityEngine;

namespace LaunchpadReloaded.Roles;

[RegisterInIl2Cpp]
public class JanitorRole(IntPtr ptr) : ImpostorRole(ptr), ICustomRole
{
    public TranslationStringNames RoleName => TranslationStringNames.JanitorRoleName;
    public ushort RoleId => (ushort)LaunchpadRoles.Janitor;
    public TranslationStringNames RoleDescription => TranslationStringNames.JanitorShortDesc;
    public TranslationStringNames RoleLongDescription => TranslationStringNames.JanitorLongDesc;
    public Color RoleColor => LaunchpadPalette.JanitorColor;
    public RoleTeamTypes Team => RoleTeamTypes.Impostor;
    public override bool IsDead => false;
    public bool TargetsBodies => true;
    public LoadableAsset<Sprite> Icon => LaunchpadAssets.DragButton;

    public static CustomNumberOption HideCooldown;
    public static CustomNumberOption HideUses;
    public static CustomToggleOption CleanInsteadOfHide;
    public static CustomOptionGroup Group;

    public void CreateOptions()
    {
        HideCooldown = new CustomNumberOption(TranslationStringNames.JanitorHideCooldown,
            defaultValue: 5,
            0, 120,
            increment: 5,
            suffixType: NumberSuffixes.Seconds,
            role: typeof(JanitorRole));

        HideUses = new CustomNumberOption(TranslationStringNames.JanitorHideUses,
            defaultValue: 3,
            1, 10,
            increment: 1,
            suffixType: NumberSuffixes.None,
            role: typeof(JanitorRole));

        Group = new CustomOptionGroup(RoleName,
            numberOpt: [HideCooldown, HideUses],
            stringOpt: [],
            toggleOpt: [], role: typeof(JanitorRole));
        Group.SetColor(RoleColor);
    }
}