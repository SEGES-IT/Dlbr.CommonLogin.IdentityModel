using System;
using System.Net.Http;
using Newtonsoft.Json;
using Seges.IdentityModel.Utils;

namespace Seges.IdentityModel.WebApi
{
    public class WebApiResponse<TResult> : WebApiResponse
    {
        public string Content { get; }
        public TResult Typed { get; }

        public WebApiResponse(HttpResponseMessage httpResponseMessage, string content) : base(httpResponseMessage)
        {
            Content = content;
            Typed = JsonDeserialize<TResult>(content);
        }

        public new void EnsureSuccessStatusCode()
        {
            this.HttpResponseMessage.EnsureSuccessStatusCode(Content);
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
    }

    public class WebApiResponse
    {
        public WebApiResponse(HttpResponseMessage httpResponseMessage)
        {
            HttpResponseMessage = httpResponseMessage;
        }

        public void EnsureSuccessStatusCode()
        {
            this.HttpResponseMessage.EnsureSuccessStatusCode();
        }
        public HttpResponseMessage HttpResponseMessage { get; }
    }
}