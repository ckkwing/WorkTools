using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WorkTools.Infrastructure.Model
{


    [XmlRoot("resources")]
    public class StringResources
    {
        [XmlElement("string")]
        public List<StringItem> StringItems { get; set; }
    }
}
