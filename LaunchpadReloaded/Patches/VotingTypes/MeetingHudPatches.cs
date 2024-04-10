using HarmonyLib;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Features.Managers;
using LaunchpadReloaded.Networking;
using LaunchpadReloaded.Roles;
using LaunchpadReloaded.Utilities;
using Reactor.Networking.Rpc;
using Reactor.Utilities.Extensions;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
namespace LaunchpadReloaded.Patches.VotingTypes;

[HarmonyPatch]
public static class MeetingHudPatches
{
    private static GameObject _typeText;
    private static PlayerVoteArea _confirmVotes;

    [HarmonyPostfix, HarmonyPatch(typeof(MeetingHud), "Start")]
    public static void AwakePostfix(MeetingHud __instance)
    {
        foreach (var plr in LaunchpadPlayer.GetAllPlayers())
        {
            plr.VoteData.VotesRemaining = VotingTypesManager.GetMaxVotes();
            plr.VoteData.VotedPlayers.Clear();

            if (plr.player.Data.Role is MayorRole)
            {
                plr.VoteData.VotesRemaining += (int)MayorRole.ExtraVotes.Value;
            }
        }

        DragManager.Instance.DraggingPlayers.Clear();

        if (_typeText == null)
        {
            _typeText = Object.Instantiate(__instance.TimerText, __instance.TimerText.transform.parent).gameObject;
            _typeText.GetComponent<TextTranslatorTMP>().Destroy();
            _typeText.GetComponent<TextMeshPro>().alignment = TextAlignmentOptions.Left;
            _typeText.transform.localPosition = new Vector3(-1.4327f, -2.1964f, 0);
            _typeText.name = "VoteTypeText";
            _typeText.gameObject.SetActive(false);
        }

        if (_confirmVotes == null && (VotingTypesManager.CanVoteMultiple() || LaunchpadGameOptions.Instance.AllowConfirmingVotes.Value))
        {
            _confirmVotes = Object.Instantiate(__instance.SkipVoteButton, __instance.SkipVoteButton.transform.parent);
            _confirmVotes.gameObject.name = "ConfirmVotesBtn";
            _confirmVotes.SetTargetPlayerId(250);
            _confirmVotes.Parent = __instance;

            var confirmText = _confirmVotes.gameObject.GetComponentInChildren<TextMeshPro>();
            _confirmVotes.gameObject.GetComponentInChildren<TextTranslatorTMP>().Destroy();
            confirmText.text = "CONFIRM VOTES";
            __instance.SkipVoteButton.transform.position += new Vector3(0, 0.18f, 0);
            _confirmVotes.transform.position -= new Vector3(0, 0.1f, 0);
            foreach (var plr in __instance.playerStates.AddItem(__instance.SkipVoteButton))
            {
                plr.gameObject.GetComponentInChildren<PassiveButton>().OnClick.AddListener((UnityAction)delegate { _confirmVotes.ClearButtons(); });
            }
        }
        else if (_confirmVotes != null && !VotingTypesManager.CanVoteMultiple() && !LaunchpadGameOptions.Instance.AllowConfirmingVotes.Value) _confirmVotes.gameObject.Destroy();
    }

