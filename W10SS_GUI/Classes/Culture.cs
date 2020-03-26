using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Windows10SetupScript.Classes
{
    internal static class Culture
    {
        private static string culture = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == CONST.LANG_RU ? CONST.LANG_RU : CONST.LANG_EN;
        private static ResourceDictionary resourceDictionaryEn = new ResourceDictionary() { Source = new Uri("pack://application:,,,/Localized/EN.xaml", UriKind.Absolute) };
        private static ResourceDictionary resourceDictionaryRu = new ResourceDictionary() { Source = new Uri("pack://application:,,,/Localized/RU.xaml", UriKind.Absolute) };

        internal static bool IsRU => culture == CONST.LANG_RU ? true : false;

        internal static ResourceDictionary GetCultureDictionary() => culture == CONST.LANG_RU ? resourceDictionaryRu : resourceDictionaryEn;
        internal static ResourceDictionary GetCultureDictionary(string cultureLang) => cultureLang == CONST.LANG_RU ? resourceDictionaryRu : resourceDictionaryEn;

        internal static string GetCultureName() => culture;

        internal static ResourceDictionary ChangeCultureDictionary()
        {
            culture = IsRU ? CONST.LANG_EN : CONST.LANG_RU;
            return GetCultureDictionary();
        }

        internal static void SetCultureDictionaryKeyValue(string cultureLang, string[] keys, string[] values)
        {
            ResourceDictionary dictionary = GetCultureDictionary(cultureLang);

            for (int i = 0; i < keys.Length; i++)
            {
                dictionary[keys[i]] = values[i];
            }
        }
    }
}
