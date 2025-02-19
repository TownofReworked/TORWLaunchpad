using LaunchpadReloaded.Buttons.Impostor;
using LaunchpadReloaded.Components;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Modifiers;
using LaunchpadReloaded.Roles.Impostor;
using LaunchpadReloaded.Utilities;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Map;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.Events.Vanilla.Usables;
using MiraAPI.Hud;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Utilities;

namespace LaunchpadReloaded;

public static class LaunchpadEventListeners
{
    public static void Initialize()
    {
        MiraEventManager.RegisterEventHandler<StartMeetingEvent>(StartMeetingEvent);
        MiraEventManager.RegisterEventHandler<EjectionEvent>(EjectEvent);
        MiraEventManager.RegisterEventHandler<PlayerCanUseEvent>(CanUseEvent, 10);
        MiraEventManager.RegisterEventHandler<EnterVentEvent>((@event) =>
        {
            if (@event.Player.HasModifier<DragBodyModifier>())
            {
                @event.Cancel();
            }
        });

        MiraEventManager.RegisterEventHandler<PlayerDeathEvent>((@event) =>
        {
            if (@event.Player.AmOwner)
            {
                if (@event.DeathReason == DeathReason.Kill && TutorialManager.InstanceExists && TaskAdderGame.Instance != null)
                {
                    return;
                }

                Coroutines.Start(RoleSelectionMinigame.CoOpen());
            }
        });

        MiraEventManager.RegisterEventHandler<SetRoleEvent>(SetRoleEvent);
        MiraEventManager.RegisterEventHandler<PlayerOpenSabotageEvent>(@event =>
        {
            if (PlayerControl.LocalPlayer.HasModifier<DragBodyModifier>())
            {
                @event.Cancel();
            }
        });
    }

    public static void SetRoleEvent(SetRoleEvent @event)
    {
        if (@event.Player.AmOwner && NotepadHud.Instance != null)
        {
            NotepadHud.Instance.UpdateAspectPos();
        }

        var tagManager = @event.Player.GetTagManager();

        if (tagManager == null)
        {
            return;
        }

        var existingRoleTag = tagManager.GetTagByName("Role");
        if (existingRoleTag.HasValue)
        {
            tagManager.RemoveTag(existingRoleTag.Value);
        }

        var role = @event.Player.Data.Role;
        var color = role is ICustomRole custom ? custom.RoleColor : role.TeamColor;
        var name = role.NiceName;

        if (role.IsDead && name == "STRMISS")
        {
            name = "Ghost";
        }

        var roleTag = new PlayerTag
        {
            Name = "Role",
            Text = name,
            Color = color,
            IsLocallyVisible = (player) =>
            {
                var plrRole = player.Data.Role;
                var localHackedFlag = PlayerControl.LocalPlayer.HasModifier<HackedModifier>();
                var playerHackedFlag = player.HasModifier<HackedModifier>();

                if (localHackedFlag || playerHackedFlag)
                {
                    return false;
                }

                if (player.HasModifier<RevealedModifier>())
                {
                    return true;
                }

                if (plrRole is ICustomRole customRole && (player.AmOwner || customRole.CanLocalPlayerSeeRole(player)))
                {
                    return true;
                }

                if (player.AmOwner || PlayerControl.LocalPlayer.Data.IsDead)
                {
                    return true;
                }

                return false;
            },
        };

        tagManager.AddTag(roleTag);
    }

    public static void EjectEvent(EjectionEvent @event)
    {
        foreach (var plr in PlayerControl.AllPlayerControls)
        {
            var tagManager = plr.GetTagManager();
            if (tagManager != null)
            {
                tagManager.MeetingEnd();
            }
        }

        foreach (var body in DeadBodyCacheComponent.GetFrozenBodies())
        {
            body.body.hideFlags = UnityEngine.HideFlags.None;
        }
    }

    // prevent meetings during hack
    public static void StartMeetingEvent(StartMeetingEvent meetingEvent)
    {
        if (HackerUtilities.AnyPlayerHacked() || meetingEvent.Reporter.HasModifier<DragBodyModifier>())
        {
            meetingEvent.Cancel();
            return;
        }

        if (PlayerControl.LocalPlayer.HasModifier<DragBodyModifier>())
        {
            PlayerControl.LocalPlayer.RpcRemoveModifier<DragBodyModifier>();
        }

        if (PlayerControl.LocalPlayer.Data.Role is SwapshifterRole swap && CustomButtonSingleton<SwapButton>.Instance.EffectActive)
        {
            CustomButtonSingleton<SwapButton>.Instance.OnEffectEnd();
        }

        if (PlayerControl.LocalPlayer.Data.Role is HitmanRole hitman && hitman.InDeadlockMode && HitmanUtilities.MarkedPlayers != null)
        {
            HitmanUtilities.ClearMarks();
            CustomButtonSingleton<DeadlockButton>.Instance.OnEffectEnd();
        }
    }

    // prevent tasks during hack
    public static void CanUseEvent(PlayerCanUseEvent @event)
    {
        if (PlayerControl.LocalPlayer == null)
        {
            return;
        }

        if (PlayerControl.LocalPlayer.HasModifier<DragBodyModifier>())
        {
            if (!@event.IsVent)
            {
                @event.Cancel();
            }

            return;
        }

        if (@event.IsVent)
        {
            var vent = @event.Usable.Cast<Vent>();
            if (vent.IsSealed())
            {
                @event.Cancel();
                return;
            }
        }

        if (PlayerControl.LocalPlayer.Data.IsHacked() && @event.IsPrimaryConsole)
        {
            @event.Cancel();
            return;
        }

        if (HackerUtilities.AnyPlayerHacked() && (@event.Usable.TryCast<SystemConsole>() || @event.Usable.TryCast<MapConsole>()))
        {
            @event.Cancel();
        }
    }
}