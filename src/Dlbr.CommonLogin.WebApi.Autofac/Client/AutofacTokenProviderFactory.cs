using Autofac;
using Dlbr.CommonLogin.WebApi.Client;

namespace Dlbr.CommonLogin.WebApi.Autofac.Client
{
    internal class AutofacTokenProviderFactory : AutofacProviderFactory<ITokenProvider>, ITokenProviderFactory
    {
        public AutofacTokenProviderFactory(IComponentContext componentContext)
            : base(componentContext)
        {
        }
    }
}
