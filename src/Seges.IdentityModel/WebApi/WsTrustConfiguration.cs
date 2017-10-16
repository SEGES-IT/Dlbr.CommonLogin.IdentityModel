using System;

namespace Seges.IdentityModel.WebApi
{
    public class WsTrustConfiguration
    {
        // Bootstrap or not (and credentials for it)
        // Integrated or not (cannot support currently)
        public string AdfsDns { get; set; }
        public string Audience { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public TimeSpan TokenCacheTime { get; set; }
    }
}