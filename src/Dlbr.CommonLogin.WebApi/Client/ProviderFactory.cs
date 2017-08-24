using System;

namespace Dlbr.CommonLogin.WebApi.Client
{
    public abstract class ProviderFactory<TProvider>
    {
        public TProvider Create(Type type)
        {
            if (!typeof(TProvider).IsAssignableFrom(type))
            {
                throw new Exception(String.Format("This factory can only create instances of {0}", typeof(TProvider).FullName));
            }
            return CreateInstance(type);
        }

        protected abstract TProvider CreateInstance(Type type);
    }
}
