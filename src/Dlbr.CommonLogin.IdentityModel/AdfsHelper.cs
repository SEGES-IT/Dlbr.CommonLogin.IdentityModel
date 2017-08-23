using System;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;

namespace Dlbr.CommonLogin.IdentityModel
{
    public static class AdfsHelper
    {
        public static SecurityToken GetActAsToken(string idpEndpoint, string realm, string serviceUsername, string servicePassword)
        {
            var idp = StripNonHostPart(idpEndpoint);
            var wsTrustClient = new WsTrustClient(idp);
            return wsTrustClient.GetActAsToken(realm, serviceUsername, servicePassword);
        }

        public static SecurityToken GetActAsToken(string idpEndpoint, string realm, string serviceUsername, string servicePassword, SecurityToken bootstrapToken)
        {
            var idp = StripNonHostPart(idpEndpoint);
            var wsTrustClient = new WsTrustClient(idp);
            return wsTrustClient.GetActAsToken(realm, serviceUsername, servicePassword, bootstrapToken);
        }

        public static SecurityToken GetSecurityToken(string idpEndpoint, string realm, string username, string password)
        {
            var idp = StripNonHostPart(idpEndpoint);
            var wsTrustClient = new WsTrustClient(idp);
            return wsTrustClient.GetSecurityToken(realm, username, password);
        }

        private static string StripNonHostPart(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                var uri = new Uri(url,UriKind.Absolute);
                return uri.DnsSafeHost;
            }
            return url;
        }
    }
}
