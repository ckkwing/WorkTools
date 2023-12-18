using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkTools.Modules.Android.Model
{
    internal class TranslationItem
    {
        private string stringID = string.Empty;
        internal string StringID { get => stringID; set => stringID = value; }
        internal bool? IsTranslatable { get; set; } = null;

        //ENU - value
        private IDictionary<string, string> dicLanguages = new Dictionary<string, string>();
        internal IDictionary<string, string> DicLanguages { get => dicLanguages; set => dicLanguages = value; }

        internal TranslationItem(string stringID, bool? isTranslatable = null)
        {
            StringID = stringID;
            IsTranslatable = isTranslatable;
        }
    }
}
