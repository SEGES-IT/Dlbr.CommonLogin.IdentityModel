using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlbr.CommonLogin.WebApi.Client
{
    public class ProxyAppSettingsConfiguration : IAppSettingsConfiguration, IProxyConfiguration
    {
        public string Prefix
        {
            get { return "proxy";  }
        }

        public Uri Address
        {
            get
            {
                var address = this.GetAppSetting("Address");
                return String.IsNullOrEmpty(address)
                    ? null
                    : new Uri(address);
            }
        }
    }
}
