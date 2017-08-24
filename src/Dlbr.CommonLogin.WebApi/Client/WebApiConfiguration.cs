using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dlbr.CommonLogin.WebApi.Client
{
    public abstract class WebApiConfiguration : IWebApiConfiguration
    {
        public abstract Uri Endpoint { get; }

        public virtual void Configure<TConfiguration>(IWebApiClient<TConfiguration> client)
            where TConfiguration : IWebApiConfiguration
        {
            client.BaseAddress = this.Endpoint;
        }

        public virtual Task Prepare<TConfiguration>(IWebApiClient<TConfiguration> client)
            where TConfiguration : IWebApiConfiguration
        {
            return Task.FromResult((object)null);
        }
    }
}
