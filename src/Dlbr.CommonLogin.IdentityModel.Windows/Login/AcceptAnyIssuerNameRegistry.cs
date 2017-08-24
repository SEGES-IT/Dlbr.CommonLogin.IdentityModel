using System.IdentityModel.Tokens;

namespace Dlbr.CommonLogin.IdentityModel.Windows.Login
{
    internal class AcceptAnyIssuerNameRegistry : IssuerNameRegistry
    {
        public override string GetIssuerName(SecurityToken securityToken)
        {
            var token = securityToken as X509SecurityToken;
            if (token == null)
            {
                return "No issuer";
            }
            return token.Certificate.Thumbprint;
        }
    }
}