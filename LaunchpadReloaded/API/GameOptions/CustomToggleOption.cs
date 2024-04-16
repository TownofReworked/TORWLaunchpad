using BepInEx.Configuration;
using LaunchpadReloaded.Features.Translations;
using Reactor.Utilities;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LaunchpadReloaded.API.GameOptions;

public class CustomToggleOption : AbstractGameOption
{
    public bool Value { get; private set; }
    public bool Default { get; }
    public ConfigEntry<bool> Config { get; }
    public Action<bool> ChangedEvent { get; init; }
    public CustomToggleOption(TranslationStringNames title, bool defaultValue, Type role = null, bool save = true) : base(title, role, save)
    {
        Default = defaultValue;
        if (Save)
        {
            try
            {
                Config = LaunchpadReloadedPlugin.Instance.Config.Bind("Toggle Options", LaunchpadTranslator.Instance.GetString(SupportedLangs.English, Title), defaultValue);
            }
            catch (Exception e)
            {
                Logger<LaunchpadReloadedPlugin>.Warning(e.ToString());
            }
        }
        CustomOptionsManager.CustomToggleOptions.Add(this);
        SetValue(Save ? Config.Value : defaultValue);
    }

    public void SetValue(bool newValue)
    {
        if (Save)
        {
            try
            {
                Config.Value = newValue;
            }
            catch (Exception e)
            {
                Logger<LaunchpadReloadedPlugin>.Warning(e.ToString());
            }
        }

        var oldValue = Value;
        Value = newValue;

        var behaviour = (ToggleOption)OptionBehaviour;
        if (behaviour)
        {
            behaviour.CheckMark.enabled = newValue;
        }

        if (newValue != oldValue)
        {
            ChangedEvent?.Invoke(newValue);
        }
    }

    protected override void OnValueChanged(OptionBehaviour optionBehaviour)
    {
        SetValue(optionBehaviour.GetBool());
    }

    public ToggleOption CreateToggleOption(ToggleOption original, Transform container)
    {
        var toggleOption = Object.Instantiate(original, container);

        toggleOption.Title = (StringNames)Title;
        toggleOption.CheckMark.enabled = Value;
        toggleOption.OnValueChanged = (Il2CppSystem.Action<OptionBehaviour>)ValueChanged;
        toggleOption.OnEnable();
        OptionBehaviour = toggleOption;
        return toggleOption;
    }
}