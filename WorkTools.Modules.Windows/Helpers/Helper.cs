using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WorkTools.Modules.Windows.Helpers
{
    public static class Helper
    {
        public static ResourceDictionary LoadStyleDictionaryFromFile(string inFileName)
        {
            ResourceDictionary resourceDictionary = null;
            if (File.Exists(inFileName))
            {
                try
                {
                    using (var fs = new FileStream(inFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        // Read in ResourceDictionary File
                        resourceDictionary = (ResourceDictionary)System.Windows.Markup.XamlReader.Load(fs);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("LoadStyleDictionaryFromFile:" + ex.Message);
                }
            }
            return resourceDictionary;
        }


    }
}
