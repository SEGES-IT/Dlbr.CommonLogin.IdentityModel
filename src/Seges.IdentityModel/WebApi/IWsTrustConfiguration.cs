using System;

namespace Seges.IdentityModel.WebApi
{
    public interface IWsTrustConfiguration
    {
        string AdfsDns { get;}
        string Audience { get; }
        string Password { get; }
        TimeSpan TokenCacheTime { get; }
        string Username { get;  }
    }
}