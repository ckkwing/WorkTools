using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Utility.Extensions;

namespace WorkTools.Modules.Android.Model
{
    [XmlRoot("resources")]
    public class TranslationResourcesEntity
    {
        [XmlElement("string")]
        public List<TranslationStringItem> Strings { get; set; }

        public void Add(string name, string value, string translatable = "")
        {
            string tempValue = value.Replace("'", "\\'");
            TranslationStringItem item = new TranslationStringItem() { Name = name, Value = tempValue };
            if (!string.IsNullOrEmpty(translatable))
            {
                item.Translatable = translatable;
            }
            Strings.Add(item);
        }

        public void Add(TranslationStringItem item)
        {
            Strings.Add(item);
        }

        public bool ReplaceExistStringByName(string name, string value)
        {
            bool bRel = false;
            TranslationStringItem translationStringItem = Strings.FirstOrDefault(item => 0 == string.Compare(item.Name, name, true));
            if (!translationStringItem.IsNull())
            {
                translationStringItem.Value = value;
                bRel = true;
            }
            return bRel;
        }
    }

    public class TranslationStringItem
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        //[XmlIgnore]
        [XmlAttribute("translatable")]
        public string Translatable { get; set; }

        //[XmlAttribute(AttributeName = "translatable")]
        //public string TranslatableValue
        //{
        //    get
        //    {
        //        return translatable.HasValue ? translatable.Value.ToString() : null;
        //    }
        //    set
        //    {
        //        bool result;
        //        // (3)
        //        translatable = bool.TryParse(value, out result) ? result : (bool?)null;
        //    }
        //}

        [XmlText]
        public string Value { get; set; }
    }
}
