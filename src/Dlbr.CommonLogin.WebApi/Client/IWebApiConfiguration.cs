using System;
using System.Threading.Tasks;

namespace Dlbr.CommonLogin.WebApi.Client
{
    public interface IWebApiConfiguration
    {
        Uri Endpoint { get; }

        void Configure<TConfiguration>(IWebApiClient<TConfiguration> client)
            where TConfiguration : IWebApiConfiguration;
        Task Prepare<TConfiguration>(IWebApiClient<TConfiguration> client)
            where TConfiguration : IWebApiConfiguration;
    }
}
