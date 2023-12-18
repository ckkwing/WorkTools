using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkTools.Modules.Android
{
    internal class Constant
    {
        internal static readonly IDictionary<string, string> BIUTranslationFolderMatchDic = new Dictionary<string, string>() {
            {"en-US", "values" },
            { "DEU", "values-de"},
            { "ESP", "values-es"},
            { "JPN", "values-ja"},
            { "FRA", "values-fr"},
            { "ITA", "values-it"},
            { "CHS", "values-zh-rCN"},
            { "CHT", "values-zh-rTW"}
        };

        internal static readonly IDictionary<string, string> StreamingTranslationFolderMatchDic = new Dictionary<string, string>() {
            //{"en-US", "values" },
            //{ "ENU", "values-en-rUS"},
             { "ENU", "values-en"},
            { "CSY", "values-cs"},
            { "DEU", "values-de"},
            { "ESP", "values-es"},
            { "FRA", "values-fr"},
            { "ITA", "values-it"},
            { "JPN", "values-ja"},
            { "KOR", "values-ko"},
            { "NLD", "values-nl"},
            { "PLK", "values-pl"},
            //{ "PTG", "values-pt-rPT"},
             { "PTG", "values-pt"},
            { "PTB", "values-pt-rBR"},
            { "RUS", "values-ru"},
            { "SVE", "values-sv"},
            { "CHS", "values-zh-rCN"},
            { "CHT", "values-zh-rTW"}
        };

        internal static readonly IDictionary<string, string> DriveSpanFolderMatchDic = new Dictionary<string, string>() {
            {"en-US", "values" },
            { "ENU", "values-en-rUS"},
            { "CSY", "values-cs"},
            { "DEU", "values-de"},
            { "ESP", "values-es"},
            { "FRA", "values-fr"},
            { "ITA", "values-it"},
            { "JPN", "values-ja"},
            { "KOR", "values-ko"},
            { "NLD", "values-nl"},
            { "PLK", "values-pl"},
            { "PTG", "values-pt-rPT"},
            { "PTB", "values-pt-rBR"},
            { "RUS", "values-ru"},
            { "SVE", "values-sv"},
            { "CHS", "values-zh-rCN"},
            { "CHT", "values-zh-rTW"}
        };

        internal static readonly IDictionary<string, string> CommonAndroidFolderMatchDic = new Dictionary<string, string>() {
            {"en-US", "values" },
            { "ENU", "values-en-rUS"},
            { "CSY", "values-cs"},
            { "DEU", "values-de"},
            { "ESP", "values-es"},
            { "FRA", "values-fr"},
            { "ITA", "values-it"},
            { "JPN", "values-ja"},
            { "KOR", "values-ko"},
            { "NLD", "values-nl"},
            { "PLK", "values-pl"},
            { "PTG", "values-pt-rPT"},
            { "PTB", "values-pt-rBR"},
            { "RUS", "values-ru"},
            { "SVE", "values-sv"},
            { "CHS", "values-zh-rCN"},
            { "CHT", "values-zh-rTW"}
        };
    }
}
