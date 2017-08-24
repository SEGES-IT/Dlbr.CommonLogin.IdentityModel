using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.IO;
using System.Security.Claims;
using System.ServiceModel.Security;
using System.Xml;

namespace Dlbr.CommonLogin.IdentityModel.Windows.Login
{
    public class TokenStringReader
    {
        /// <summary>
        /// Parses the suppied token string to a Claims Principal, not performing validation of the issuer/signature
        /// This is useful for clients desiring to read claims from tokens not issued to themselves, for personalization purposes.
        /// </summary>
        /// <param name="samlTokenString">SAML token string to parse</param>
        /// <returns>ClaimsPrincipal with the identities and claims contained in the token</returns>
        public ClaimsPrincipal ReadUnvalidated(string samlTokenString)
        {
            var configuration = new SecurityTokenHandlerConfiguration
            {
                AudienceRestriction = new AudienceRestriction(AudienceUriMode.Never),
                CertificateValidationMode = X509CertificateValidationMode.None,
                CertificateValidator = new NoneX509CertificateValidator(),
                DetectReplayedTokens = false,
                IssuerNameRegistry = new AcceptAnyIssuerNameRegistry(),
                MaxClockSkew = TimeSpan.MaxValue,
            };

            var handlers = SecurityTokenHandlerCollection.CreateDefaultSecurityTokenHandlerCollection(configuration);
            var securityToken = ReadSecurityToken(samlTokenString, handlers);
            return new ClaimsPrincipal(handlers.ValidateToken(securityToken));

        }

        private SecurityToken ReadSecurityToken(string tokenString, SecurityTokenHandlerCollection handlers)
        {
            using (var reader = new XmlTextReader(new StringReader(tokenString)))
            {
                var token = handlers.ReadToken(reader);
                return token;
            }
        }
    }
}