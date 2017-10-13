namespace Seges.IdentityModel.WsTrust
{
    public class SamlActAsTokenRequest : SamlTokenRequest
    {
        public string BootstrapTokenXml { get; set; }
    }
}