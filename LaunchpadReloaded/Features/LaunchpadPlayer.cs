using LaunchpadReloaded.Networking;
using LaunchpadReloaded.Roles;
using LaunchpadReloaded.Utilities;
using Reactor.Utilities.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LaunchpadReloaded.Features;

[RegisterInIl2Cpp]
public class LaunchpadPlayer(IntPtr ptr) : MonoBehaviour(ptr)
{
    public Transform knife;

    public PlayerControl player;

    public CustomVoteData VoteData;
    public DeadPlayerData DeadData;
    public bool ShowFootsteps = false;
    public static LaunchpadPlayer LocalPlayer { get; private set; }
    public static LaunchpadPlayer GetById(byte id) => GameData.Instance.GetPlayerById(id).Object.GetLpPlayer();
    public static IEnumerable<LaunchpadPlayer> GetAllPlayers() => PlayerControl.AllPlayerControls.ToArray().Select(player => player.GetLpPlayer());
    public static IEnumerable<LaunchpadPlayer> GetAllAlivePlayers() => GetAllPlayers().Where(plr => !plr.player.Data.IsDead && !plr.player.Data.Disconnected);
    private Vector3 lastPos;

    private void Awake()
    {
        player = gameObject.GetComponent<PlayerControl>();
        VoteData = new CustomVoteData();
        DeadData = new DeadPlayerData();

        if (player.AmOwner)
        {
            LocalPlayer = this;
        }

        lastPos = transform.position;
    }

    private void Update()
    {
        if (LocalPlayer.ShowFootsteps && PlayerControl.LocalPlayer.Data.Role is DetectiveRole)
        {
            if (Vector3.Distance(lastPos, transform.position) > 1)
            {
                var angle = Mathf.Atan2(player.MyPhysics.Velocity.y, player.MyPhysics.Velocity.x) * Mathf.Rad2Deg;

                var footstep = new GameObject("Footstep")
                {
                    transform =
                    {
                        parent = ShipStatus.Instance.transform,
                        position = new Vector3(transform.position.x, transform.position.y, 2.5708f),
                        rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward)
                    }
                };

                var sprite = footstep.AddComponent<SpriteRenderer>();
                sprite.sprite = LaunchpadAssets.Footstep.LoadAsset();
                sprite.material = LaunchpadAssets.GradientMaterial.LoadAsset();
                footstep.layer = LayerMask.NameToLayer("Players");

                sprite.transform.localScale = new Vector3(0.06f, 0.06f, 0.06f);
                player.SetPlayerMaterialColors(sprite);

                lastPos = transform.position;
                Destroy(footstep, DetectiveRole.FootstepsDuration.Value);
            }
        }
    }

    public void OnDeath(PlayerControl killer)
    {
        if (player.Data.IsHacked() && player.AmOwner)
        {
            player.RpcUnHackPlayer();
        }

        DeadData.DeathTime = DateTime.Now;
        DeadData.Killer = killer;
        DeadData.Suspects = PlayerControl.AllPlayerControls.ToArray().Where((pc) => pc != DeadData.Killer && !pc.Data.IsDead).Take(4).ToList();
    }

    private void FixedUpdate()
    {
        if (MeetingHud.Instance)
        {
            return;
        }

        if (player.IsRevived())
        {
            player.cosmetics.currentBodySprite.BodySprite.material.SetColor(ShaderID.VisorColor, LaunchpadPalette.RevivedVisorColor);
        }

        if (player.Data.IsHacked())
        {
            var randomString = Helpers.RandomString(Helpers.Random.Next(4, 6), "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@#!?$(???#@)$@@@@0000");
            player.cosmetics.SetName(randomString);
            player.cosmetics.SetNameMask(true);
            player.cosmetics.gameObject.SetActive(false);
        }

        if (knife is null)
        {
            knife = player.gameObject.transform.FindChild("BodyForms/Seeker/KnifeHand");
            return;
        }

        knife.gameObject.SetActive(!player.Data.IsDead && player.CanMove);
    }

    public struct CustomVoteData()
    {
        public readonly List<byte> VotedPlayers = [];
        public int VotesRemaining = (int)LaunchpadGameOptions.Instance.MaxVotes.Value;
        public bool DidSkip = false;
    }

    public struct DeadPlayerData()
    {
        public DateTime DeathTime;
        public PlayerControl Killer;
        public List<PlayerControl> Suspects;
    }
}