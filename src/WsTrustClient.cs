using System;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Dlbr.CommonLogin.IdentityModel.Logging;
using Dlbr.CommonLogin.IdentityModel.WSTrustBindings;

namespace Dlbr.CommonLogin.IdentityModel
{
    public partial class WsTrustClient
    {
        private static readonly ILog Log = LogProvider.For<WsTrustClient>();

        private readonly string _usernameMixedEndpoint;
        private readonly string _windowsTransportEndpoint;
        public Uri ProxyAddress { get; set; }

        public WsTrustClient(string adfsDnsName)
        {
            if (!Regex.IsMatch(adfsDnsName, @"^[\w-]+(\.[\w-]+)*$", RegexOptions.IgnoreCase))
            {
                throw new ArgumentException("Supply the DNS-name portion of the ADFS url, e.g. 'idp.dlbr.dk'");
            }
            const string usernameMixedFormatString = "https://{0}/adfs/services/trust/13/usernamemixed";
            const string windowsTransportFormatString = "https://{0}/adfs/services/trust/13/windowstransport";
            _usernameMixedEndpoint = string.Format(usernameMixedFormatString, adfsDnsName);
            _windowsTransportEndpoint = string.Format(windowsTransportFormatString, adfsDnsName);
        }

        public virtual SecurityToken GetSecurityToken(string realm, string username, string password)
        {
            return GetSecurityTokenResponse(realm, username, password).Token;
        }

        private TokenResponse GetSecurityTokenResponse(string realm, string username, string password)
        {
            Log.Debug(string.Format("Requesting token for {0} at {1} from {2}", username, realm, _usernameMixedEndpoint));
            WSTrustChannelFactory factory = CreateUsernameWSTrustChannelFactory(username, password);
            var rst = CreateRst(realm);
            return IssueTokenForRst(factory, rst);
        }

        public virtual async Task<SecurityToken> GetSecurityTokenAsync(string realm, string username, string password)
        {
            return (await GetSecurityTokenResponseAsync(realm, username, password).ConfigureAwait(false)).Token;
        }

        public async Task<TokenResponse> GetSecurityTokenResponseAsync(string realm, string username, string password)
        {
            Log.Debug(string.Format("Requesting token for {0} at {1} from {2}", username, realm, _usernameMixedEndpoint));
            WSTrustChannelFactory factory = CreateUsernameWSTrustChannelFactory(username, password);
            var rst = CreateRst(realm);
            return await IssueTokenForRstAsync(factory, rst).ConfigureAwait(false);
        }

        public TokenResponse GetTokenResponseForCurrentProcessIdentity(string realm)
        {
            Log.Debug(string.Format("Requesting token for current process identity at {0} from {1}", realm, _usernameMixedEndpoint));
            var factory = CreateWindowsTransportWSTrustChannelFactory();
            var rst = CreateRst(realm);
            return IssueTokenForRst(factory, rst);
        }

        public async Task<TokenResponse> GetTokenResponseForCurrentProcessIdentityAsync(string realm)
        {
            Log.Debug(string.Format("Requesting token for current process identity at {0} from {1}", realm, _usernameMixedEndpoint));
            WSTrustChannelFactory factory = CreateWindowsTransportWSTrustChannelFactory();
            var rst = CreateRst(realm);
            return await IssueTokenForRstAsync(factory, rst).ConfigureAwait(false);
        }

        public virtual SecurityToken GetActAsToken(string realm, string serviceUsername, string servicePassword)
        {
            var bootstrapToken = ((ClaimsIdentity) Thread.CurrentPrincipal.Identity).BootstrapContext as SecurityToken;
            if (bootstrapToken == null)
            {
                throw new InvalidOperationException("Thread.CurrentPrincipal.Identity.BootstrapContext is null or not a SecurityToken");
            }
            return GetActAsToken(realm, serviceUsername, servicePassword, bootstrapToken);
        }

        public virtual SecurityToken GetActAsToken(string realm, string serviceUsername, string servicePassword, SecurityToken bootstrapToken)
        {
            return GetActAsTokenResponse(realm, serviceUsername, servicePassword, bootstrapToken).Token;
        }

        private TokenResponse GetActAsTokenResponse(string realm, string serviceUsername, string servicePassword, SecurityToken bootstrapToken)
        {
            if (bootstrapToken == null) throw new ArgumentNullException("bootstrapToken");
            Log.Debug(string.Format("Requesting ActAs token for {0} / bootstrap token {1} at {2} from {3}", serviceUsername,
                bootstrapToken.Id, realm, _usernameMixedEndpoint));
            var factory = CreateUsernameWSTrustChannelFactory(serviceUsername, servicePassword);
            var rst = CreateRst(realm, bootstrapToken);
            return IssueTokenForRst(factory, rst);
        }

        public virtual async Task<SecurityToken> GetActAsTokenAsync(string realm, string serviceUsername, string servicePassword)
        {
            var bootstrapToken = ((ClaimsIdentity)Thread.CurrentPrincipal.Identity).BootstrapContext as SecurityToken;
            if (bootstrapToken == null)
            {
                throw new InvalidOperationException("Thread.CurrentPrincipal.Identity.BootstrapContext is null or not a SecurityToken");
            }
            return await GetActAsTokenAsync(realm, serviceUsername, servicePassword, bootstrapToken).ConfigureAwait(false);
        }

