using LaunchpadReloaded.Modifiers;
using LaunchpadReloaded.Options;
using LaunchpadReloaded.Utilities;
using MiraAPI.GameOptions;
using Reactor.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace LaunchpadReloaded.Features.Voting;
public static class VotingTypesManager
{
    public static VotingTypes SelectedType => OptionGroupSingleton<VotingOptions>.Instance.VotingType;

    public static readonly byte[] RecommendedVotes =
    [
        1, 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5, 5, 5
    ];

    public static int GetDynamicVotes() =>
        (int)Math.Min(
            RecommendedVotes[
                Math.Min(
                    Math.Clamp(
                        Helpers.GetAlivePlayers().Count,
                        0,
                        15),
                    RecommendedVotes.Length)],
            OptionGroupSingleton<VotingOptions>.Instance.MaxVotes.Value);

    public static int GetVotes()
    {
        switch (SelectedType)
        {
            case VotingTypes.Combined:
            case VotingTypes.Multiple:
                return GetDynamicVotes();

            case VotingTypes.Chance:
            case VotingTypes.Classic:
            default:
                return 1;
        }
    }

    #region Vote calculations
    public static List<CustomVote> CalculateVotes()
    {
        return (from player in Helpers.GetAlivePlayers()
                from vote in MiraAPI.Utilities.Extensions.GetModifier<VoteData>(player)!.VotedPlayers
                select new CustomVote(player.PlayerId, vote)).ToList();
    }

    public static Dictionary<byte, float> GetChancePercents(List<CustomVote> votes)
    {
        var dict = new Dictionary<byte, float>();

        foreach (var pair in CalculateNumVotes(votes))
        {
            dict[pair.Key] = (float)pair.Value / votes.Count * 100;
        }

        return dict;
    }

    public static byte GetVotedPlayerByChance(List<CustomVote> votes)
    {
        if (!votes.Any())
        {
            return (byte)SpecialVotes.Skip;
        }

        var rand = new Random();
        List<byte> plrs = [.. votes.Select(vote => vote.Suspect)];
        return plrs[rand.Next(plrs.Count)];
    }

    public static Dictionary<byte, int> CalculateNumVotes(IEnumerable<CustomVote> votes)
    {
        var dictionary = new Dictionary<byte, int>();

        foreach (var vote in votes)
        {
            if (!dictionary.TryAdd(vote.Suspect, 1))
            {
                dictionary[vote.Suspect] += 1;
            }
        }

        return dictionary;
    }

    #endregion

    #region Populate results 
    public static void HandlePopulateResults(List<CustomVote> votes)
    {
        MeetingHud.Instance.TitleText.text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.MeetingVotingResults, Il2CppSystem.Array.Empty<Il2CppSystem.Object>());

        if (!OptionGroupSingleton<VotingOptions>.Instance.HideVotingIcons.Value)
        {
            var delays = new Dictionary<byte, int>();
            var num = 0;

            for (var i = 0; i < MeetingHud.Instance.playerStates.Length; i++)
            {
                var playerVoteArea = MeetingHud.Instance.playerStates[i];
                playerVoteArea.ClearForResults();
                foreach (var vote in votes)
                {
                    var playerById = GameData.Instance.GetPlayerById(vote.Voter);
                    if (playerById == null)
                    {
                        Logger<LaunchpadReloadedPlugin>.Error($"Couldn't find player info for voter: {vote.Voter}");
                    }
                    else if (i == 0 && vote.Suspect == (byte)SpecialVotes.Skip)
                    {
                        MeetingHud.Instance.BloopAVoteIcon(playerById, num, MeetingHud.Instance.SkippedVoting.transform);
                        num++;
                    }
                    else if (vote.Suspect == playerVoteArea.TargetPlayerId)
                    {
                        if (!delays.TryAdd(vote.Suspect, 0))
                        {
                            delays[vote.Suspect]++;
                        }
                        MeetingHud.Instance.BloopAVoteIcon(playerById, delays[vote.Suspect], playerVoteArea.transform);
                    }
                }
            }
        }

        if (!UseChance() && !OptionGroupSingleton<VotingOptions>.Instance.ShowPercentages.Value)
        {
            return;
        }

        var chances = GetChancePercents(votes);

        var skipText = MeetingHud.Instance.SkippedVoting;
        skipText.GetComponentInChildren<TextTranslatorTMP>().Destroy();

        chances.TryGetValue((byte)SpecialVotes.Skip, out var skips);
        skipText.GetComponentInChildren<TextMeshPro>().text += "\n<size=110%>" + Math.Round(skips, 0) + "%</size>";

        foreach (var voteArea in MeetingHud.Instance.playerStates)
        {
            chances.TryGetValue(voteArea.TargetPlayerId, out var val);
            if (voteArea.AmDead || val < 1)
            {
                continue;
            }

            var text = $"{Math.Round(val, 0)}%";
            var chanceThing = Object.Instantiate(voteArea.LevelNumberText.transform.parent, voteArea.transform).gameObject;
            chanceThing.gameObject.name = "ChanceCircle";
            chanceThing.transform.localPosition = new Vector3(1.2801f, -0.2431f, -2.5401f);
            chanceThing.transform.localScale = new Vector3(0.35f, 0.35f, 1);
            chanceThing.transform.GetChild(0).gameObject.SetActive(false);
            chanceThing.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);

            var tmp = chanceThing.transform.GetChild(1).gameObject.GetComponent<TextMeshPro>();
            tmp.fontSize = 3f;
            tmp.text = text;
            tmp.transform.localPosition = new Vector3(0, 0, 0);
        }
    }
    #endregion

    public static bool UseChance() => SelectedType == VotingTypes.Chance || SelectedType == VotingTypes.Combined;
    public static bool CanVoteMultiple() => SelectedType is VotingTypes.Multiple or VotingTypes.Combined;
}