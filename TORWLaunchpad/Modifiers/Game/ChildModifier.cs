using LaunchpadReloaded.Options.Modifiers;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;

namespace LaunchpadReloaded.Modifiers.Fun;

public sealed class ChildModifier : LPModifier
{
    public override string ModifierName => "Child";
    public override int GetAssignmentChance() => (int)OptionGroupSingleton<GameModifierOptions>.Instance.ChildChance;
    public override int GetAmountPerGame() => 1;
    public override bool IsModifierValidOn(RoleBehaviour role) => base.IsModifierValidOn(role) && !role.Player.HasModifier<GiantModifier>();

    public override void OnActivate()
    {
        if (Player != null)
        {
            Player.MyPhysics.Speed /= 0.75f;
            Player.transform.localScale *= 0.7f;
        }
    }

    public override void OnDeactivate()
    {
        if (Player != null)
        {
            Player.MyPhysics.Speed *= 0.75f;
            Player.transform.localScale /= 0.7f;
        }
    }
}