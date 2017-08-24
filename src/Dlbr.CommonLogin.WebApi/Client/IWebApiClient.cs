using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Dlbr.CommonLogin.WebApi.Client
{
    public interface IWebApiClient<TConfiguration>
        where TConfiguration : IWebApiConfiguration
    {
        Uri BaseAddress { get; set; }
        HttpRequestHeaders DefaultRequestHeaders { get; }

        Task<HttpResponseMessage> PostAsJson<T>(string uri, T t);
        Task<HttpResponseMessage> Get(string uri);

        Task<HttpResponseMessage> Delete(string uri);
    }
}
