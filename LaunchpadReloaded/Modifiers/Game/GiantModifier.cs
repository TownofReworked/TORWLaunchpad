using LaunchpadReloaded.Options.Modifiers;
using MiraAPI.GameOptions;
using MiraAPI.Utilities;

namespace LaunchpadReloaded.Modifiers.Game;

public sealed class GiantModifier : LPModifier
{
    public override string ModifierName => "Giant";
    public override int GetAssignmentChance() => (int)OptionGroupSingleton<GameModifierOptions>.Instance.GiantChance;
    public override bool IsModifierValidOn(RoleBehaviour role) => base.IsModifierValidOn(role) && !role.Player.HasModifier<SmolModifier>();
    public override void OnActivate()
    {
        if (Player != null)
        {
            Player.MyPhysics.Speed *= 0.8f;
            Player.transform.localScale /= 0.7f;
        }
    }

    public override void OnDeactivate()
    {
        if (Player != null)
        {
            Player.MyPhysics.Speed /= 0.8f;
            Player.transform.localScale *= 0.7f;
        }
    }
}