        public virtual async Task<SecurityToken> GetActAsTokenAsync(string realm, string serviceUsername, string servicePassword, SecurityToken bootstrapToken)
        {
            return (await GetActAsTokenResponseAsync(realm, serviceUsername, servicePassword, bootstrapToken).ConfigureAwait(false)).Token;
        }

        public async Task<TokenResponse> GetActAsTokenResponseAsync(string realm, string serviceUsername, string servicePassword, SecurityToken bootstrapToken)
        {
            if (bootstrapToken == null) throw new ArgumentNullException("bootstrapToken");
            Log.Debug(string.Format("Requesting ActAs token for {0} / bootstrap token {1} at {2} from {3}", serviceUsername,
                bootstrapToken.Id, realm, _usernameMixedEndpoint));
            var factory = CreateUsernameWSTrustChannelFactory(serviceUsername, servicePassword);
            var rst = CreateRst(realm, bootstrapToken);
            return await IssueTokenForRstAsync(factory, rst).ConfigureAwait(false);
        }

        public TokenResponse GetActAsTokenResponseForCurrentProcessIdentity(string realm, SecurityToken bootstrapToken)
        {
            if (bootstrapToken == null) throw new ArgumentNullException("bootstrapToken");
            Log.Debug(string.Format("Requesting token for current process identity / bootstrap token {0} at {1} from {2}", bootstrapToken.Id, realm, _usernameMixedEndpoint));
            var factory = CreateWindowsTransportWSTrustChannelFactory();
            var rst = CreateRst(realm, bootstrapToken);
            return IssueTokenForRst(factory, rst);
        }

        public async Task<TokenResponse> GetActAsTokenResponseForCurrentProcessIdentityAsync(string realm, SecurityToken bootstrapToken)
        {
            if (bootstrapToken == null) throw new ArgumentNullException("bootstrapToken");
            Log.Debug(string.Format("Requesting token for current process identity / bootstrap token {0} at {1} from {2}",  
                bootstrapToken.Id, realm, _usernameMixedEndpoint));
            WSTrustChannelFactory factory = CreateWindowsTransportWSTrustChannelFactory();
            var rst = CreateRst(realm, bootstrapToken);
            return await IssueTokenForRstAsync(factory, rst).ConfigureAwait(false);
        }


        private static RequestSecurityToken CreateRst(string realm, SecurityToken bootstrapToken = null)
        {
            var rst = new RequestSecurityToken
            {
                RequestType = WSTrust13Constants.RequestTypes.Issue,
                AppliesTo = new EndpointReference(realm),
                KeyType = WSTrust13Constants.KeyTypes.Bearer,
            };
            if (bootstrapToken != null)
            {
                rst.ActAs = new SecurityTokenElement(bootstrapToken);
            }
            return rst;
        }

        protected virtual WSTrustChannelFactory CreateUsernameWSTrustChannelFactory(string serviceUsername, string servicePassword)
        {
            var binding = new UserNameWSTrustBinding(SecurityMode.TransportWithMessageCredential);

            if (ProxyAddress != null)
            {
                binding.ProxyAddress = ProxyAddress;
            }
            var factory = new WSTrustChannelFactory(binding, _usernameMixedEndpoint)
            {
                TrustVersion = TrustVersion.WSTrust13
            };
            factory.Credentials.UserName.UserName = serviceUsername;
            factory.Credentials.UserName.Password = servicePassword;
            return factory;
        }

        private WSTrustChannelFactory CreateWindowsTransportWSTrustChannelFactory()
        {
            var binding = new WS2007HttpBinding(SecurityMode.Transport);
            if (ProxyAddress != null)
            {
                binding.ProxyAddress = ProxyAddress;
                binding.UseDefaultWebProxy = false;
            }
            var factory = new WSTrustChannelFactory(binding, _windowsTransportEndpoint)
            {
                TrustVersion = TrustVersion.WSTrust13
            };
            return factory;
        }


        protected virtual TokenResponse IssueTokenForRst(WSTrustChannelFactory factory, RequestSecurityToken rst)
        {
            var channel = factory.CreateChannel();
            RequestSecurityTokenResponse rstr;
            var token = channel.Issue(rst, out rstr);
            return new TokenResponse(token, rstr);
        }

        protected virtual async Task<TokenResponse> IssueTokenForRstAsync(WSTrustChannelFactory factory, RequestSecurityToken rst)
        {
            var channel = factory.CreateChannel();

            var taskCompletionSource = new TaskCompletionSource<TokenResponse>();
            channel.BeginIssue(rst, asyncResult =>
            {
                try
                {
                    RequestSecurityTokenResponse rstr2;
                    var token = channel.EndIssue(asyncResult, out rstr2);
                    taskCompletionSource.SetResult(new TokenResponse(token, rstr2));
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(ex);
                }
            }, null);
            return await taskCompletionSource.Task.ConfigureAwait(false);
        }
    }
}