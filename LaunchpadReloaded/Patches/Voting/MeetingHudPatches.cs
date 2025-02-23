using HarmonyLib;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Features.Voting;
using LaunchpadReloaded.Modifiers;
using LaunchpadReloaded.Networking.Voting;
using LaunchpadReloaded.Options;
using LaunchpadReloaded.Options.Roles.Crewmate;
using LaunchpadReloaded.Roles.Crewmate;
using LaunchpadReloaded.Utilities;
using MiraAPI.GameOptions;
using MiraAPI.Utilities;
using Reactor.Networking.Rpc;
using Reactor.Utilities.Extensions;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Helpers = LaunchpadReloaded.Utilities.Helpers;

namespace LaunchpadReloaded.Patches.Voting;

[HarmonyPatch(typeof(MeetingHud))]
public static class MeetingHudPatches
{
    private static GameObject? _typeText;
    private static PlayerVoteArea? _confirmVotes;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(MeetingHud.Start))]
    public static void AwakePostfix(MeetingHud __instance)
    {
        if (NotepadHud.Instance != null)
        {
            NotepadHud.Instance.UpdateAspectPos();
        }

        foreach (var plr in PlayerControl.AllPlayerControls)
        {
            var state = __instance.playerStates.FirstOrDefault(state => state.TargetPlayerId == plr.PlayerId);
            var tagManager = plr.GetTagManager();

            if (state != null && tagManager != null)
            {
                tagManager.MeetingStart();
            }

            plr.GetModifierComponent()?.RemoveModifier<DragBodyModifier>();

            if (!plr.HasModifier<VoteData>())
            {
                continue;
            }

            var voteData = plr.GetModifier<VoteData>()!;
            voteData.VotesRemaining = (plr.HasModifier<RevivedModifier>() || plr.Data.IsDead || plr.Data.Disconnected) ? 0 : VotingTypesManager.GetVotes();
            voteData.VotedPlayers.Clear();

            if (plr.Data.Role is MayorRole)
            {
                voteData.VotesRemaining += (int)OptionGroupSingleton<MayorOptions>.Instance.ExtraVotes;
            }
        }

        if (_typeText == null)
        {
            _typeText = Object.Instantiate(__instance.TimerText, __instance.TimerText.transform.parent).gameObject;
            _typeText.GetComponent<TextTranslatorTMP>().Destroy();
            _typeText.GetComponent<TextMeshPro>().alignment = TextAlignmentOptions.Left;
            _typeText.transform.localPosition = new Vector3(-1.4327f, -2.1964f, 0);
            _typeText.name = "VoteTypeText";
            _typeText.gameObject.SetActive(false);
        }

        if (_confirmVotes == null && (VotingTypesManager.CanVoteMultiple() ||
                                      OptionGroupSingleton<VotingOptions>.Instance.AllowConfirmingVotes.Value))
        {
            _confirmVotes = Object.Instantiate(__instance.SkipVoteButton, __instance.SkipVoteButton.transform.parent);
            _confirmVotes.gameObject.name = "ConfirmVotesBtn";
            _confirmVotes.SetTargetPlayerId((byte)SpecialVotes.Confirm);
            _confirmVotes.Parent = __instance;

            var confirmText = _confirmVotes.gameObject.GetComponentInChildren<TextMeshPro>();
            _confirmVotes.gameObject.GetComponentInChildren<TextTranslatorTMP>().Destroy();
            confirmText.text = "CONFIRM VOTES";
            __instance.SkipVoteButton.transform.position += new Vector3(0, 0.18f, 0);
            _confirmVotes.transform.position -= new Vector3(0, 0.1f, 0);
            foreach (var plr in __instance.playerStates.AddItem(__instance.SkipVoteButton))
            {
                plr.gameObject.GetComponentInChildren<PassiveButton>().OnClick
                    .AddListener((UnityAction)(() => _confirmVotes.ClearButtons()));
            }
        }
        else if (_confirmVotes != null && !VotingTypesManager.CanVoteMultiple() &&
                 !OptionGroupSingleton<VotingOptions>.Instance.AllowConfirmingVotes.Value)
        {
            _confirmVotes.gameObject.Destroy();
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(MeetingHud.Update))]
    public static void UpdatePatch(MeetingHud __instance)
    {
        if (HackerUtilities.AnyPlayerHacked())
        {
            HackerUtilities.ForceEndHack();
        }

        var voteData = PlayerControl.LocalPlayer.GetModifier<VoteData>();
        if (voteData == null || _typeText == null)
        {
            return;
        }

        var tmp = _typeText.GetComponent<TextMeshPro>();
        tmp.text = VotingTypesManager.SelectedType != VotingTypes.Classic
            ? $"<size=160%>{voteData.VotesRemaining} votes left</size>\nVoting Type: {VotingTypesManager.SelectedType}"
            : $"<size=160%>{voteData.VotesRemaining} votes left</size>";

        var logicOptionsNormal = GameManager.Instance.LogicOptions.TryCast<LogicOptionsNormal>();

        if (logicOptionsNormal is not null)
        {
            var votingTime = logicOptionsNormal.GetVotingTime();
            if (votingTime > 0)
            {
                var num2 = __instance.discussionTimer - logicOptionsNormal.GetDiscussionTime();
                if (AmongUsClient.Instance.AmHost && num2 >= votingTime)
                {
                    foreach (var player in Helpers.GetAlivePlayers()
                                 .Where(x => x.GetModifier<VoteData>()!.VotesRemaining > 0))
                    {
                        __instance.CastVote(player.PlayerId, (byte)SpecialVotes.Confirm);
                    }
                }
            }

        }

        if (PlayerControl.LocalPlayer.Data.IsDead)
        {
            if (_confirmVotes != null)
            {
                _confirmVotes.SetDisabled();
            }

            _typeText.gameObject.SetActive(false);
            if (__instance.state != MeetingHud.VoteStates.Results)
            {
                return;
            }

            foreach (var voteArea in __instance.playerStates.Where(state => !state.resultsShowing))
            {
                voteArea.ClearForResults();
            }

            return;
        }

        switch (__instance.state)
        {
            case MeetingHud.VoteStates.Voted:
            case MeetingHud.VoteStates.NotVoted:
                if (PlayerControl.LocalPlayer.GetModifier<VoteData>()!.VotesRemaining == 0)
                {
                    _typeText.gameObject.SetActive(false);
                    if (_confirmVotes != null)
                    {
                        _confirmVotes.SetDisabled();
                    }
                }
                else
                {
                    _typeText.gameObject.SetActive(true);
                    if (_confirmVotes != null)
                    {
                        _confirmVotes.SetEnabled();
                    }
                }

                break;

            case MeetingHud.VoteStates.Results:
                if (_confirmVotes != null)
                {
                    _confirmVotes.SetDisabled();
                }

                _typeText.gameObject.SetActive(false);
                foreach (var voteArea in __instance.playerStates.Where(state => !state.resultsShowing))
                    voteArea.ClearForResults();
                break;

            default:
                if (_confirmVotes != null)
                {
                    _confirmVotes.SetDisabled();
                }

                _typeText.gameObject.SetActive(false);
                break;
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(MeetingHud.CheckForEndVoting))]
    public static bool EndCheck(MeetingHud __instance)
    {
        if (Helpers.GetAlivePlayers().Where(plr => plr.HasModifier<VoteData>()).Any(plr => plr.GetModifier<VoteData>()!.VotesRemaining > 0))
        {
            return false;
        }

        NetworkedPlayerInfo? exiled;
        bool isTie;
        
        var votes = VotingTypesManager.CalculateVotes();

        if (VotingTypesManager.UseChance())
        {
            isTie = false;
            var playerId = VotingTypesManager.GetVotedPlayerByChance(votes);
            exiled = GameData.Instance.GetPlayerById(playerId);
        }
        else
        {
            var max = VotingTypesManager.CalculateNumVotes(votes).MaxPair(out isTie);
            exiled = GameData.Instance.AllPlayers.ToArray().FirstOrDefault(v => !isTie && v.PlayerId == max.Key);
        }

        if (exiled is null || exiled.IsDead || exiled.Disconnected)
        {
            exiled = null;
        }

        __instance.RpcVotingComplete(new MeetingHud.VoterState[__instance.playerStates.Length], exiled, isTie);

        return false;
    }


    [HarmonyPrefix]
    [HarmonyPatch(nameof(MeetingHud.Select))]
    public static bool SelectPatch(MeetingHud __instance, int suspectStateIdx)
    {
        var voteData = PlayerControl.LocalPlayer.GetModifier<VoteData>();
        if (voteData == null)
        {
            return false;
        }

        var hasVotes = voteData.VotesRemaining > 0;
        var hasVotedFor = voteData.VotedPlayers.Contains((byte)suspectStateIdx);

        if (OptionGroupSingleton<VotingOptions>.Instance.AllowVotingForSamePerson.Value
            && hasVotes)
        {
            return true;
        }

        return hasVotes && !hasVotedFor;
    }



    [HarmonyPrefix]
    [HarmonyPatch(nameof(MeetingHud.HandleDisconnect), [typeof(PlayerControl), typeof(DisconnectReasons)])]
    public static bool HandleDisconnect(MeetingHud __instance, [HarmonyArgument(0)] PlayerControl pc)
    {
        if (!AmongUsClient.Instance.AmHost || __instance.playerStates is null || !pc || !GameData.Instance)
        {
            return false;
        }

        var playerVoteArea = __instance.playerStates.First(pv => pv.TargetPlayerId == pc.PlayerId);
        playerVoteArea.AmDead = true;
        playerVoteArea.Overlay.gameObject.SetActive(true);

        foreach (var player in Helpers.GetAlivePlayers())
        {
            var pva = __instance.playerStates.First(pv => pv.TargetPlayerId == player.PlayerId);
            var voteData = player.GetModifier<VoteData>();

            if (pva.AmDead || voteData == null || !voteData.VotedPlayers.Contains(pc.PlayerId))
            {
                continue;
            }

            voteData.VotedPlayers.Remove(pc.PlayerId);
            voteData.VotesRemaining += 1;

            VotingRpc.RpcRemoveVote(PlayerControl.LocalPlayer, player.PlayerId, pc.PlayerId);
        }

        __instance.SetDirtyBit(1U);
        __instance.CheckForEndVoting();

        if (__instance.state == MeetingHud.VoteStates.Results)
        {
            __instance.SetupProceedButton();
        }

        return false;
    }


    [HarmonyPrefix]
    [HarmonyPatch(nameof(MeetingHud.PopulateResults))]
    public static bool PopulateResultsPatch(MeetingHud __instance)
    {
        if (!AmongUsClient.Instance.AmHost)
        {
            return false;
        }

        var votes = VotingTypesManager.CalculateVotes();

        Rpc<PopulateResultsRpc>.Instance.Send(votes.ToArray());
        return false;
    }

    private static void HandleVote(VoteData voteData, byte suspectIdx)
    {
        switch (suspectIdx)
        {
            case (byte)SpecialVotes.Confirm:
                voteData.VotesRemaining = 0;
                break;
            default:
                voteData.VotesRemaining--;
                voteData.VotedPlayers.Add(suspectIdx);
                break;
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(MeetingHud.CastVote))]
    public static bool CastVotePatch(MeetingHud __instance, [HarmonyArgument(0)] byte playerId,
        [HarmonyArgument(1)] byte suspectIdx)
    {
        var plr = GameData.Instance.GetPlayerById(playerId);

        if (plr is null || !plr.Object.HasModifier<VoteData>())
        {
            return false;
        }

        var voteData = plr.Object.GetModifier<VoteData>();
        if (voteData == null || voteData.VotesRemaining == 0 ||
            (voteData.VotedPlayers.Contains(suspectIdx) &&
             !OptionGroupSingleton<VotingOptions>.Instance.AllowVotingForSamePerson.Value))
        {
            return false;
        }

        HandleVote(voteData, suspectIdx);

        __instance.SetDirtyBit(1U);
        __instance.CheckForEndVoting();

        if (voteData.VotesRemaining != 0)
        {
            return false;
        }

        __instance.playerStates.First(x => x.TargetPlayerId == playerId).SetVote(suspectIdx);
        PlayerControl.LocalPlayer.RpcSendChatNote(playerId, ChatNoteTypes.DidVote);

        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(MeetingHud.CmdCastVote))]
    public static void CmdCastVotePatch(MeetingHud __instance, [HarmonyArgument(0)] byte playerId,
        [HarmonyArgument(1)] byte suspectIdx)
    {

        var plr = GameData.Instance.GetPlayerById(playerId);
        var voteData = plr.Object.GetModifier<VoteData>();

        if (!AmongUsClient.Instance.AmHost)
        {
            if (voteData == null || voteData.VotesRemaining == 0 ||
                (voteData.VotedPlayers.Contains(suspectIdx) &&
                 !OptionGroupSingleton<VotingOptions>.Instance.AllowVotingForSamePerson.Value))
            {
                return;
            }

            HandleVote(voteData, suspectIdx);

            if (voteData.VotesRemaining == 0)
            {
                __instance.playerStates.First(x => x.TargetPlayerId == playerId).SetVote(suspectIdx);
            }
        }

        if (PlayerControl.LocalPlayer.PlayerId != playerId)
        {
            return;
        }

        SoundManager.Instance.PlaySound(__instance.VoteLockinSound, false);

        foreach (var playerVoteArea in __instance.playerStates)
        {
            playerVoteArea.ClearButtons();
        }

        __instance.SkipVoteButton.ClearButtons();

        var localVoteData = PlayerControl.LocalPlayer.GetModifier<VoteData>();
        if (localVoteData != null && localVoteData.VotesRemaining == 0)
        {
            __instance.SkipVoteButton.voteComplete = true;
            __instance.SkipVoteButton.gameObject.SetActive(false);
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(MeetingHud.Confirm))]
    public static bool ConfirmPatch(MeetingHud __instance, [HarmonyArgument(0)] byte suspect)
    {
        __instance.CmdCastVote(PlayerControl.LocalPlayer.PlayerId, suspect);

        return false;
    }
}