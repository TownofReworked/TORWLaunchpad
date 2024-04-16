using LaunchpadReloaded.API.Roles;
using LaunchpadReloaded.Features.Translations;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LaunchpadReloaded.API.GameOptions;
public class CustomOptionGroup
{
    public TranslationStringNames Title { get; }
    public Func<bool> Hidden { get; set; }

    public GameObject Header;
    public Color Color;
    public Type AdvancedRole { get; set; }

    public readonly List<AbstractGameOption> Options = [];
    public readonly List<CustomNumberOption> CustomNumberOptions;
    public readonly List<CustomToggleOption> CustomToggleOptions;
    public readonly List<CustomStringOption> CustomStringOptions;
    public CustomOptionGroup(TranslationStringNames title, List<CustomNumberOption> numberOpt,
        List<CustomToggleOption> toggleOpt, List<CustomStringOption> stringOpt, Type role = null)
    {
        Title = title;

        if (role is not null && role.IsAssignableTo(typeof(ICustomRole)))
        {
            AdvancedRole = role;
        }


        Hidden = () => false;
        CustomNumberOptions = numberOpt;
        CustomToggleOptions = toggleOpt;
        CustomStringOptions = stringOpt;

        Options.AddRange(CustomNumberOptions);
        Options.AddRange(CustomToggleOptions);
        Options.AddRange(CustomStringOptions);

        foreach (var option in Options)
        {
            option.Group = this;
        }

        CustomOptionsManager.CustomGroups.Add(this);
    }

    public void SetColor(Color color)
    {
        this.Color = color;
    }
}