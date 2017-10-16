using System;

namespace Seges.IdentityModel.WebApi
{
    public interface IWebApiConfiguration
    {
        Uri Endpoint { get; }
    }
}