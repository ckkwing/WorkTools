using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkTools.Modules.Windows
{
    internal static class Constant
    {
        internal static IReadOnlyList<IList<string>> MultilingualIdentificationCodeList = new List<IList<string>>()
        {
            new List<string>(){ "CHS", "zh-cn"},
            new List<string>(){ "CHT", "zh-tw"},
            new List<string>(){ "CSY", "cs-cz"},
            new List<string>(){ "DAN", "da-dk"},
            new List<string>(){ "DEU", "de-de"},
            new List<string>(){ "ELL", "el-gr"},
            new List<string>(){ "ENU", "en-US"},
            new List<string>(){ "ESP", "es-es"},
            new List<string>(){ "FIN", "fi-fi"},
            new List<string>(){ "FRA", "fr-fr"},
            new List<string>(){ "HUN", "hu-hu"},
            new List<string>(){ "ITA", "it-it"},
            new List<string>(){ "JPN", "ja-jp"},
            new List<string>(){ "KOR", "ko-kr"},
            new List<string>(){ "NLD", "nl-nl"},
            new List<string>(){ "NOR", "nb-no"},
            new List<string>(){ "PLK", "pl-pl"},
            new List<string>(){ "PTB", "pt-br"},
            new List<string>(){ "PTG", "pt-pt"},
            new List<string>(){ "RUS", "ru-ru"},
            new List<string>(){ "SVE", "sv-se"},
            new List<string>(){ "THA", "th-th"},
            new List<string>(){ "TRK", "tr-tr"}
        };

        static Constant()
        {
        }
    }
}