    [HarmonyPostfix, HarmonyPatch(typeof(MeetingHud), "Update")]
    public static void UpdatePatch(MeetingHud __instance)
    {
        if (!_typeText) return;

        var tmp = _typeText.GetComponent<TextMeshPro>();
        tmp.text = VotingTypesManager.SelectedType != Features.Voting.VotingTypes.Classic ? $"<size=160%>{LaunchpadPlayer.LocalPlayer.VoteData.VotesRemaining} votes left</size>\nVoting Type: {VotingTypesManager.SelectedType}" : $"<size=160%>{LaunchpadPlayer.LocalPlayer.VoteData.VotesRemaining} votes left</size>";

        var logicOptionsNormal = GameManager.Instance.LogicOptions.TryCast<LogicOptionsNormal>();

        if (__instance.state == MeetingHud.VoteStates.NotVoted && __instance.discussionTimer >= logicOptionsNormal.GetVotingTime())
        {
            foreach (var plr in LaunchpadPlayer.GetAllAlivePlayers().Where(plr => plr.VoteData.VotesRemaining != 0))
            {
                __instance.CmdCastVote(plr.player.PlayerId, 255);
            }
        }

        if (PlayerControl.LocalPlayer.Data.IsDead)
        {
            if (_confirmVotes) _confirmVotes.SetDisabled();
            _typeText.gameObject.SetActive(false);
            if (__instance.state == MeetingHud.VoteStates.Results)
            {
                foreach (var voteArea in __instance.playerStates.Where(state => !state.resultsShowing)) voteArea.ClearForResults();
            }

            return;
        }

        switch (__instance.state)
        {
            case MeetingHud.VoteStates.Voted:
            case MeetingHud.VoteStates.NotVoted:
                if (LaunchpadPlayer.LocalPlayer.VoteData.VotesRemaining == 0)
                {
                    _typeText.gameObject.SetActive(false);
                    if (_confirmVotes) _confirmVotes.SetDisabled();
                }
                else
                {
                    _typeText.gameObject.SetActive(true);
                    if (_confirmVotes) _confirmVotes.SetEnabled();
                }
                break;

            case MeetingHud.VoteStates.Results:
                if (_confirmVotes) _confirmVotes.SetDisabled();
                _typeText.gameObject.SetActive(false);
                foreach (var voteArea in __instance.playerStates.Where(state => !state.resultsShowing)) voteArea.ClearForResults();
                break;

            default:
                if (_confirmVotes) _confirmVotes.SetDisabled();
                _typeText.gameObject.SetActive(false);
                break;
        }
    }

    [HarmonyPrefix, HarmonyPatch(typeof(MeetingHud), "CheckForEndVoting")]
    public static bool EndCheck(MeetingHud __instance)
    {
        if (LaunchpadPlayer.GetAllAlivePlayers().Any(plr => plr.VoteData.VotesRemaining > 0))
        {
            return false;
        }

        GameData.PlayerInfo exiled;
        bool isTie;

        if (VotingTypesManager.UseChance())
        {
            isTie = false;
            var playerId = VotingTypesManager.GetVotedPlayerByChance(VotingTypesManager.CalculateVotes());
            exiled = GameData.Instance.GetPlayerById(playerId);
        }
        else
        {
            var max = VotingTypesManager.CalculateNumVotes(VotingTypesManager.CalculateVotes()).MaxPair(out isTie);
            exiled = GameData.Instance.AllPlayers.ToArray().FirstOrDefault(v => !isTie && v.PlayerId == max.Key);
        }

        if (exiled is null || exiled.IsDead || exiled.Disconnected)
        {
            exiled = null;
        }
        __instance.RpcVotingComplete(new MeetingHud.VoterState[__instance.playerStates.Length], exiled, isTie);

        return false;
    }

    [HarmonyPrefix, HarmonyPatch(typeof(MeetingHud), "Select")]
    public static bool SelectPatch(MeetingHud __instance, [HarmonyArgument(0)] byte suspect)
    {
        if (LaunchpadGameOptions.Instance.AllowVotingForSamePerson.Value)
        {
            return LaunchpadPlayer.LocalPlayer.VoteData.VotesRemaining > 0;
        }

        return !LaunchpadPlayer.LocalPlayer.VoteData.VotedPlayers.Contains(suspect);
    }

    [HarmonyPrefix, HarmonyPatch(typeof(MeetingHud), "HandleDisconnect", [typeof(PlayerControl), typeof(DisconnectReasons)])]
    public static bool HandleDisconnect(MeetingHud __instance, [HarmonyArgument(0)] PlayerControl pc)
    {
        if (!AmongUsClient.Instance.AmHost || __instance.playerStates is null || !pc || !GameData.Instance) return false;

        var playerVoteArea = __instance.playerStates.First(pv => pv.TargetPlayerId == pc.PlayerId);
        playerVoteArea.AmDead = true;
        playerVoteArea.Overlay.gameObject.SetActive(true);

        foreach (var player in LaunchpadPlayer.GetAllAlivePlayers())
        {
            var pva = __instance.playerStates.First(pv => pv.TargetPlayerId == player.player.PlayerId);

            if (!pva.AmDead && player.VoteData.VotedPlayers.Contains(pc.PlayerId))
            {
                player.VoteData.VotedPlayers.Remove(pc.PlayerId);
                player.VoteData.VotesRemaining += 1;

                VotingRpc.RpcRemoveVote(PlayerControl.LocalPlayer, player.player.PlayerId, pc.PlayerId);
            }
        }

        __instance.SetDirtyBit(1U);
        __instance.CheckForEndVoting();

        if (__instance.state == MeetingHud.VoteStates.Results)
        {
            __instance.SetupProceedButton();
        }

        return false;
    }


