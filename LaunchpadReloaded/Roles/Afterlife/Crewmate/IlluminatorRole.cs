using Il2CppSystem.Text;
using LaunchpadReloaded.Components;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Roles.Afterlife;
using MiraAPI.Roles;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LaunchpadReloaded.Roles.Crewmate;

public class IlluminatorRole(IntPtr ptr) : CrewmateGhostRole(ptr), ICustomRole, IAfterlifeRole
{
    public string RoleName => "Illuminator";
    public string RoleDescription => "Place lanterns to help the crew.";
    public string RoleLongDescription => RoleDescription;
    public Color RoleColor => LaunchpadPalette.IlluminatorRole;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;

    public RoleOptionsGroup RoleOptionsGroup { get; } = LaunchpadConstants.AfterLifeCrewGroup;

    public List<OgLightSource> PlacedLanterns = new();

    public override bool IsDead => true;
    public override bool IsAffectedByComms => CommsSabotaged;

    public override void AppendTaskHint(StringBuilder taskStringBuilder)
    {
        if (HudManager.InstanceExists && HudManager.Instance.ShadowQuad.gameObject.active != true)
        {
            HudManager.Instance.ShadowQuad.gameObject.SetActive(true);
        }
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        if (HudManager.InstanceExists && HudManager.Instance.ShadowQuad.gameObject.active == true)
        {
            HudManager.Instance.ShadowQuad.gameObject.SetActive(false);
        }
    }
}