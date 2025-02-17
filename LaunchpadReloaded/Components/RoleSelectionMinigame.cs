using AmongUs.GameOptions;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Patches.Reverse;
using LaunchpadReloaded.Utilities;
using MiraAPI.Roles;
using Reactor.Utilities;
using Reactor.Utilities.Attributes;
using Reactor.Utilities.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Color = UnityEngine.Color;

namespace LaunchpadReloaded.Components;

[RegisterInIl2Cpp]
public sealed class RoleSelectionMinigame(nint ptr) : Minigame(ptr)
{
    public Transform RolesHolder;
    public GameObject RolePrefab;
    public TextMeshPro StatusText;

    private Color _bgColor = new Color32(0, 0, 0, 215);
    private RoleTypes? _selectedRole;

    private List<RoleBehaviour> _availableRoles;

    private void Awake()
    {
        StatusText = transform.FindChild("Status").gameObject.GetComponent<TextMeshPro>();
        RolePrefab = transform.FindChild("RolePrefab").gameObject;
        RolesHolder = transform.FindChild("Roles");

        StatusText.font = HudManager.Instance.TaskPanel.taskText.font;
        StatusText.fontMaterial = HudManager.Instance.TaskPanel.taskText.fontMaterial;
        StatusText.text = "Select a ghost role.";

        transform.localPosition = new Vector3(0, 0, -505);
        StatusText.gameObject.SetActive(false);

        var roles = RoleManager.Instance.AllRoles.Where((role) =>
        {
            var isDeadRole = role.IsDead;
            var sameTeam = false;
            var localRole = PlayerControl.LocalPlayer.Data.Role;

            if (CustomRoleManager.GetCustomRoleBehaviour(role.Role, out var customRole))
            {
                if (localRole is ICustomRole customRole2)
                {
                    sameTeam = customRole?.Team == customRole2.Team;
                }
                else
                {
                    sameTeam = customRole?.Team == (ModdedRoleTeams)localRole.TeamType;
                }
            }
            else
            {
                if (localRole is ICustomRole customRole2)
                {
                    sameTeam = role.TeamType == (RoleTeamTypes)customRole2.Team;
                }
                else
                {
                    sameTeam = role.TeamType == localRole.TeamType;
                }
            }

            return isDeadRole && sameTeam;
        });

        _availableRoles = new List<RoleBehaviour>();

        var rand = new System.Random();
        foreach (var role in roles.OrderBy(role => role.GetRoleChance()))
        {
            var randomNum = rand.Next(100);
            var roleChance = role.GetRoleChance();

            if (randomNum < roleChance)
            {
                _availableRoles.Add(role);

                if (_availableRoles.Count >= 2)
                {
                    break;
                }
            }
        }


    }

    public void FixedClose(bool closedDueToMeeting = false)
    {
        if (_selectedRole.HasValue && PlayerControl.LocalPlayer.Data.Role.Role != _selectedRole.Value)
        {
            PlayerControl.LocalPlayer.RpcSetRole(_selectedRole.Value, true);
        }

        HudManager.Instance.StartCoroutine(HudManager.Instance.CoFadeFullScreen(_bgColor, Color.clear, 0.2f, false));

        if (closedDueToMeeting) // Wait until meeting ends, then re-open screen
        {
            Coroutines.Start(CoOpen());
        }

        MinigameStubs.Close(this);
    }

    public override void Close()
    {
        FixedClose(MeetingHud.Instance != null);
    }

    public static IEnumerator CoOpen()
    {
        while ((HudManager.Instance.KillOverlay != null && HudManager.Instance.KillOverlay.IsOpen) || MeetingHud.Instance != null || ExileController.Instance != null)
        {
            yield return new WaitForSeconds(0.65f);
        }

        var gameObject = Instantiate(LaunchpadAssets.GhostRoleGame.LoadAsset(), HudManager.Instance.transform);
        gameObject.GetComponent<Minigame>().DestroyImmediate();

        var minigame = gameObject.AddComponent<RoleSelectionMinigame>();
        minigame.Open();
    }

    private void Open()
    {
        if (_availableRoles.Count <= 1)
        {
            Destroy(gameObject);
            return;
        }

        HudManager.Instance.StartCoroutine(HudManager.Instance.CoFadeFullScreen(Color.clear, _bgColor, 0.2f, false));

        StatusText.gameObject.SetActive(true);

        foreach (var role in _availableRoles)
        {
            var newRoleObj = Instantiate(RolePrefab, RolesHolder);
            var text = newRoleObj.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>();
            var spriteRend = newRoleObj.GetComponent<SpriteRenderer>();
            var passiveButton = newRoleObj.GetComponent<PassiveButton>();

            text.font = HudManager.Instance.TaskPanel.taskText.font;
            text.fontMaterial = HudManager.Instance.TaskPanel.taskText.fontMaterial;
            spriteRend.material = HatManager.Instance.PlayerMaterial;

            var color = Palette.CrewmateRoleBlue;
            var roleName = role.NiceName == "STRMISS" ? "Ghost" : role.NiceName;

            if (CustomRoleManager.GetCustomRoleBehaviour(role.Role, out var customRole))
            {
                color = customRole!.RoleColor;
            }
            else
            {
                color = role.TeamColor;
            }

            text.text = roleName;
            spriteRend.gameObject.AddComponent<GradientColorComponent>().SetColor(color, color);

            passiveButton.OnClick.RemoveAllListeners();
            passiveButton.OnClick.AddListener((UnityAction)(() =>
            {
                _selectedRole = role.Role;
                Close();
            }));

            newRoleObj.gameObject.SetActive(true);
        }

        Begin(null);
    }
}