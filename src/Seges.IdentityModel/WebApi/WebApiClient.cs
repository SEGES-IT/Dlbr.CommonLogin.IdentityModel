using System;
using Seges.IdentityModel.Utils;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Seges.IdentityModel.Log.Logging;

namespace Seges.IdentityModel.WebApi
{
    public class WebApiClient<TConfiguration> where TConfiguration : WebApiConfiguration
    {
        private static readonly ILog Log = LogProvider.For<WebApiClient<TConfiguration>>();

        private readonly HttpClient _httpClient;
        private readonly WsTrustTokenProvider<TConfiguration> _tokenProvider;

        public WebApiClient(TConfiguration configuration, HttpClient httpClient, WsTrustTokenProvider<TConfiguration> tokenProvider) 
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = configuration.ServiceBaseUri;
            _tokenProvider = tokenProvider;
        }

        public StringContent JsonSerialize<T>(T payload)
        {
            var buffer = JsonConvert.SerializeObject(payload);
            var content = new StringContent(buffer, Encoding.UTF8, "application/json");
            return content;
        }

        public T JsonDeserialize<T>(string buffer)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(buffer);
            }
            catch (JsonException ex)
            {
                throw new Exception($"Unable to deserialize type {typeof(T).FullName} from JSON {buffer}", ex);
            }
        }


        public async Task<TResult> PostAsJson<TResult,T>(string relativeUri, T request)
        {
            var payload = JsonSerialize(request);
            var responsePair = await SendAuthenticatedAsync(relativeUri, HttpMethod.Post, payload);
            responsePair.Item1.EnsureSuccessStatusCode(responsePair.Item2);
            var obj = JsonDeserialize<TResult>(responsePair.Item2);
            return obj;
        }

        public async Task<TResult> Get<TResult>(string relativeUri)
        {
            var responsePair = await SendAuthenticatedAsync(relativeUri);
            responsePair.Item1.EnsureSuccessStatusCode(responsePair.Item2);
            var obj = JsonDeserialize<TResult>(responsePair.Item2);
            return obj;
        }
        
        private async Task<(HttpResponseMessage, string)> SendAuthenticatedAsync(string relativeUri, HttpMethod method = null,  HttpContent content = null)
        {
            method = method ?? HttpMethod.Get;
            var requestMessage = new HttpRequestMessage(method, relativeUri);
            if (content != null)
            {
                requestMessage.Content = content;
            }
            Log.Info($"HTTP {method.Method} {_httpClient.BaseAddress} -> {relativeUri}");
            if (Log.IsDebugEnabled() && content != null)
            {
                Log.Debug("Content:");
                Log.Debug(await (content ?? new StringContent("")).ReadAsStringAsync());
            }
            var token = await _tokenProvider.DeflatedSaml();
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.SendAsync(requestMessage);
            Log.Debug($"{(int)response.StatusCode} {response.StatusCode}");
            var responseContent = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode(responseContent);
            return (response, responseContent);
        }
    }
}
