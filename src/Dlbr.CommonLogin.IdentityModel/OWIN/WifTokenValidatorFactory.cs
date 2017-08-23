using System;
using System.IdentityModel.Services;
using System.IO;
using System.Security.Claims;
using System.Xml;

namespace Dlbr.CommonLogin.IdentityModel.Owin
{
    public static class WifTokenValidatorFactory
    {
        public static Func<string, ClaimsPrincipal> CreateWindowsIdentityFoundationTokenValidator()
        {
            return tokenString =>
            {
                var handlers = FederatedAuthentication.FederationConfiguration.IdentityConfiguration.SecurityTokenHandlers;

                using (var sr = new StringReader(tokenString))
                {
                    using (var xr = new XmlTextReader(sr))
                    {
                        var token = handlers.ReadToken(xr);
                        var identityCollection = handlers.ValidateToken(token);
                        var principal = new ClaimsPrincipal(identityCollection);
                        return principal;
                    }
                }
            };
        }
    }
}