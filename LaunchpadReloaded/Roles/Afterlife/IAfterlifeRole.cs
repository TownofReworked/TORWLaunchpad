using MiraAPI.Roles;

namespace LaunchpadReloaded.Roles.Afterlife;

public interface IAfterlifeRole : ICustomRole
{
    CustomRoleConfiguration ICustomRole.Configuration => new(this)
    {
        MaxRoleCount = 1,
        DefaultRoleCount = 1,
        HideSettings = false,
        ShowInFreeplay = true,
    };

    public bool CanBeAssigned(PlayerControl player)
    {
        return true;
    }
}
