using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Seges.IdentityModel.WsTrust
{
    public class RstrParser
    {
        private Regex AssertionPattern { get; } = new Regex("(<saml:Assertion.+?</saml:Assertion>)");
        private Regex RstrPattern { get; } = new Regex("(<trust:RequestSecurityTokenResponse.+?</trust:RequestSecurityTokenResponse>)");
        public SamlTokenResponse Parse(string rstr)
        {
            var rstrMatch = this.RstrPattern.Match(rstr);
            if (!rstrMatch.Success)
            {
                throw new Exception($"Could not find pattern {this.RstrPattern} in text {rstr}");
            }

            var assertionMatch = this.AssertionPattern.Match(rstr);
            if (!assertionMatch.Success)
            {
                throw new Exception($"Could not find pattern {this.AssertionPattern} in text {rstr}");
            }
            return new SamlTokenResponse (rstrMatch.Value, assertionMatch.Value);


        }
    }
}