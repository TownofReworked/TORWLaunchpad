using BepInEx.Configuration;
using LaunchpadReloaded.Features.Translations;
using Reactor.Localization.Utilities;
using Reactor.Utilities;
using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LaunchpadReloaded.API.GameOptions;

public class CustomStringOption : AbstractGameOption
{
    public int IndexValue { get; private set; }
    public string Value => Options[IndexValue];
    public int Default { get; }
    public string[] Options { get; private set; }
    public ConfigEntry<int> Config { get; }
    public Action<int> ChangedEvent { get; init; }
    public CustomStringOption(TranslationStringNames title, int defaultValue, string[] options, Type role = null, bool save = true) : base(title, role, save)
    {
        IndexValue = defaultValue;
        Options = options;
        Default = defaultValue;

        CustomOptionsManager.CustomStringOptions.Add(this);
        if (Save)
        {
            try
            {
                Config = LaunchpadReloadedPlugin.Instance.Config.Bind("String Options", LaunchpadTranslator.Instance.GetString(SupportedLangs.English, title), defaultValue);
            }
            catch (Exception e)
            {
                Logger<LaunchpadReloadedPlugin>.Warning(e.ToString());
            }
        }

        SetValue(Save ? Config.Value : defaultValue);
    }

    public void SetValue(int newValue)
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

        var oldValue = IndexValue;
        IndexValue = newValue;

        var behaviour = (StringOption)OptionBehaviour;
        if (behaviour)
        {
            behaviour.Value = newValue;
        }

        if (oldValue != newValue)
        {
            ChangedEvent?.Invoke(newValue);
        }
    }

    public void SetValue(string newValue) => SetValue(Options.ToList().IndexOf(newValue));

    protected override void OnValueChanged(OptionBehaviour optionBehaviour)
    {
        SetValue(optionBehaviour.GetInt());
    }

    public StringOption CreateStringOption(StringOption original, Transform container)
    {
        var stringOption = Object.Instantiate(original, container);
        stringOption.Title = (StringNames)Title;
        stringOption.Value = Options.ToList().IndexOf(Value);
        stringOption.Values = Options.Select(CustomStringName.CreateAndRegister).ToArray();
        stringOption.OnValueChanged = (Il2CppSystem.Action<OptionBehaviour>)ValueChanged;
        stringOption.OnEnable();

        OptionBehaviour = stringOption;

        return stringOption;
    }
}