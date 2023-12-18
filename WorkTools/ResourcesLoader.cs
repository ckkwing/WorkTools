using NLogger;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace WorkTools
{
    public class ResourcesLoader
    {
        public static ResourceDictionary LoadLanguage(CultureInfo cultureInfo)
        {
            ResourceDictionary lanRes = new ResourceDictionary() { Source = new Uri("/LocalizedResources/Language.xaml", UriKind.Relative) };
            string LanResPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LocalizedResources");
            string en_USResFile = Path.Combine(LanResPath, "Language_en-US.xaml");
            string resFile = string.Format("Language_{0}.xaml", cultureInfo.IetfLanguageTag);
            string curLanRes = Path.Combine(LanResPath, resFile);
            ResourceDictionary localizedlanRes = null;
            if (File.Exists(curLanRes))
            {
                try
                {
                    using (var fs = new FileStream(curLanRes, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        // Read in an EnhancedResourceDictionary File or preferably an GlobalizationResourceDictionary file
                        localizedlanRes = XamlReader.Load(fs) as ResourceDictionary;
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.UILogger.Error("load language file Failed", ex);
                }
            }

            if (localizedlanRes == null)
            {
                LogHelper.UILogger.DebugFormat("try to load language file:{0}", en_USResFile);
                if (File.Exists(en_USResFile))
                {
                    try
                    {
                        using (var fs = new FileStream(en_USResFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            localizedlanRes = XamlReader.Load(fs) as ResourceDictionary;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.UILogger.Error("load language file Failed", ex);
                    }
                }
            }

            if (localizedlanRes != null)
            {
                foreach (Object dicKey in localizedlanRes.Keys)
                {
                    if (lanRes.Contains(dicKey))
                    {
                        lanRes[dicKey] = localizedlanRes[dicKey];
                    }
                }
            }

            ReplacePlaceHolder(lanRes);
            return lanRes;
        }

        private static void ReplacePlaceHolder(ResourceDictionary lanRes)
        {
            LogHelper.UILogger.Debug("ReplacePlaceHolder Start");
            Dictionary<String, String> ControlCodesReplacement = new Dictionary<String, String>();
            ControlCodesReplacement.Add("\\n", "\n");
            ControlCodesReplacement.Add("\\t", "\t");
            ControlCodesReplacement.Add("\\\"", "\"");
            ControlCodesReplacement.Add("\\v", "\v");
            ControlCodesReplacement.Add("\\r", "\r");
            ControlCodesReplacement.Add("\\\\", "\\");
            //ControlCodesReplacement.Add("[THIS_APPLICATION_NAME]", StringConstant.ApplicationName);
            //ControlCodesReplacement.Add("[PRODUCT_NAME]", StringConstant.ProductName);
            //ControlCodesReplacement.Add("[FEATURENAME]", StringConstant.ApplicationName);
            //ControlCodesReplacement.Add("[APPLICATION_NAME_ANDROID]", StringConstant.BackItUpAndroidAppName);

            foreach (Object newDictKey in lanRes.Keys)
            {
                Object newDictValue = null;
                if (!lanRes.Contains(newDictKey))
                {
                    continue;
                }

                newDictValue = lanRes[newDictKey];
                if (newDictValue is String)
                {
                    String tempString = (String)newDictValue;
                    foreach (String ControlCode in ControlCodesReplacement.Keys)
                    {
                        tempString = tempString.Replace(ControlCode, ControlCodesReplacement[ControlCode]);
                    }
                    lanRes[newDictKey] = tempString;
                }
            }
            // AH: Excluded Flow Direction from LocalizedDictionary.xaml and adding it programmatically in ResourceDictionary
            lanRes.Add("IDL_FLOWDIRECTION", FlowDirection.LeftToRight);

            LogHelper.UILogger.Debug("ReplacePlaceHolder End");
        }

    }
}
