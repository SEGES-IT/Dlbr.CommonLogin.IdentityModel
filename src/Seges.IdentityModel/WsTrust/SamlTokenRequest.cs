namespace Seges.IdentityModel.WsTrust
{
    public class SamlTokenRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Audience { get; set; }
    }
}