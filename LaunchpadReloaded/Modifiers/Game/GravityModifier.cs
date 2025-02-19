using LaunchpadReloaded.Components;
using LaunchpadReloaded.Options.Modifiers;
using MiraAPI.GameOptions;
using UnityEngine;

namespace LaunchpadReloaded.Modifiers.Game;

public sealed class GravityModifier : LPModifier
{
    public override string ModifierName => "Gravity Field";
    public override int GetAssignmentChance() => (int)OptionGroupSingleton<GameModifierOptions>.Instance.GravityChance;

    private GameObject? detectionCircle;

    public override void OnActivate()
    {
        if (Player is null)
        {
            return;
        }

        detectionCircle = new GameObject("DetectionCircle");
        detectionCircle.transform.SetParent(Player.transform);
        detectionCircle.transform.localPosition = new Vector3(0, 0, 0);

        var collider = detectionCircle.AddComponent<CircleCollider2D>();
        collider.radius = OptionGroupSingleton<GameModifierOptions>.Instance.GravityFieldRadius.Value;
        collider.isTrigger = true;

        var gravityComp = detectionCircle.AddComponent<GravityComponent>();
        gravityComp.gravityGuy = Player;

        detectionCircle.gameObject.SetActive(true);
    }

    public override void OnDeactivate()
    {
        if (detectionCircle != null)
        {
            Object.Destroy(detectionCircle.gameObject);
        }
    }
}