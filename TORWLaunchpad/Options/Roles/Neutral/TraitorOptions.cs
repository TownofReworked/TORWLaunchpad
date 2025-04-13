using LaunchpadReloaded.Roles.Neutral;
using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;

namespace LaunchpadReloaded.Options.Roles.Neutral;

public class TraitorOptions : AbstractOptionGroup<TraitorRole>
{
    public override string GroupName => "Traitor";

    [ModdedToggleOption("Can Call Meeting")]
    public bool CanCallMeeting { get; set; } = false;

    [ModdedEnumOption("On Target Death, Traitor Becomes", typeof(TraitorBecomes))]
    public TraitorBecomes TargetDeathNewRole { get; set; } = TraitorBecomes.Jester;

    public enum TraitorBecomes
    {
        Crewmate,
        Jester,
    }
}