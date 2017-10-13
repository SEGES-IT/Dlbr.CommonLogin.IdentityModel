using System;

namespace Seges.IdentityModel.WebApi
{
    public class WsTrustConfiguration
    {
        // Bootstrap or not (and credentials for it)
        // Integrated or not (cannot support currently)
        public WsTrustConfiguration(string adfsDns, string audience, string username, string password, TimeSpan tokenCacheTime)
        {
            AdfsDns = adfsDns;
            Audience = audience;
            Username = username;
            Password = password;
            TokenCacheTime = tokenCacheTime;
        }

        public string AdfsDns { get; }
        public string Audience { get; }
        public string Username { get;}
        public string Password { get; set; }
        public TimeSpan TokenCacheTime { get; }

    }
}