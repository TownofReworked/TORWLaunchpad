using LaunchpadReloaded.Options.Modifiers;
using MiraAPI.GameOptions;

namespace LaunchpadReloaded.Modifiers.Fun;

public sealed class FlashModifier : LPModifier
{
    public override string ModifierName => "Flash";
    public override int GetAssignmentChance() => (int)OptionGroupSingleton<GameModifierOptions>.Instance.FlashChance;
    public override int GetAmountPerGame() => 1;

    public override void OnActivate()
    {
        if (Player != null)
        {
            Player.MyPhysics.Speed /= 0.75f;
        }
    }

    public override void OnDeactivate()
    {
        if (Player != null)
        {
            Player.MyPhysics.Speed *= 0.75f;
        }
    }
}