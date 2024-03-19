namespace LaunchpadReloaded.Networking;
public enum LaunchpadRPC : uint
{
    HackPlayer = 0,
    UnHackPlayer = 1,
    CreateNodes = 2,
    ToggleNode = 4,
    StartDrag = 5,
    StopDrag = 6,
    RemoveBody = 7,
    HideBodyInVent = 8,
    ExposeBody = 9,
    SetGameMode = 10,
    SetBodyType = 11,
    CreateScanner = 12,
    Revive = 13,
    SyncGradient = 14,
    SyncGameOptions = 15,
    SyncWeaponPos = 16
}