using LaunchpadReloaded.Components;
using LaunchpadReloaded.Modifiers;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Map;
using MiraAPI.Events.Vanilla.Usables;
using MiraAPI.Modifiers;

namespace LaunchpadReloaded.Events.Roles;

public static class JanitorEvents
{
    [RegisterEvent]
    public static void PlayerOpenSabotageEvent(PlayerOpenSabotageEvent @event)
    {
        if (!PlayerControl.LocalPlayer.HasModifier<DragBodyModifier>()) return;
        @event.Cancel();
    }
    
    [RegisterEvent]
    public static void ExitVentEvent(ExitVentEvent @event)
    {
        if (@event.Vent && @event.Vent!.TryGetComponent(out VentBodyComponent ventBody) && ventBody.deadBody)
        {
            ventBody.ExposeBody();
        }

        if (!@event.Player.HasModifier<DragBodyModifier>()) return;
        @event.Cancel();
    }

    [RegisterEvent]
    public static void EnterVentEvent(EnterVentEvent @event)
    {
        if (@event.Vent && @event.Vent!.TryGetComponent(out VentBodyComponent ventBody) && ventBody.deadBody)
        {
            ventBody.ExposeBody();
        }

        if (!@event.Player.HasModifier<DragBodyModifier>()) return;
        @event.Cancel();
    }
}