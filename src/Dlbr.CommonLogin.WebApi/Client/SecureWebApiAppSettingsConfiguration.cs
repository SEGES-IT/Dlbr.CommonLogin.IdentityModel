using System;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Net.Http.Headers;
using Dlbr.CommonLogin.IdentityModel.WebApi;
using System.Threading.Tasks;

namespace Dlbr.CommonLogin.WebApi.Client
{


    public abstract class SecureWebApiAppSettingsConfiguration : SecureWebApiConfiguration, IAppSettingsConfiguration
    {
        public SecureWebApiAppSettingsConfiguration(ITokenProviderFactory tokenProviderFactory)
            : base(tokenProviderFactory)
        {
        }

        public abstract string Prefix { get; }

        public override Uri Endpoint
        {
            get { return new Uri(this.GetAppSetting("Endpoint")); }
        }

        public override string AdfsDnsName
        {
            get { return this.GetAppSetting("AdfsDnsName"); }
        }

        public override string Realm
        {
            get { return this.GetAppSetting("Realm"); }
        }

        public override string Username
        {
            get { return this.GetAppSetting("Username"); }
        }

        public override string Password
        {
            get { return this.GetAppSetting("Password"); }
        }

        public override bool ActAs
        {
            get
            {
                var actAs = this.GetAppSetting("ActAs");
                return String.IsNullOrEmpty(actAs)
                    ? false
                    : Convert.ToBoolean(actAs);
            }
        }

        public override Type TokenProviderType
        {
            get
            {
                var typename = this.GetAppSetting("TokenProvider");
                return Type.GetType(typename);
            }
        }
    }
}
