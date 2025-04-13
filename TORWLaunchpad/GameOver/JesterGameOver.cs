using LaunchpadReloaded.Features;
using LaunchpadReloaded.Roles.Neutral;
using MiraAPI.GameEnd;
using MiraAPI.Utilities;
using UnityEngine;

namespace LaunchpadReloaded.GameOver;

public sealed class JesterGameOver : CustomGameOver
{
    public override bool VerifyCondition(PlayerControl playerControl, NetworkedPlayerInfo[] winners)
    {
        return winners is [{ Role: JesterRole }];
    }

    public override void AfterEndGameSetup(EndGameManager endGameManager)
    {
        endGameManager.WinText.text = "Jester Wins!";
        endGameManager.WinText.color = LaunchpadPalette.JesterColor;
        endGameManager.BackgroundBar.material.SetColor(ShaderID.Color, LaunchpadPalette.JesterColor);
    }
}