namespace Seges.IdentityModel.WsTrust
{
    public class SamlTokenResponse
    {
        public SamlTokenResponse(string rstr, string tokenXml)
        {
            TokenXml = tokenXml;
            Rstr = rstr;
        }

        public string TokenXml { get; }
        public string Rstr { get; } 
    }
}