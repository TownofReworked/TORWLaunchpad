using LaunchpadReloaded.API.Roles;
using LaunchpadReloaded.Features.Managers;
using LaunchpadReloaded.Features.Translations;
using System.Globalization;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace LaunchpadReloaded.Utilities;

public static class Helpers
{
    public static readonly Random Random = new();

    public static bool ShouldCancelClick()
    {
        return (DragManager.Instance is not null && DragManager.Instance.IsDragging(PlayerControl.LocalPlayer.PlayerId));
    }

    public static string FirstLetterToUpper(string str)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
    }

    public static DeadBody GetBodyById(byte id)
    {
        return Object.FindObjectsOfType<DeadBody>().FirstOrDefault(body => body.ParentId == id);
    }

    public static TextMeshPro CreateTextLabel(string name, Transform parent,
        AspectPosition.EdgeAlignments alignment, Vector3 distance, float fontSize = 2f,
        TextAlignmentOptions textAlignment = TextAlignmentOptions.Center)
    {
        var textObj = new GameObject(name)
        {
            transform =
            {
                parent = parent
            },
            layer = LayerMask.NameToLayer("UI")
        };

        var pingTracker = HudManager.Instance.gameObject.GetComponentInChildren<PingTracker>();
        var textMeshPro = textObj.AddComponent<TextMeshPro>();
        textMeshPro.fontSize = fontSize;
        textMeshPro.alignment = textAlignment;
        textMeshPro.fontMaterial = pingTracker.text.fontMaterial;

        var aspectPosition = textObj.AddComponent<AspectPosition>();
        aspectPosition.Alignment = alignment;
        aspectPosition.DistanceFromEdge = distance;
        aspectPosition.AdjustPosition();

        return textMeshPro;
    }

    public static string RandomString(int length, string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")
    {
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[UnityEngine.Random.RandomRangeInt(0, s.Length)]).ToArray());
    }

    public static string GetSuffix(NumberSuffixes suffix)
    {
        return suffix switch
        {
            NumberSuffixes.None => string.Empty,
            NumberSuffixes.Multiplier => "x",
            NumberSuffixes.Seconds => "s",
            _ => string.Empty
        };
    }

    public static void SendNotification(string text, Color textColor, float duration = 2f, float fontSize = 3f)
    {
        var notifs = HudManager.Instance.Notifier.transform.parent.FindChild("LaunchpadNotifications").GetComponent<NotificationPopper>();
        notifs.TextArea.text = text;
        notifs.TextArea.fontSize = fontSize;
        notifs.textColor = textColor;
        notifs.alphaTimer = duration;
    }

    public static PlainShipRoom GetRoom(Vector3 pos)
    {
        return ShipStatus.Instance.AllRooms.ToList().Find(room => room.roomArea.OverlapPoint(pos));
    }

    public static StringBuilder CreateForRole(ICustomRole role)
    {
        var taskStringBuilder = new StringBuilder();
        taskStringBuilder.Append($"{role.RoleColor.ToTextColor()}");
        taskStringBuilder.AppendLine(TranslationController.Instance.GetString((StringNames)TranslationStringNames.RoleTabTitle, new Il2CppSystem.Object[]
        {
            TranslationController.Instance.GetString((StringNames)role.RoleName)
        }));

        taskStringBuilder.Append("</color><size=70%>");
        taskStringBuilder.AppendLine($"{LaunchpadTranslator.Instance.GetLocaleString(role.RoleLongDescription)}");
        return taskStringBuilder;
    }
}