using AmongUs.GameOptions;
using LaunchpadReloaded.Components;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Modifiers;
using LaunchpadReloaded.Options;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using PowerTools;
using Reactor.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace LaunchpadReloaded.Utilities;

public static class Extensions
{
    private static readonly ContactFilter2D Filter = ContactFilter2D.CreateLegacyFilter(Constants.NotShipMask, float.MinValue, float.MaxValue);

    // Vanilla code to prevent crashes (Credits to Submerged)
    public static void BaseClose(this Minigame self)
    {
        bool isComplete;
        if (self.amClosing == Minigame.CloseState.Closing)
        {
            Object.Destroy(self.gameObject);
            return;
        }
        if (self.CloseSound && Constants.ShouldPlaySfx())
        {
            SoundManager.Instance.PlaySound(self.CloseSound, false, 1f, null);
        }
        if (PlayerControl.LocalPlayer.Data.Role.TeamType == RoleTeamTypes.Crewmate)
        {
            GameManager.Instance.LogicMinigame.OnMinigameClose();
        }
        if (PlayerControl.LocalPlayer)
        {
            PlayerControl.HideCursorTemporarily();
        }
        self.amClosing = Minigame.CloseState.Closing;
        self.logger.Info(string.Concat("Closing minigame ", self.GetType().Name));
        IAnalyticsReporter analytics = DestroyableSingleton<DebugAnalytics>.Instance.Analytics;
        NetworkedPlayerInfo data = PlayerControl.LocalPlayer.Data;
        TaskTypes taskType = self.TaskType;
        float realtimeSinceStartup = Time.realtimeSinceStartup - self.timeOpened;
        PlayerTask myTask = self.MyTask;
        if (myTask != null)
        {
            isComplete = myTask.IsComplete;
        }
        else
        {
            isComplete = false;
        }
        analytics.MinigameClosed(data, taskType, realtimeSinceStartup, isComplete);
        self.StartCoroutine(self.CoDestroySelf());
    }

    public static int GetRoleChance(this RoleBehaviour role)
    {
        int chance = 0;

        if (CustomRoleManager.GetCustomRoleBehaviour(role.Role, out var customRole))
        {
            chance = customRole!.GetChance()!.Value;
        }
        else if (role.Role == RoleTypes.ImpostorGhost || role.Role == RoleTypes.CrewmateGhost)
        {
            chance = 100;
        }
        else if (role.Role == RoleTypes.GuardianAngel)
        {
            IRoleOptionsCollection roleOptions = GameOptionsManager.Instance.currentHostOptions.RoleOptions;
            chance = roleOptions.GetChancePerGame(role.Role);
        }

        return chance;
    }

    public static PlayerTagManager? GetTagManager(this PlayerControl player)
    {
        return player.GetComponent<PlayerTagManager>();
    }

    public static DeadBodyCacheComponent GetCacheComponent(this DeadBody body)
    {
        return body.gameObject.GetComponent<DeadBodyCacheComponent>();
    }

    public static void ClearModifiers(this ModifierComponent comp)
    {
        foreach (var mod in comp.ActiveModifiers)
        {
            comp.RemoveModifier(mod);
        }
    }

    public static void SetBloomByMap(this Bloom bloom)
    {
        if (ShipStatus.Instance == null)
        {
            return;
        }

        bloom.ThresholdLinear =
            ShipStatus.Instance.TryCast<AirshipStatus>() ||
            ShipStatus.Instance.Type is ShipStatus.MapType.Hq or ShipStatus.MapType.Fungle
                ? 1.3f :
                0.95f;
    }

    public static void SetBodyType(this PlayerControl player, int bodyType)
    {
        if (bodyType == 6)
        {
            player.MyPhysics.SetBodyType(PlayerBodyTypes.Seeker);
            if (!OptionGroupSingleton<BattleRoyaleOptions>.Instance.ShowKnife.Value)
            {
                return;
            }

            var seekerHand = player.transform.FindChild("BodyForms/Seeker/SeekerHand").gameObject;
            var hand = Object.Instantiate(seekerHand).gameObject;
            hand.transform.SetParent(seekerHand.transform.parent);
            hand.transform.localScale = new Vector3(2, 2, 2);
            hand.name = "KnifeHand";
            hand.layer = LayerMask.NameToLayer("Players");

            var transform = player.transform;

            hand.transform.localPosition = transform.localPosition;
            hand.transform.position = transform.position;

            var nodeSync = hand.GetComponent<SpriteAnimNodeSync>();
            nodeSync.flipOffset = new Vector3(-1.5f, 0.5f, 0);
            nodeSync.normalOffset = new Vector3(1.5f, 0.5f, 0);

            var rend = hand.GetComponent<SpriteRenderer>();
            rend.sprite = LaunchpadAssets.KnifeHandSprite.LoadAsset();

            hand.SetActive(true);
            return;
        }

        player.MyPhysics.SetBodyType((PlayerBodyTypes)bodyType);

        if (bodyType == (int)PlayerBodyTypes.Normal)
        {
            player.cosmetics.currentBodySprite.BodySprite.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
            player.cosmetics.SetNamePosition(new Vector3(0, 1, -.5f));
        }
    }

