using System;
using System.IdentityModel.Tokens;
using System.Threading;
using Dlbr.CommonLogin.IdentityModel;
using System.Threading.Tasks;

namespace Dlbr.CommonLogin.WebApi.Client
{
    public class ServiceAccountTokenProvider : ITokenProvider
    {
        public ServiceAccountTokenProvider(IProxyConfiguration proxyConfiguration)
        {
            this.ProxyConfiguration = proxyConfiguration;
        }

        private readonly IProxyConfiguration ProxyConfiguration;

        protected virtual SecurityToken GetBootstrapToken()
        {
            return BootstrapTokenProvider.GetBootstrapTokenFromContext();
        }

        public async Task<SecurityToken> GetToken(ISecureWebApiConfiguration configuration)
        {
            var client = new WsTrustClient(configuration.AdfsDnsName)
            {
                ProxyAddress = this.ProxyConfiguration.Address
            };
            if (configuration.ActAs)
            {
                var bootstrapToken = GetBootstrapToken();
                if (bootstrapToken == null)
                {
                    throw new Exception(string.Format("The configuration '{0}' specifies to 'ActAs' but no bootstraptoken could be obtained.", configuration.GetType().FullName));
                }
                return await client.GetActAsTokenAsync(configuration.Realm, configuration.Username, configuration.Password, bootstrapToken).ConfigureAwait(false);
            }
            return (await client.GetSecurityTokenResponseAsync(configuration.Realm, configuration.Username, configuration.Password).ConfigureAwait(false)).Token;
        }
    }
}
