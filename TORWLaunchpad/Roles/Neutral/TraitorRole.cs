using System.Linq;
using AmongUs.GameOptions;
using HarmonyLib;
using LaunchpadReloaded.Components;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.GameOver;
using LaunchpadReloaded.Options.Roles.Neutral;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.GameEnd;
using MiraAPI.GameOptions;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace LaunchpadReloaded.Roles.Neutral;

public class TraitorRole(System.IntPtr ptr) : RoleBehaviour(ptr), INeutralRole
{
    public string RoleName => "Traitor";
    public string RoleLongDescription => "As the <color=#870707>Traitor</color>, your job is to betray your fellow <color=#ff0000>Impostor(s)</color>.\nBy doing so, could give you the chance to win alone!\n\nYou can see the <color=#ff0000>Impostor(s)</color>, but they can't see you!";
    public string RoleDescription => "Betray your fellow <color=#ff0000>Impostor</color> friends and win!";
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public Color RoleColor => LaunchpadPalette.TraitorColor;
    public override bool IsDead => false;

    public CustomRoleConfiguration Configuration => new CustomRoleConfiguration(this)
    {
        Icon = LaunchpadAssets.TraitorIcon,
        TasksCountForProgress = false,
        UseVanillaKillButton = false,
        CanUseVent = false,
        CanUseSabotage = false,
    };

    private static readonly PlayerTag TargetTag = new()
    {
        Name = "ExeTarget",
        Text = "Target",
        Color = LaunchpadPalette.TraitorColor.LightenColor(),
        IsLocallyVisible = _ => true,
    };

    private PlayerControl? target;
    private bool targetExiled;
    
    [RegisterEvent]
    public static void PlayerDeathEvent(PlayerDeathEvent @event)
    {
        var exes = CustomRoleUtils.GetActiveRolesOfType<TraitorRole>()
            .Where(x => x.target && x.target == @event.Player).ToArray();

        foreach (var exe in exes)
        {
            switch (@event.DeathReason)
            {
                case DeathReason.Exile:
                    exe.targetExiled = true;
                    break;
                default:
                    exe.OnTargetDeath();
                    break;
            }
        }
    }

    [RegisterEvent]
    public static void RoundStartEvent(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro) return;

        // If their target was exiled, win game
        var winners = CustomRoleUtils.GetActiveRolesOfType<TraitorRole>()
            .Where(x => x.target && x.targetExiled).ToArray();

        if (winners.Length <= 0) return;
        CustomGameOver.Trigger<TraitorGameOver>(winners.Select(x => x.Player.Data));
    }

    private static PlayerControl GetValidTarget(PlayerControl source)
    {
        return Helpers.GetAlivePlayers().Where(x => x != source).ToArray().Random()!;
    }

    public void OnTargetDeath()
    {
        var roleType = OptionGroupSingleton<TraitorOptions>.Instance.TargetDeathNewRole switch
        {
            TraitorOptions.TraitorBecomes.Crewmate => RoleTypes.Crewmate,
            TraitorOptions.TraitorBecomes.Jester => (RoleTypes)RoleId.Get<JesterRole>(),
            _ => RoleTypes.Crewmate
        };

        RoleManager.Instance.SetRole(Player, roleType);
    }

    public void SetTarget(PlayerControl? newTarget)
    {
        if (Player.AmOwner)
        {
            if (target)
            {
                var tagManager = Utilities.Extensions.GetTagManager(target!);
                if (!tagManager) return;
                tagManager!.RemoveTag(TargetTag);
            }

            if (newTarget)
            {
                var tagManager = Utilities.Extensions.GetTagManager(newTarget!);
                if (!tagManager) return;
            
                var existingTag = tagManager!.GetTagByName(TargetTag.Name);
                if (existingTag.HasValue)
                {
                    tagManager.RemoveTag(existingTag.Value);
                }

                tagManager.AddTag(TargetTag);
            }
        }
        
        target = newTarget;
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);

        if (!OptionGroupSingleton<TraitorOptions>.Instance.CanCallMeeting)
        {
            player.RemainingEmergencies = 0;
        }
        
        SetTarget(GetValidTarget(player));
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        SetTarget(null);
    }

    public override void OnDeath(DeathReason reason)
    {
        RoleBehaviourStubs.OnDeath(this, reason);
        SetTarget(null);
    }

    public override void AppendTaskHint(Il2CppSystem.Text.StringBuilder taskStringBuilder)
    {
        // remove default task hint
    }

    public override bool DidWin(GameOverReason reason)
    {
        return reason == CustomGameOver.GameOverReason<TraitorGameOver>();
    }

    public override bool CanUse(IUsable usable)
    {
        if (!GameManager.Instance.LogicUsables.CanUse(usable, Player))
        {
            return false;
        }

        var console = usable.TryCast<Console>();
        return !(console != null) || console.AllowImpostor;
    }
}
