using System;
using System.Configuration;
using System.Threading.Tasks;

namespace Dlbr.CommonLogin.WebApi.Client
{
    public abstract class WebApiAppSettingsConfiguration : WebApiConfiguration, IAppSettingsConfiguration
    {
        public WebApiAppSettingsConfiguration()
        {
        }

        public abstract string Prefix { get; }

        public override Uri Endpoint
        {
            get { return new Uri(this.GetAppSetting("Endpoint")); }
        }
    }
}
