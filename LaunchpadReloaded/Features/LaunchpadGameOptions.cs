using LaunchpadReloaded.API.GameModes;
using LaunchpadReloaded.API.GameOptions;
using LaunchpadReloaded.API.Roles;
using LaunchpadReloaded.Features.Managers;
using LaunchpadReloaded.Features.Translations;
using LaunchpadReloaded.Networking;
using Reactor.Networking.Rpc;

namespace LaunchpadReloaded.Features;

public class LaunchpadGameOptions
{
    public static LaunchpadGameOptions Instance { get; private set; }

    public readonly CustomStringOption GameModes;
    public readonly CustomToggleOption BanCheaters;

    // Voting Types
    public readonly CustomStringOption VotingType;
    public readonly CustomNumberOption MaxVotes;
    public readonly CustomToggleOption AllowVotingForSamePerson;
    public readonly CustomToggleOption AllowConfirmingVotes;
    public readonly CustomToggleOption HideVotingIcons;
    public readonly CustomToggleOption ShowPercentages;
    public readonly CustomOptionGroup VotingGroup;

    // General Options
    public readonly CustomToggleOption OnlyShowRoleColor;
    public readonly CustomToggleOption DisableMeetingTeleport;
    public readonly CustomToggleOption GhostsSeeRoles;
    public readonly CustomOptionGroup GeneralGroup;

    // Battle Royale
    public readonly CustomToggleOption SeekerCharacter;
    public readonly CustomToggleOption ShowKnife;
    public readonly CustomOptionGroup BattleRoyaleGroup;

    // Fun Options
    public readonly CustomToggleOption FriendlyFire;
    public readonly CustomToggleOption UniqueColors;
    public readonly CustomStringOption Character;
    public readonly CustomOptionGroup FunGroup;

    private LaunchpadGameOptions()
    {
        GameModes = new CustomStringOption(TranslationStringNames.Gamemode, 0, ["Default", "Battle Royale"])
        {
            ChangedEvent = i =>
            {
                if (!AmongUsClient.Instance || !AmongUsClient.Instance.AmHost)
                {
                    return;
                }
                CustomGameModeManager.RpcSetGameMode(GameData.Instance, i);
            }
        };

        VotingType = new CustomStringOption(TranslationStringNames.VotingTypes, 0, ["Classic", "Multiple", "Chance", "Combined"]);

        MaxVotes = new CustomNumberOption(TranslationStringNames.MaxVotes, 3, 2, 5, 1, NumberSuffixes.None)
        {
            Hidden = () => !VotingTypesManager.CanVoteMultiple()
        };

        HideVotingIcons = new CustomToggleOption(TranslationStringNames.HideVotingIcons, false)
        {
            Hidden = () => !VotingTypesManager.UseChance() && !ShowPercentages.Value
        };

        ShowPercentages = new CustomToggleOption(TranslationStringNames.ShowPercentages, false)
        {
            Hidden = VotingTypesManager.UseChance
        };

        AllowConfirmingVotes = new CustomToggleOption(TranslationStringNames.AllowConfirmingVotes, false)
        {
            Hidden = VotingTypesManager.CanVoteMultiple
        };

        AllowVotingForSamePerson = new CustomToggleOption(TranslationStringNames.AllowVotingSamePersonAgain, true)
        {
            Hidden = () => !VotingTypesManager.CanVoteMultiple()
        };


        VotingGroup = new CustomOptionGroup(TranslationStringNames.VotingTypes,
            toggleOpt: [AllowVotingForSamePerson, ShowPercentages, AllowConfirmingVotes, HideVotingIcons],
            stringOpt: [],
            numberOpt: [MaxVotes]);

        BanCheaters = new CustomToggleOption(TranslationStringNames.BanCheaters, true)
        {
            ShowInHideNSeek = true
        };

        DisableMeetingTeleport = new CustomToggleOption(TranslationStringNames.DisableMeetingTeleport, false);

        OnlyShowRoleColor = new CustomToggleOption(TranslationStringNames.RevealCrewmateRoles, false);

        GhostsSeeRoles = new CustomToggleOption(TranslationStringNames.GhostsSeeRoles, true);
        GeneralGroup = new CustomOptionGroup(TranslationStringNames.GeneralOptions,
            toggleOpt: [BanCheaters, OnlyShowRoleColor, DisableMeetingTeleport, GhostsSeeRoles],
            stringOpt: [],
            numberOpt: []);

        FriendlyFire = new CustomToggleOption(TranslationStringNames.FriendlyFire, false);

        UniqueColors = new CustomToggleOption(TranslationStringNames.UniqueColors, true)
        {
            ShowInHideNSeek = true,
            ChangedEvent = value =>
            {
                if (!AmongUsClient.Instance.AmHost || !value)
                {
                    return;
                }

                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (GradientManager.TryGetColor(player.PlayerId, out var grad))// && !player.AmOwner)
                    {
                        Rpc<CustomCheckColorRpc>.Instance.Handle(player, new CustomCheckColorRpc.Data((byte)player.Data.DefaultOutfit.ColorId, grad));
                    }
                }
            }
        };

        Character = new CustomStringOption(TranslationStringNames.Character, 0, ["Default", "Horse", "Long"])
        {
            ChangedEvent = i =>
            {
                PlayerBodyTypes bodyType;
                switch (Character?.Options[i])
                {
                    default:
                    case "Default":
                        bodyType = PlayerBodyTypes.Normal;
                        break;
                    case "Horse":
                        bodyType = PlayerBodyTypes.Horse;
                        break;
                    case "Long":
                        bodyType = PlayerBodyTypes.Long;
                        break;
                }

                foreach (var plr in PlayerControl.AllPlayerControls)
                {
                    plr.MyPhysics.SetBodyType(bodyType);
                    if (bodyType == PlayerBodyTypes.Normal)
                    {
                        plr.cosmetics.currentBodySprite.BodySprite.transform.localScale = new(0.5f, 0.5f, 1f);
                    }
                }
            }
        };

        FunGroup = new CustomOptionGroup(TranslationStringNames.FunOptions,
            toggleOpt: [FriendlyFire, UniqueColors],
            stringOpt: [Character],
            numberOpt: []);

        SeekerCharacter = new CustomToggleOption(TranslationStringNames.UseSeekerCharacter, true);
        ShowKnife = new CustomToggleOption(TranslationStringNames.ShowKnife, true)
        {
            Hidden = () => SeekerCharacter.Value == false
        };

        BattleRoyaleGroup = new CustomOptionGroup(TranslationStringNames.BattleRoyaleOptions,
            toggleOpt: [SeekerCharacter, ShowKnife],
            stringOpt: [],
            numberOpt: [])
        {
            Hidden = () => GameModes.IndexValue != (int)LaunchpadGamemodes.BattleRoyale
        };

        GeneralGroup.Hidden = FunGroup.Hidden = VotingType.Hidden = VotingGroup.Hidden = () => !CustomGameModeManager.IsDefault();

        foreach (var role in CustomRoleManager.CustomRoles)
        {
            if (role.Value is ICustomRole customRole)
            {
                customRole.CreateOptions();
            }
        }
    }

    public static void Initialize()
    {
        Instance = new LaunchpadGameOptions();
    }
}