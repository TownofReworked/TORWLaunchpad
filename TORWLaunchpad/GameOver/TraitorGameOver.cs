using LaunchpadReloaded.Features;
using LaunchpadReloaded.Roles.Neutral;
using MiraAPI.GameEnd;
using MiraAPI.Utilities;
using UnityEngine;

namespace LaunchpadReloaded.GameOver;

public sealed class TraitorGameOver : CustomGameOver
{
    public override bool VerifyCondition(PlayerControl playerControl, NetworkedPlayerInfo[] winners)
    {
        return winners is [{ Role: TraitorRole }];
    }

    public override void AfterEndGameSetup(EndGameManager endGameManager)
    {
        endGameManager.WinText.text = "<size=80%>Traitor Wins!</size>";
        endGameManager.WinText.color = LaunchpadPalette.TraitorColor;
        endGameManager.BackgroundBar.material.SetColor(ShaderID.Color, LaunchpadPalette.TraitorColor);
    }
}