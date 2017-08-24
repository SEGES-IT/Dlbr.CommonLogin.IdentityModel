using System;

namespace Dlbr.CommonLogin.WebApi.Client
{
    public interface ITokenProviderFactory
    {
        ITokenProvider Create(Type type);
    }
}
