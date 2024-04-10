using Reactor.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using LaunchpadReloaded.Features.Voting;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace LaunchpadReloaded.Features.Managers;
public static class VotingTypesManager
{
    public static VotingTypes SelectedType
    {
        get => (VotingTypes)LaunchpadGameOptions.Instance.VotingType.IndexValue;
        private set => LaunchpadGameOptions.Instance.VotingType.SetValue((int)value);
    }

    private static byte[] RecommendedVotes
    {
        get
        {
            return (VoteLimitType)LaunchpadGameOptions.Instance.VoteLimitType.IndexValue switch
            {
                VoteLimitType.Disabled => [],
                VoteLimitType.Normal => [1, 1, 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5, 5],
                VoteLimitType.Lenient => [1, 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5, 5, 5],
                VoteLimitType.Strict => [1, 1, 1, 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5],
                _ => [1, 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5, 5, 5]
            };
        }
    }

    private static int GetDynamicVotes()
    {
        return 
            (int)Math.Min(
                RecommendedVotes[Math.Clamp(LaunchpadPlayer.GetAllAlivePlayers().Count(), 0, RecommendedVotes.Length)], 
                LaunchpadGameOptions.Instance.MaxVotes.Value);
    }

    public static int GetMaxVotes()
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
        return (from player in LaunchpadPlayer.GetAllAlivePlayers()
                from vote in player.VoteData.VotedPlayers
                select new CustomVote(player.player.PlayerId, vote)).ToList();
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
        if (!votes.Any()) return 253;

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

        if (!LaunchpadGameOptions.Instance.HideVotingIcons.Value)
        {
            var delays = new Dictionary<byte, int>();
            var num2 = 0;
            
            foreach (var vote in votes)
            {
                if (vote.Suspect == 253)
                {
                    MeetingHud.Instance.BloopAVoteIcon(GameData.Instance.GetPlayerById(vote.Voter), num2, MeetingHud.Instance.SkippedVoting.transform);
                    num2++;
                    continue;
                }

                var playerVoteArea = MeetingHud.Instance.playerStates[vote.Suspect];

                if (!delays.TryAdd(vote.Voter, 0))
                {
                    delays[vote.Voter]++;
                }
                
                MeetingHud.Instance.BloopAVoteIcon(GameData.Instance.GetPlayerById(vote.Voter), delays[vote.Voter], playerVoteArea.transform);
            }
        }

        if (!UseChance() && !LaunchpadGameOptions.Instance.ShowPercentages.Value)
        {
            return;
        }

        var chances = GetChancePercents(votes);

        var skipText = MeetingHud.Instance.SkippedVoting;
        skipText.GetComponentInChildren<TextTranslatorTMP>().Destroy();

        chances.TryGetValue(253, out var skips);
        skipText.GetComponentInChildren<TextMeshPro>().text += "\n<size=110%>" + Math.Round(skips, 0) + "%</size>";

        foreach (var voteArea in MeetingHud.Instance.playerStates)
        {
            chances.TryGetValue(voteArea.TargetPlayerId, out var val);
            if (voteArea.AmDead || val < 1) continue;

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