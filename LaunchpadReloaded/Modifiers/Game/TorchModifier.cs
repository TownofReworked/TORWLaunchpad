using LaunchpadReloaded.Options.Modifiers;
using MiraAPI.GameOptions;

namespace LaunchpadReloaded.Modifiers.Game;

public class TorchModifier : LPModifier
{
    public override string ModifierName => "Torch";

    public override int GetAssignmentChance() => (int)OptionGroupSingleton<GameModifierOptions>.Instance.GravityChance;

    public override void OnActivate()
    {
        
    }

    public override void OnDeactivate()
    {
        
    }
}