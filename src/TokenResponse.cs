using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;

namespace Dlbr.CommonLogin.IdentityModel
{
    public class TokenResponse
    {
        public TokenResponse(SecurityToken token, RequestSecurityTokenResponse rstr)
        {
            Token = token;
            Rstr = rstr;
        }

        public SecurityToken Token { get; private set; }
        public RequestSecurityTokenResponse Rstr { get; private set; }
    }
}