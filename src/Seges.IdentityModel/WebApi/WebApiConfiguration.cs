using System;

namespace Seges.IdentityModel.WebApi
{
    public abstract class WebApiConfiguration
    {
        public WsTrustConfiguration WsTrustConfiguration { get; set; }
        public Uri ServiceBaseUri { get; set; }
    }
}