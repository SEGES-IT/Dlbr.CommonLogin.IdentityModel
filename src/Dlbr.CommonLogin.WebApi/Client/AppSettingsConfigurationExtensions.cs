using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlbr.CommonLogin.WebApi.Client
{
    public static class AppSettingsConfigurationExtensions
    {
        public static string GetAppSetting(this IAppSettingsConfiguration configuration, string name)
        {
            var qualifiedName = String.Format("{0}:{1}", configuration.Prefix, name);
            return ConfigurationManager.AppSettings[qualifiedName];
        }
    }
}
