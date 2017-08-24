using System;
using System.IdentityModel.Tokens;

namespace Dlbr.CommonLogin.WebApi.Client
{
    public interface ISecureWebApiConfiguration : IWebApiConfiguration
    {
        string AdfsDnsName { get; }
        string Realm { get; }
        string Username { get; }
        string Password { get; }
        bool ActAs { get; }

        ITokenProvider TokenProvider { get; }
    }
}
