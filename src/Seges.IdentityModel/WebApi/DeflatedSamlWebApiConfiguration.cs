using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Seges.IdentityModel.WebApi
{
    public class DeflatedSamlWebApiConfiguration : IHttpRequestMessagePreparer, IWebApiConfiguration
    {
        private readonly WsTrustTokenProvider _tokenProvider;

        protected DeflatedSamlWebApiConfiguration(WsTrustTokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        public WsTrustConfiguration WsTrustConfiguration { get; set; }

        public async Task PrepareRequest(HttpRequestMessage requestMessage)
        {

            var token = await _tokenProvider.DeflatedSaml();
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public Uri Endpoint { get; set; }
    }
}