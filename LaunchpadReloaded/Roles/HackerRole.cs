using LaunchpadReloaded.API.GameOptions;
using LaunchpadReloaded.API.Roles;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Features.Translations;
using Reactor.Utilities.Attributes;
using System;
using UnityEngine;

namespace LaunchpadReloaded.Roles;

[RegisterInIl2Cpp]
public class HackerRole(IntPtr ptr) : ImpostorRole(ptr), ICustomRole
{
    public TranslationStringNames RoleName => TranslationStringNames.HackerRoleName;
    public ushort RoleId => (ushort)LaunchpadRoles.Hacker;
    public TranslationStringNames RoleDescription => TranslationStringNames.HackerShortDesc;
    public TranslationStringNames RoleLongDescription => TranslationStringNames.HackerLongDesc;
    public Color RoleColor => LaunchpadPalette.HackerColor;
    public RoleTeamTypes Team => RoleTeamTypes.Impostor;
    public override bool IsDead => false;
    public LoadableAsset<Sprite> Icon => LaunchpadAssets.HackButton;

    public static CustomNumberOption HackCooldown;
    public static CustomNumberOption HackUses;
    public static CustomNumberOption MapCooldown;
    public static CustomNumberOption MapDuration;
    public static CustomOptionGroup Group;

    public void CreateOptions()
    {
        HackCooldown = new CustomNumberOption(TranslationStringNames.HackerHackCooldown,
            defaultValue: 60,
            10, 300,
            increment: 10,
            suffixType: NumberSuffixes.Seconds,
            role: typeof(HackerRole));

        HackUses = new CustomNumberOption(TranslationStringNames.HackerHackUses,
            defaultValue: 2,
            1, 8,
            increment: 1,
            suffixType: NumberSuffixes.None,
            role: typeof(HackerRole));

        MapCooldown = new CustomNumberOption(TranslationStringNames.HackerMapCooldown,
            defaultValue: 10,
            0, 40,
            increment: 3,
            suffixType: NumberSuffixes.Seconds,
            role: typeof(HackerRole));

        MapDuration = new CustomNumberOption(TranslationStringNames.HackerMapDuration,
            defaultValue: 3,
            1, 30,
            increment: 3,
            suffixType: NumberSuffixes.Seconds,
            role: typeof(HackerRole));

        Group = new CustomOptionGroup(RoleName,
            numberOpt: [HackCooldown, HackUses, MapCooldown, MapDuration],
            stringOpt: [],
            toggleOpt: [], role: typeof(HackerRole));
        Group.SetColor(RoleColor);
    }
}