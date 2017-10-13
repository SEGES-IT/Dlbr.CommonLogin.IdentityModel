using Seges.IdentityModel.Log.Logging;
using Seges.IdentityModel.Utils;
using System;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Seges.IdentityModel.WsTrust
{
    public class WsTrustClient
    {
        private static readonly ILog Log = LogProvider.For<WsTrustClient>();

        private readonly string _usernameMixedEndpoint;
        private RstrParser _rstrParser;

        // Note https://docs.microsoft.com/en-us/azure/architecture/antipatterns/improper-instantiation/ - might be a good idea to share the WsTrustClient instance
        private HttpClient _httpClient { get; }


        public Uri ProxyAddress { get; set; }

        public WsTrustClient(string adfsDnsName)
        {
            if (!Regex.IsMatch(adfsDnsName, @"^[\w-]+(\.[\w-]+)*$", RegexOptions.IgnoreCase))
            {
                throw new ArgumentException("Supply the DNS-name portion of the ADFS url, e.g. 'idp.dlbr.dk'");
            }
            const string usernameMixedFormatString = "https://{0}/adfs/services/trust/13/usernamemixed";
            _usernameMixedEndpoint = string.Format(usernameMixedFormatString, adfsDnsName);
            _httpClient = new HttpClient();
            _rstrParser = new RstrParser();
        }

        public virtual async Task<SamlTokenResponse> RequestTokenAsync(SamlTokenRequest request)
        {
            var rst = BuildRst(request);
            return await RequestTokenAsync(rst);
        }

        private async Task<SamlTokenResponse> RequestTokenAsync(string rst)
        {
            var response = await _httpClient.PostAsync(_usernameMixedEndpoint, new StringContent(rst, Encoding.UTF8, "application/soap+xml"));
            var responseContent = await response.Content.ReadAsStringAsync();
            var samlTokenResponse = ExtractTokenResponse(response, responseContent);
            return samlTokenResponse;
        }

        private SamlTokenResponse ExtractTokenResponse(HttpResponseMessage response, string responseContent)
        {
            response.EnsureSuccessStatusCode(responseContent);
            var rstr = responseContent;
            var samlTokenResponse = _rstrParser.Parse(rstr);
            return samlTokenResponse;
        }

        private string BuildRst(SamlTokenRequest request)
        {
            var messageId = Guid.NewGuid();
            var tokenId = Guid.NewGuid();
            return string.Format(
                WsTrustSaml11TokenRstTemplate,
                messageId,
                _usernameMixedEndpoint,
                tokenId,
                request.Username,
                request.Password,
                request.Audience);
        }

        //public virtual async Task<TokenResponse> RequestActAsTokenAsync(ActAsTokenRequest request)
        //{
        //    return null;
        //}

        const string WsTrustSaml11TokenRstTemplate =
        @"<s:Envelope 
            xmlns:s=""http://www.w3.org/2003/05/soap-envelope"" 
            xmlns:a=""http://www.w3.org/2005/08/addressing"" 
            xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">
            <s:Header>
                <a:Action s:mustUnderstand=""1"">http://docs.oasis-open.org/ws-sx/ws-trust/200512/RST/Issue</a:Action>
                <a:MessageID>urn:uuid:{0}</a:MessageID>
                <a:ReplyTo>
                    <a:Address>http://www.w3.org/2005/08/addressing/anonymous</a:Address>
                </a:ReplyTo>
                <a:To s:mustUnderstand=""1"">{1}</a:To>
                <o:Security s:mustUnderstand=""1"" 
                    xmlns:o=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"">
                    <o:UsernameToken u:Id=""uuid-{2}-7"">
                        <o:Username>{3}</o:Username>
                        <o:Password o:Type=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText"">{4}</o:Password>
                    </o:UsernameToken>
                </o:Security>
            </s:Header>
            <s:Body>
                <trust:RequestSecurityToken 
                    xmlns:trust=""http://docs.oasis-open.org/ws-sx/ws-trust/200512"">
                    <wsp:AppliesTo 
                        xmlns:wsp=""http://schemas.xmlsoap.org/ws/2004/09/policy"">
                        <a:EndpointReference>
                            <a:Address>{5}</a:Address>
                        </a:EndpointReference>
                    </wsp:AppliesTo>
                    <trust:KeyType>http://docs.oasis-open.org/ws-sx/ws-trust/200512/Bearer</trust:KeyType>
                    <trust:RequestType>http://docs.oasis-open.org/ws-sx/ws-trust/200512/Issue</trust:RequestType>
                </trust:RequestSecurityToken>
            </s:Body>
        </s:Envelope>";
    }
}