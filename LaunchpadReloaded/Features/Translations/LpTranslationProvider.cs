using Reactor.Localization;
using Reactor.Utilities;
using System;

namespace LaunchpadReloaded.Features.Translations;
public class LpTranslationProvider : LocalizationProvider
{
    public override int Priority => ReactorPriority.VeryHigh;

    public override bool TryGetText(StringNames stringName, out string? result)
    {
        if (Enum.TryParse(stringName.ToString(), out TranslationStringNames translated))
        {
            if (LaunchpadTranslator.Instance.TryGetString(CurrentLanguage.Value, translated, out string value))
            {
                result = value;
                return true;
            }
            else
            {
                if (LaunchpadTranslator.Instance.TryGetString(SupportedLangs.English, translated, out string str))
                {
                    result = str;
                    return true;
                }
            }
        }

        result = null;
        return false;
    }
}