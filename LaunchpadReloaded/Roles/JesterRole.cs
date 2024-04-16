using AmongUs.GameOptions;
using Il2CppSystem.Text;
using LaunchpadReloaded.API.GameOptions;
using LaunchpadReloaded.API.Roles;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Features.Translations;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace LaunchpadReloaded.Roles;

[RegisterInIl2Cpp]
public class JesterRole(System.IntPtr ptr) : RoleBehaviour(ptr), ICustomRole
{
    public TranslationStringNames RoleName => TranslationStringNames.JesterRoleName;
    public ushort RoleId => (ushort)LaunchpadRoles.Jester;
    public TranslationStringNames RoleDescription => TranslationStringNames.JesterShortDesc;
    public TranslationStringNames RoleLongDescription => TranslationStringNames.JesterLongDesc;
    public Color RoleColor => LaunchpadPalette.JesterColor;
    public RoleTeamTypes Team => RoleTeamTypes.Crewmate;
    public bool IsOutcast => true;
    public bool TasksCount => false;
    public bool CanUseVent => CanUseVents?.Value ?? true;
    public RoleTypes GhostRole => (RoleTypes)LaunchpadRoles.OutcastGhost;
    public override bool IsDead => false;
    public LoadableAsset<Sprite> Icon => LaunchpadAssets.JesterIcon;

    public override void AppendTaskHint(StringBuilder taskStringBuilder) { }
    public override bool DidWin(GameOverReason reason)
    {
        return reason == (GameOverReason)GameOverReasons.JesterWins;
    }

    public string GetCustomEjectionMessage(GameData.PlayerInfo exiled)
    {
        return $"You've been fooled! {exiled.PlayerName} was The Jester.";
    }
    public override bool CanUse(IUsable usable)
    {
        if (!GameManager.Instance.LogicUsables.CanUse(usable, this.Player))
        {
            return false;
        }

        Console console = usable.TryCast<Console>();
        return !(console != null) || console.AllowImpostor;
    }

    public override void SpawnTaskHeader(PlayerControl playerControl)
    {
        if (playerControl != PlayerControl.LocalPlayer) return;
        ImportantTextTask orCreateTask = PlayerTask.GetOrCreateTask<ImportantTextTask>(playerControl, 0);
        orCreateTask.Text = string.Concat(new string[]
            {
                LaunchpadPalette.JesterColor.ToTextColor(),
                DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.FakeTasks, Il2CppSystem.Array.Empty<Il2CppSystem.Object>()),
                "</color>"
            });

    }
    public static CustomToggleOption CanUseVents;
    public static CustomOptionGroup Group;
    public void CreateOptions()
    {
        CanUseVents = new(TranslationStringNames.JesterCanUseVents, true, typeof(JesterRole));
        Group = new CustomOptionGroup(RoleName,
            numberOpt: [],
            stringOpt: [],
            toggleOpt: [CanUseVents], role: typeof(JesterRole));
        Group.SetColor(RoleColor);
    }
}