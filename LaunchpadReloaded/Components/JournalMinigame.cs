using System;
using System.Collections.Generic;
using System.Linq;
using Cpp2IL.Core.Extensions;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Roles;
using Reactor.Utilities.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Random = System.Random;

[RegisterInIl2Cpp]
public class JournalMinigame(nint ptr) : Minigame(ptr)
{
    public TextMeshPro DeadPlayerInfo;
    public PassiveButton CloseButton;
    public PassiveButton OutsideButton;
    public SpriteRenderer DeadBodyIcon;
    public List<SpriteRenderer> Suspects;

    private void Awake()
    {
        OutsideButton = transform.FindChild("Background/OutsideCloseButton").GetComponent<PassiveButton>();
        CloseButton = transform.FindChild("CloseButton").GetComponent<PassiveButton>();
        DeadPlayerInfo = transform.FindChild("BodyInfo/DeadPlayerInfo").GetComponent<TextMeshPro>();
        DeadBodyIcon = transform.FindChild("BodyInfo/Icon").GetComponent<SpriteRenderer>();

        CloseButton.OnClick.AddListener((UnityAction)(() =>
        {
            Close();
        }));

        OutsideButton.OnClick.AddListener((UnityAction)(() =>
        {
            Close();
        }));

        Suspects = gameObject.transform.FindChild("Suspects").GetComponentsInChildren<SpriteRenderer>().ToList();
    }

    public void Open(LaunchpadPlayer deadPlayer)
    {
        // init body
        TimeSpan timeSinceDeath = DateTime.Now.Subtract(deadPlayer.DeadData.DeathTime);
        DeadPlayerInfo.text = timeSinceDeath.Minutes < 1 ? $"{deadPlayer.player.Data.PlayerName}\n<size=70%>Died {timeSinceDeath.Seconds} seconds ago</size>" :
            $"{deadPlayer.player.Data.PlayerName}\n<size=70%>Died {timeSinceDeath.Minutes} minutes ago</size>";

        deadPlayer.player.SetPlayerMaterialColors(DeadBodyIcon);

        if (LaunchpadPlayer.GetAllAlivePlayers().Count() < 4 || DetectiveRole.HideSuspects.Value)
        {
            gameObject.transform.FindChild("Suspects").gameObject.SetActive(false);
            gameObject.transform.FindChild("SuspectsText").gameObject.SetActive(false);
            gameObject.transform.FindChild("SuspectsTitle").GetComponent<TextMeshPro>().text = "Suspects cannot be shown.";
            Begin(null);
            return;
        }

        Random rand = new Random();
        SpriteRenderer chosenRend = Suspects[rand.Next(Suspects.Count)];
        deadPlayer.DeadData.Killer.SetPlayerMaterialColors(chosenRend);

        List<SpriteRenderer> availableSprites = Suspects;
        availableSprites.Remove(chosenRend);

        List<PlayerControl> sus = deadPlayer.DeadData.Suspects.Clone();

        foreach (SpriteRenderer rend in availableSprites)
        {
            PlayerControl randPlayer = sus[rand.Next(sus.Count)];
            randPlayer.SetPlayerMaterialColors(rend);
            sus.Remove(randPlayer);
        }

        Begin(null);
    }
}