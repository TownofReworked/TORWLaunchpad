using AmongUs.GameOptions;
using Il2CppSystem.Text;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Options.Roles.Neutral;
using LaunchpadReloaded.Roles.Afterlife.Outcast;
using MiraAPI.GameOptions;
using MiraAPI.PluginLoading;
using MiraAPI.Roles;
using UnityEngine;

namespace LaunchpadReloaded.Roles.Neutral;

[MiraIgnore]
public class JesterRole(System.IntPtr ptr) : BaseOutcastRole(ptr)
{
    public override string RoleName => "Jester";
    public override string RoleDescription => "Get ejected to win";
    public override string RoleLongDescription => "Convince the crew to vote you out by being suspicious.\nIf you get voted out, you win the game.";
    public override Color RoleColor => LaunchpadPalette.JesterColor;
    public override bool IsDead => false;

    public override CustomRoleConfiguration Configuration => new(this)
    {
        TasksCountForProgress = false,
        CanUseVent = OptionGroupSingleton<JesterOptions>.Instance.CanUseVents,
        GhostRole = (RoleTypes)RoleId.Get<OutcastGhostRole>(),
        Icon = LaunchpadAssets.JesterIcon,
        OptionsScreenshot = LaunchpadAssets.JesterBanner,
    };

    public override void AppendTaskHint(StringBuilder taskStringBuilder)
    {
        // remove default task hint
    }

    public override bool DidWin(GameOverReason reason)
    {
        return reason == (GameOverReason)GameOverReasons.JesterWins;
    }

    public string GetCustomEjectionMessage(NetworkedPlayerInfo exiled)
    {
        return $"You've been fooled! {exiled.PlayerName} was The Jester.";
    }

    public override bool CanUse(IUsable usable)
    {
        if (!GameManager.Instance.LogicUsables.CanUse(usable, Player))
        {
            return false;
        }

        var console = usable.TryCast<Console>();
        return !(console != null) || console.AllowImpostor;
    }

    public override void SpawnTaskHeader(PlayerControl playerControl)
    {
        if (playerControl != PlayerControl.LocalPlayer)
        {
            return;
        }

        var orCreateTask = PlayerTask.GetOrCreateTask<ImportantTextTask>(playerControl);
        orCreateTask.Text = string.Concat([
            LaunchpadPalette.JesterColor.ToTextColor(),
                DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.FakeTasks, Il2CppSystem.Array.Empty<Il2CppSystem.Object>()),
                "</color>"
        ]);
    }
}
