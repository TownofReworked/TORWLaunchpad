using AmongUs.GameOptions;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Options.Roles.Neutral;
using MiraAPI.GameOptions;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using System.Text;
using Il2CppInterop.Runtime.Attributes;
using LaunchpadReloaded.Roles.Afterlife.Outcast;
using UnityEngine;

namespace LaunchpadReloaded.Roles.Neutral;

public class ReaperRole(System.IntPtr ptr) : RoleBehaviour(ptr), IOutcastRole
{
    public string RoleName => "Reaper";
    public string RoleDescription => "Collect souls to win";
    public string RoleLongDescription => "Collect souls from dead bodies to win the game.";
    public Color RoleColor => LaunchpadPalette.ReaperColor;
    public override bool IsDead => false;

    public int CollectedSouls;

    public CustomRoleConfiguration Configuration => new(this)
    {
        TasksCountForProgress = false,
        CanUseVent = false,
        GhostRole = (RoleTypes)RoleId.Get<OutcastGhostRole>(),
        Icon = LaunchpadAssets.SoulButton,
        OptionsScreenshot = LaunchpadAssets.JesterBanner,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var sb = Helpers.CreateForRole(this);
        sb.Append($"\n<b>{CollectedSouls}/{OptionGroupSingleton<ReaperOptions>.Instance.SoulCollections} souls collected.");
        return sb;
    }

    public override void AppendTaskHint(Il2CppSystem.Text.StringBuilder taskStringBuilder)
    {
        // remove default task hint
    }

    /*
    private GameObject _display;
    private TextMeshPro _displayText;

    public void UpdateSoulsCollected()
    {
        if (_display != null)
        {
            _displayText.text = $"{CollectedSouls}/{OptionGroupSingleton<ReaperOptions>.Instance.SoulCollections} Souls";
        }
    }
    
        public override void SpawnTaskHeader(PlayerControl playerControl)
        {
            if (playerControl.AmOwner)
            {
                _display = Instantiate(LaunchpadAssets.ReaperDisplay.LoadAsset(), HudManager.Instance.transform);
                var aspectPosition = _display.AddComponent<AspectPosition>();
                aspectPosition.Alignment = AspectPosition.EdgeAlignments.Top;
                aspectPosition.DistanceFromEdge = new Vector3(0, 0.8f, 0);
                aspectPosition.AdjustPosition();

                _display.transform.localPosition = new Vector3(_display.transform.localPosition.x, _display.transform.localPosition.y, 20);
                _displayText = _display.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>();
                _displayText.font = HudManager.Instance.TaskPanel.taskText.font;
                _displayText.fontMaterial = HudManager.Instance.TaskPanel.taskText.fontMaterial;

                UpdateSoulsCollected();
            }
        }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        if (targetPlayer.AmOwner && _display != null)
        {
            _display.gameObject.DestroyImmediate();
        }
    }*/

    public override bool DidWin(GameOverReason reason)
    {
        return reason == (GameOverReason)GameOverReasons.ReaperWins;
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
}
