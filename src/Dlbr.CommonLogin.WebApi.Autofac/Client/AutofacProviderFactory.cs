using System;
using Autofac;
using Dlbr.CommonLogin.WebApi.Client;

namespace Dlbr.CommonLogin.WebApi.Autofac.Client
{
    internal abstract class AutofacProviderFactory<TProvider> : ProviderFactory<TProvider>
    {
        protected AutofacProviderFactory(IComponentContext componentContext)
        {
            this.ComponentContext = componentContext;
        }

        private IComponentContext ComponentContext { get; set; }

        protected override TProvider CreateInstance(Type type)
        {
            return (TProvider)this.ComponentContext.Resolve(type);
        }
    }
}