    [HarmonyPrefix, HarmonyPatch(typeof(MeetingHud), "PopulateResults")]
    public static bool PopulateResultsPatch(MeetingHud __instance)
    {
        if (!AmongUsClient.Instance.AmHost)
        {
            return false;
        }

        var votes = VotingTypesManager.CalculateVotes();
        
        Rpc<PopulateResultsRpc>.Instance.Send(new PopulateResultsRpc.Data(votes.ToArray()));
        return false;
    }

    public static void HandleVote(LaunchpadPlayer plr, byte suspectIdx)
    {
        if (suspectIdx == 250)
        {
            plr.VoteData.VotesRemaining = 0;
        }
        else if (suspectIdx == 255)
        {
            plr.VoteData.VotesRemaining = 0;
            plr.VoteData.VotedPlayers.Clear();
        }
        else
        {
            plr.VoteData.VotesRemaining--;
            plr.VoteData.VotedPlayers.Add(suspectIdx);
        }
    }

    [HarmonyPrefix, HarmonyPatch(typeof(MeetingHud), "CastVote")]
    public static bool CastVotePatch(MeetingHud __instance, [HarmonyArgument(0)] byte playerId, [HarmonyArgument(1)] byte suspectIdx)
    {
        var plr = LaunchpadPlayer.GetById(playerId);
        if (plr.VoteData.VotesRemaining == 0 ||
            (plr.VoteData.VotedPlayers.Contains(suspectIdx) &&
             !LaunchpadGameOptions.Instance.AllowVotingForSamePerson.Value))
            return false;

        HandleVote(plr, suspectIdx);

        __instance.SetDirtyBit(1U);
        __instance.CheckForEndVoting();

        if (plr.VoteData.VotesRemaining == 0)
        {
            __instance.playerStates[playerId].SetVote(suspectIdx);
        }

        if (suspectIdx != 255) PlayerControl.LocalPlayer.RpcSendChatNote(playerId, ChatNoteTypes.DidVote);
        return false;
    }

    [HarmonyPostfix, HarmonyPatch(typeof(MeetingHud), "CmdCastVote")]
    public static void CmdCastVotePatch(MeetingHud __instance, [HarmonyArgument(0)] byte playerId, [HarmonyArgument(1)] byte suspectIdx)
    {
        
        var plr = LaunchpadPlayer.GetById(playerId);

        if (!AmongUsClient.Instance.AmHost)
        {
            if (plr.VoteData.VotesRemaining == 0 || (plr.VoteData.VotedPlayers.Contains(suspectIdx) && !LaunchpadGameOptions.Instance.AllowVotingForSamePerson.Value)) return;
            HandleVote(plr, suspectIdx);

            if (plr.VoteData.VotesRemaining == 0)
            {
                __instance.playerStates[playerId].SetVote(suspectIdx);
            }
        }

        if (PlayerControl.LocalPlayer.PlayerId == playerId)
        {
            SoundManager.Instance.PlaySound(__instance.VoteLockinSound, false, 1f, null);

            foreach (var playerVoteArea in __instance.playerStates)
            {
                playerVoteArea.ClearButtons();
            }

            __instance.SkipVoteButton.ClearButtons();

            if (LaunchpadPlayer.LocalPlayer.VoteData.VotesRemaining == 0)
            {
                __instance.SkipVoteButton.voteComplete = true;
                __instance.SkipVoteButton.gameObject.SetActive(false);
            }
        }
    }

    [HarmonyPrefix, HarmonyPatch(typeof(MeetingHud), "Confirm")]
    public static bool ConfirmPatch(MeetingHud __instance, [HarmonyArgument(0)] byte suspect)
    {
        __instance.CmdCastVote(PlayerControl.LocalPlayer.PlayerId, suspect);

        return false;
    }
}