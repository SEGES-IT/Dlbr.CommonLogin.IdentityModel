using System;

namespace Dlbr.CommonLogin.WebApi.Client
{
    internal abstract class ReflectionProviderFactory<TProvider> : ProviderFactory<TProvider>
    {
        protected override TProvider CreateInstance(Type type)
        {
            return (TProvider)Activator.CreateInstance(type);
        }
    }

}
