using AmongUs.GameOptions;
using BepInEx.Configuration;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Features.Translations;
using LaunchpadReloaded.Utilities;
using System.Text;
using UnityEngine;

namespace LaunchpadReloaded.API.Roles;

public interface ICustomRole
{
    TranslationStringNames RoleName { get; }

    ushort RoleId { get; }

    TranslationStringNames RoleDescription { get; }

    TranslationStringNames RoleLongDescription { get; }

    Color RoleColor { get; }

    RoleTeamTypes Team { get; }

    LoadableAsset<Sprite> Icon => LaunchpadAssets.NoImage;

    ConfigDefinition NumConfigDefinition => new("Roles", $"Num{RoleName}");

    ConfigDefinition ChanceConfigDefinition => new("Roles", $"Chance{RoleName}");

    bool AffectedByLight => Team == RoleTeamTypes.Crewmate;

    bool CanGetKilled => Team == RoleTeamTypes.Crewmate;
    bool IsOutcast => false;
    bool CanKill => Team == RoleTeamTypes.Impostor;
    bool CanUseVent => Team == RoleTeamTypes.Impostor;
    bool TasksCount => Team == RoleTeamTypes.Crewmate;
    bool IsGhostRole => false;
    bool TargetsBodies => false;
    bool CreateCustomTab => true;
    bool HideSettings => IsGhostRole;
    RoleTypes GhostRole => Team == RoleTeamTypes.Crewmate ? RoleTypes.CrewmateGhost : RoleTypes.ImpostorGhost;

    void CreateOptions() { }
    void PlayerControlFixedUpdate(PlayerControl playerControl) { }

    void HudUpdate(HudManager hudManager) { }
    string GetCustomEjectionMessage(GameData.PlayerInfo player)
    {
        return Team == RoleTeamTypes.Impostor ? $"{player.PlayerName} was The {RoleName}" : null;
    }

    StringBuilder SetTabText()
    {
        var taskStringBuilder = Helpers.CreateForRole(this);
        return taskStringBuilder;
    }
}