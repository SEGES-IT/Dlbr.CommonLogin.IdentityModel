using System;
using Seges.IdentityModel.Utils;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Seges.IdentityModel.Log.Logging;

namespace Seges.IdentityModel.WebApi
{
    public class WebApiClient<TConfiguration> where TConfiguration : IWebApiConfiguration, IHttpRequestMessagePreparer
    {
        private static readonly ILog Log = LogProvider.For<WebApiClient<TConfiguration>>();

        private readonly HttpClient _httpClient;
        private readonly TConfiguration _configuration;

        public WebApiClient(TConfiguration configuration, HttpClient httpClient) 
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (configuration.Endpoint == null) throw new ArgumentNullException($"{nameof(configuration)}.{nameof(configuration.Endpoint)}");
            if (!configuration.Endpoint.IsAbsoluteUri)
            {
                throw new ArgumentOutOfRangeException(nameof(configuration),$"configuration.Endpoint value ({configuration.Endpoint}) should be an absolute Uri");
            }
            _configuration = configuration;
        }

        public StringContent JsonSerialize<T>(T payload)
        {
            var buffer = JsonConvert.SerializeObject(payload);
            var content = new StringContent(buffer, Encoding.UTF8, "application/json");
            return content;
        }

        public async Task<WebApiResponse<TResult>> PostAsJson<TResult,T>(string relativeUri, T request)
        {
            var payload = JsonSerialize(request);
            var responsePair = await SendPreparedAsync(relativeUri, HttpMethod.Post, payload);
            return new WebApiResponse<TResult>(responsePair.Item1, responsePair.Item2);
        }

        public async Task<WebApiResponse> PostAsJsonFireAndForget<T>(string relativeUri, T request)
        {
            var payload = JsonSerialize(request);
            var responsePair = await SendPreparedAsync(relativeUri, HttpMethod.Post, payload);
            return new WebApiResponse(responsePair.Item1);
        }

        public async Task<WebApiResponse<TResult>> Get<TResult>(string relativeUri)
        {
            var responsePair = await SendPreparedAsync(relativeUri);
            return new WebApiResponse<TResult>(responsePair.Item1, responsePair.Item2);
        }

        public async Task<WebApiResponse> GetFireAndForget(string relativeUri)
        {
            var responsePair = await SendPreparedAsync(relativeUri);
            return new WebApiResponse(responsePair.Item1);
        }

        private async Task<(HttpResponseMessage, string)> SendPreparedAsync(string relativeUri, HttpMethod method = null,  HttpContent content = null)
        {
            method = method ?? HttpMethod.Get;
            var uri = new Uri(_configuration.Endpoint, new Uri(relativeUri, UriKind.RelativeOrAbsolute));
            var requestMessage = new HttpRequestMessage(method, uri);
            if (content != null)
            {
                requestMessage.Content = content;
            }
            Log.Info($"HTTP {method.Method} {_httpClient.BaseAddress} -> {relativeUri}");
            if (Log.IsDebugEnabled() && content != null)
            {
                Log.Debug("Content:");
                Log.Debug(await content.ReadAsStringAsync());
            }
            await _configuration.PrepareRequest(requestMessage);
            var response = await _httpClient.SendAsync(requestMessage);
            Log.Debug($"{(int)response.StatusCode} {response.StatusCode}");
            var responseContent = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode(responseContent);
            return (response, responseContent);
        }
    }
}
