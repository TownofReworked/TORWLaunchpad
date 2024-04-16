using LaunchpadReloaded.API.Roles;
using LaunchpadReloaded.Features.Translations;
using System;

namespace LaunchpadReloaded.API.GameOptions;

public abstract class AbstractGameOption
{
    public TranslationStringNames Title { get; }
    public Type AdvancedRole { get; }
    public bool Save { get; }
    public bool ShowInHideNSeek { get; init; }
    public CustomOptionGroup Group { get; set; }
    public Func<bool> Hidden { get; set; }
    public OptionBehaviour OptionBehaviour { get; protected set; }
    public void ValueChanged(OptionBehaviour optionBehaviour)
    {
        OnValueChanged(optionBehaviour);
        CustomOptionsManager.SyncOptions();
    }

    protected abstract void OnValueChanged(OptionBehaviour optionBehaviour);

    protected AbstractGameOption(TranslationStringNames title, Type roleType, bool save)
    {
        Title = title;
        if (roleType is not null && roleType.IsAssignableTo(typeof(ICustomRole)))
        {
            AdvancedRole = roleType;
        }

        Save = save;
        Hidden = () => false;
        CustomOptionsManager.CustomOptions.Add(this);
    }
}