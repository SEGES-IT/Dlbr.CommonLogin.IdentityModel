using System.IdentityModel.Tokens;

namespace Dlbr.CommonLogin.IdentityModel.Tests.Infrastructure.TestSts
{
    public class SimpleValidateByThumbprintOnlyIssuerNameRegistry : IssuerNameRegistry
    {
        public string Issuer { get; set; }
        public string AcceptedThumbprint { get; set; }
        public SimpleValidateByThumbprintOnlyIssuerNameRegistry(string thumbprint, string issuer)
        {
            Issuer = issuer;
            AcceptedThumbprint = thumbprint.ToLowerInvariant();
        }

        public override string GetIssuerName(SecurityToken securityToken)
        {
            var token = securityToken as X509SecurityToken;
            if (token == null)
            {
                return null;
            }

            if (token.Certificate.Thumbprint == null)
            {
                return null;
            }
            string tokenThumbprint = token.Certificate.Thumbprint.ToLowerInvariant();
            if (tokenThumbprint != AcceptedThumbprint)
            {
                return null;
            }
            return Issuer;
        }
    }
}