    public static void SetGradientData(this GameObject gameObject, byte playerId)
    {
        var data = gameObject.GetComponent<PlayerGradientData>();
        if (!data)
        {
            data = gameObject.AddComponent<PlayerGradientData>();
        }
        data.playerId = playerId;
    }

    public static KeyValuePair<byte, int> MaxPair(this Dictionary<byte, int> self, out bool tie)
    {
        tie = true;
        var result = new KeyValuePair<byte, int>(byte.MaxValue, int.MinValue);
        foreach (var keyValuePair in self)
        {
            if (keyValuePair.Value > result.Value)
            {
                result = keyValuePair;
                tie = false;
            }
            else if (keyValuePair.Value == result.Value)
            {
                tie = true;
            }
        }
        return result;
    }

    public static KeyValuePair<byte, float> MaxPair(this Dictionary<byte, float> self, out bool tie)
    {
        tie = true;
        var result = new KeyValuePair<byte, float>(byte.MaxValue, int.MinValue);
        foreach (var keyValuePair in self)
        {
            if (keyValuePair.Value > result.Value)
            {
                result = keyValuePair;
                tie = false;
            }
            else if (Math.Abs(keyValuePair.Value - result.Value) < .05)
            {
                tie = true;
            }
        }
        return result;
    }


    // Source: https://stackoverflow.com/a/78897981
    public static int GetColumnCount(this GridLayoutGroup grid)
    {
        if (grid.transform.childCount <= 1)
        {
            return grid.transform.childCount;
        }

        var maxWidth = grid.GetComponent<RectTransform>().rect.width;
        var cellWidth = grid.cellSize.x;
        var cellSpacing = grid.spacing.x;
        for (var i = 2; i < grid.transform.childCount; ++i)
        {
            if (i * cellWidth + (i - 1) * cellSpacing > maxWidth)
            {
                return i - 1;
            }
        }

        return grid.transform.childCount;
    }

    public static bool ButtonTimerEnabled(this PlayerControl playerControl)
    {
        return (playerControl.moveable || playerControl.petting) && playerControl is { inVent: false, shapeshifting: false } && (!DestroyableSingleton<HudManager>.InstanceExists || !DestroyableSingleton<HudManager>.Instance.IsIntroDisplayed) && !MeetingHud.Instance && !PlayerCustomizationMenu.Instance && !ExileController.Instance && !IntroCutscene.Instance;
    }

    public static bool IsSealed(this Vent vent)
    {
        return vent.gameObject.TryGetComponent<SealedVentComponent>(out _);
    }

    public static void Revive(this DeadBody body, PlayerControl sender)
    {
        var player = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(player => player.PlayerId == body.ParentId);
        if (player == null)
        {
            return;
        }

        player.NetTransform.SnapTo(body.transform.position);

        if (PhysicsHelpers.AnythingBetween(player.Collider, player.Collider.bounds.center, player.transform.position, Constants.ShipAndAllObjectsMask, false))
        {
            player.NetTransform.SnapTo(sender.transform.position);
        }

        body.gameObject.Destroy();
        player.GetModifierComponent()!.AddModifier<RevivedModifier>();
    }

    public static void HideBody(this DeadBody body)
    {
        body.GetComponent<DeadBodyCacheComponent>().SetVisibility(false);
        body.Reported = true;
    }

    public static void ShowBody(this DeadBody body, bool reported)
    {
        body.GetComponent<DeadBodyCacheComponent>().SetVisibility(true);
        body.Reported = reported;
    }

    public static bool IsOverride(this MethodInfo methodInfo)
    {
        return methodInfo.GetBaseDefinition() != methodInfo;
    }
}