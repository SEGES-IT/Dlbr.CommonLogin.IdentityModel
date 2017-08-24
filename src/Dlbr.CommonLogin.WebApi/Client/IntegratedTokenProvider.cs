using System;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.ServiceModel;
using System.ServiceModel.Security;
using Dlbr.CommonLogin.IdentityModel;
using Dlbr.CommonLogin.WebApi.Logging;
using System.Threading.Tasks;

namespace Dlbr.CommonLogin.WebApi.Client
{
    public class IntegratedTokenProvider : ITokenProvider
    {
        private static readonly ILog Log = LogProvider.For<IntegratedTokenProvider>();

        public IntegratedTokenProvider(IProxyConfiguration proxyConfiguration)
        {
            this.ProxyConfiguration = proxyConfiguration;
        }

        private readonly IProxyConfiguration ProxyConfiguration;

        protected virtual SecurityToken GetBootstrapToken()
        {
            return BootstrapTokenProvider.GetBootstrapTokenFromContext();
        }

        //public async Task<SecurityToken> GetToken(ISecureWebApiConfiguration configuration)
        //{
        //    var idp = string.Format("https://{0}/adfs/services/trust/13/windowstransport", configuration.AdfsDnsName);
        //    var binding = new WS2007HttpBinding(SecurityMode.Transport);
        //    //Create enpoint address with explicit 'dummy' spn identity to force ntlm instead of kerberos
        //    var endpointAddress = new EndpointAddress(new Uri(idp), EndpointIdentity.CreateSpnIdentity(string.Empty));
        //    var factory = new WSTrustChannelFactory(binding, endpointAddress)
        //    {
        //        TrustVersion = TrustVersion.WSTrust13
        //    };

        //    if (this.ProxyConfiguration != null && this.ProxyConfiguration.Address != null)
        //    {
        //        binding.ProxyAddress = this.ProxyConfiguration.Address;
        //        binding.UseDefaultWebProxy = false;
        //    }

        //    var rst = new RequestSecurityToken
        //    {
        //        RequestType = RequestTypes.Issue,
        //        AppliesTo = new EndpointReference(configuration.Realm),
        //        KeyType = KeyTypes.Bearer
        //    };

        //    if (configuration.ActAs)
        //    {
        //        var bootstrapToken = GetBootstrapToken();
        //        if (bootstrapToken == null)
        //        {
        //            throw new Exception(string.Format("The configuration '{0}' specifies to 'ActAs' but no bootstraptoken could be obtained.", configuration.GetType().FullName));
        //        }
        //        rst.ActAs = new SecurityTokenElement(bootstrapToken);
        //    }

        //    var channel = factory.CreateChannel();
        //    var taskCompletionSource = new TaskCompletionSource<TokenResponse>();
        //    channel.BeginIssue(rst, asyncResult =>
        //    {
        //        try
        //        {
        //            RequestSecurityTokenResponse rstr2;
        //            var token = channel.EndIssue(asyncResult, out rstr2);
        //            taskCompletionSource.SetResult(new TokenResponse(token, rstr2));
        //        }
        //        catch (Exception ex)
        //        {
        //            taskCompletionSource.SetException(ex);
        //        }
        //    }, null);

        //    var response = await taskCompletionSource.Task.ConfigureAwait(false);
        //    return response.Token;
        //}

        public async Task<SecurityToken> GetToken(ISecureWebApiConfiguration configuration)
        {
            var c = new WsTrustClient(configuration.AdfsDnsName)
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
                return (await c.GetActAsTokenResponseForCurrentProcessIdentityAsync(configuration.Realm, bootstrapToken).ConfigureAwait(false)).Token;
            }
            return (await c.GetTokenResponseForCurrentProcessIdentityAsync(configuration.Realm).ConfigureAwait(false)).Token;
        }
    }
}
