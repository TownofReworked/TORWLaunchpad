using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace LaunchpadReloaded.Features.Translations;
public class LaunchpadTranslator
{
    public static LaunchpadTranslator Instance;
    public Dictionary<SupportedLangs, Dictionary<StringNames, string>> ParsedLanguages = new();

    public LaunchpadTranslator()
    {
        Instance = this;

        List<string> fileNames = Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(
            name => name.StartsWith("LaunchpadReloaded.Resources.Translations") && name.EndsWith(".json")).ToList();

        foreach (var file in fileNames)
        {
            string fileName = file.Replace("LaunchpadReloaded.Resources.Translations.", "").Replace(".json", "");
            if (!Enum.TryParse(fileName, out SupportedLangs lang)) continue;

            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(file))
            using (StreamReader reader = new StreamReader(stream))
            {
                string text = reader.ReadToEnd();

                Dictionary<string, string> dict = JsonSerializer.Deserialize<Dictionary<string, string>>(text);
                Dictionary<StringNames, string> newDict = new Dictionary<StringNames, string>();
                foreach (KeyValuePair<string, string> kvp in dict)
                {
                    if (Enum.TryParse(kvp.Key, out TranslationStringNames translated))
                    {
                        newDict.Add((StringNames)translated, kvp.Value);
                    }
                }

                ParsedLanguages.TryAdd(lang, newDict);
            }
        }
    }

    public string GetLocaleString(TranslationStringNames stringName)
    {
        if (TryGetString(TranslationController.Instance.currentLanguage.languageID, stringName, out string value))
        {
            return value;
        }
        else
        {
            return GetString(SupportedLangs.English, stringName);
        }
    }

    public bool TryGetString(SupportedLangs lang, TranslationStringNames stringName, out string result)
    {
        var dict = ParsedLanguages[lang];
        if (dict.TryGetValue((StringNames)stringName, out string value))
        {
            result = value;
            return true;
        }

        result = null;
        return false;
    }

    public string GetString(SupportedLangs lang, TranslationStringNames stringName)
    {
        var dict = ParsedLanguages[lang];
        if (dict == null) return string.Empty;

        if (dict.TryGetValue((StringNames)stringName, out string val))
        {
            return val;
        }
        else
        {
            return string.Empty;
        }
    }
}