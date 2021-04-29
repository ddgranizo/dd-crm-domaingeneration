using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelUI.Utilities
{
    public static class SettingsManager
    {

        public static void SetAppConfig(string configName, string value)
        {

            try
            {
                Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (!configuration.AppSettings.Settings.AllKeys.Contains(configName))
                {
                    configuration.AppSettings.Settings.Add(new KeyValueConfigurationElement(configName, null));
                }
                configuration.AppSettings.Settings[configName].Value = value;
                configuration.Save();
                ConfigurationManager.RefreshSection("appSettings");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetAppConfig(string configName)
        {
            string config = string.Empty;
            try
            {
                config = ConfigurationManager.AppSettings[configName];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return config;
        }
    }
}
