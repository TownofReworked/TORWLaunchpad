using AmongUs.Data;
using HarmonyLib;
using LaunchpadReloaded.Features;
using LaunchpadReloaded.Features.Managers;
using LaunchpadReloaded.Features.Translations;
using LaunchpadReloaded.Networking;
using LaunchpadReloaded.Utilities;
using Reactor.Networking.Rpc;
using Reactor.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace LaunchpadReloaded.Patches.Colors.Gradients;

[HarmonyPatch(typeof(PlayerTab))]
public static class PlayerTabPatches
{
    public static bool SelectGradient;
    private static ColorChip _switchButton;
    private static TextMeshPro _buttonText;
    private static TextMeshPro _titleText;

    private static void SwitchSelector(PlayerTab instance)
    {
        SelectGradient = !SelectGradient;
        instance.currentColor = SelectGradient ? GradientManager.LocalGradientId : DataManager.Player.Customization.Color;
    }


    [HarmonyPostfix]
    [HarmonyPatch("OnEnable")]
    public static void OnEnablePostfix(PlayerTab __instance)
    {
        if (!_switchButton)
        {
            _titleText = __instance.transform.FindChild("Text").GetComponent<TextMeshPro>();
            _switchButton = Object.Instantiate(__instance.ColorTabPrefab, __instance.ColorTabArea);

            var spriteRenderer = _switchButton.GetComponent<SpriteRenderer>();
            var sprite = spriteRenderer.sprite = LaunchpadAssets.BlankButton.LoadAsset();

            _switchButton.GetComponent<BoxCollider2D>().size = sprite.rect.size / sprite.pixelsPerUnit;
            _switchButton.transform.localScale = new Vector3(1, 1, 1);
            _switchButton.transform.localPosition = new Vector3(2, 1.5f, -2);

            var buttonText = Object.Instantiate(__instance.transform.Find("Text").gameObject, _switchButton.transform);
            buttonText.transform.localPosition = new Vector3(0, 0, 0);
            buttonText.GetComponent<TextTranslatorTMP>().Destroy();

            _buttonText = buttonText.GetComponent<TextMeshPro>();
            _buttonText.alignment = TextAlignmentOptions.Center;
            _buttonText.richText = true;
            _buttonText.fontSize = _buttonText.fontSizeMax = 2.8f;

            _switchButton.Button.OnClick.RemoveAllListeners();
            _switchButton.Button.OnMouseOut.RemoveAllListeners();
            _switchButton.Button.OnMouseOver.RemoveAllListeners();
            _switchButton.Button.OnClick.AddListener((UnityAction)(() => { SwitchSelector(__instance); }));
        }

        foreach (var colorChip in __instance.ColorChips)
        {
            colorChip.Button.OnMouseOut.RemoveAllListeners();
            colorChip.Button.OnMouseOut.AddListener((UnityAction)(() =>
            {
                __instance.SelectColor(SelectGradient ? GradientManager.LocalGradientId : DataManager.Player.Customization.Color);
            }));
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(PlayerTab.ClickEquip))]
    public static bool ClickPrefix(PlayerTab __instance)
    {
        if (SelectGradient && __instance.AvailableColors.Remove(__instance.currentColor))
        {
            GradientManager.LocalGradientId = __instance.currentColor;
            __instance.PlayerPreview.UpdateFromDataManager(PlayerMaterial.MaskType.None);
            if (__instance.HasLocalPlayer())
            {
                Rpc<CustomCheckColorRpc>.Instance.SendTo(AmongUsClient.Instance.HostId,
                    new CustomCheckColorRpc.Data(
                        (byte)PlayerControl.LocalPlayer.Data.DefaultOutfit.ColorId,
                        (byte)__instance.currentColor));
            }
            return false;
        }

        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(PlayerTab.SelectColor))]
    public static bool SelectPrefix(PlayerTab __instance, [HarmonyArgument(0)] int colorId)
    {
        if (SelectGradient)
        {
            __instance.UpdateAvailableColors();
            __instance.currentColor = colorId;
            var colorName = Palette.GetColorName(colorId);
            PlayerCustomizationMenu.Instance.SetItemName(colorName);
            __instance.PlayerPreview.UpdateFromDataManager(PlayerMaterial.MaskType.None);
            return false;
        }

        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(PlayerTab.GetCurrentColorId))]
    public static bool GetCurrentColorPrefix(PlayerTab __instance, ref int __result)
    {
        if (SelectGradient)
        {
            __result = GradientManager.LocalGradientId;
            return false;
        }

        return true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(PlayerTab.Update))]
    public static void UpdatePostfix(PlayerTab __instance)
    {
        if (_buttonText && _titleText)
        {
            _buttonText.text = SelectGradient ? TranslationController.Instance.GetString((StringNames)TranslationStringNames.MainColor) :
                TranslationController.Instance.GetString((StringNames)TranslationStringNames.SecondaryColor);
            _titleText.text = SelectGradient ? TranslationController.Instance.GetString((StringNames)TranslationStringNames.SecondaryColor) :
            TranslationController.Instance.GetString((StringNames)TranslationStringNames.MainColor);
        }

        if (SelectGradient)
        {
            __instance.currentColorIsEquipped = __instance.currentColor == GradientManager.LocalGradientId;
        }

        var mat = __instance.PlayerPreview.cosmetics.currentBodySprite.BodySprite.material;

        mat.SetFloat(ShaderID.GradientBlend, 2);
        mat.SetFloat(ShaderID.GradientOffset, .4f);



    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(PlayerTab.UpdateAvailableColors))]
    public static bool UpdateColorsPrefix(PlayerTab __instance)
    {
        for (var i = 0; i < Palette.PlayerColors.Length; i++)
        {
            __instance.AvailableColors.Add(i);
        }
        if (!LaunchpadGameOptions.Instance.UniqueColors.Value)
        {
            return false;
        }

        if (!GameData.Instance)
        {
            return false;
        }

        var allPlayers = GameData.Instance.AllPlayers;
        var localGradId = GradientManager.LocalGradientId;
        var localColorId = PlayerControl.LocalPlayer.Data.DefaultOutfit.ColorId;

        foreach (var data in allPlayers)
        {
            if (!GradientManager.TryGetColor(data.PlayerId, out var gradColor))
            {
                continue;
            }

            // TODO: simplify logic
            if (SelectGradient)
            {
                var isSame = data.DefaultOutfit.ColorId == localColorId;
                var isOpposite = gradColor == localColorId;
                if (isSame)
                {
                    __instance.AvailableColors.Remove(gradColor);
                }

                if (isOpposite)
                {
                    __instance.AvailableColors.Remove(data.DefaultOutfit.ColorId);
                }
            }
            else
            {
                var isSame = gradColor == localGradId;
                var isOpposite = data.DefaultOutfit.ColorId == localGradId;
                if (isSame)
                {
                    __instance.AvailableColors.Remove(data.DefaultOutfit.ColorId);
                }

                if (isOpposite)
                {
                    __instance.AvailableColors.Remove(gradColor);
                }

            }
        }

        return false;
    }
}