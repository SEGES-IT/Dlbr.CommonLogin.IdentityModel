using System;
using System.Threading.Tasks;
using Seges.IdentityModel.DeflatedSaml;
using Seges.IdentityModel.Log.Logging;
using Seges.IdentityModel.WsTrust;

namespace Seges.IdentityModel.WebApi
{
    public class WsTrustTokenProvider<TConfiguration> where TConfiguration : WebApiConfiguration
    {
        private static readonly ILog Log = LogProvider.For<WebApiClient<TConfiguration>>();

        private readonly WsTrustClient _wsTrustClient;
        private readonly TConfiguration _configuration;
        private SamlTokenResponse _token = new SamlTokenResponse(string.Empty, string.Empty);
        private DateTime _expires = DateTime.MinValue;

        public WsTrustTokenProvider(TConfiguration configuration)
        {
            _configuration = configuration;
            _wsTrustClient = new WsTrustClient(configuration.WsTrustConfiguration.AdfsDns);
        }

        public async Task RefreshToken()
        {
            Log.Debug("Refreshing token");
            var response = await _wsTrustClient.RequestTokenAsync(new SamlTokenRequest()
            {
                Username = _configuration.WsTrustConfiguration.Username,
                Password = _configuration.WsTrustConfiguration.Password,
                Audience = _configuration.WsTrustConfiguration.Audience
            });
            _expires = DateTime.UtcNow.Add(_configuration.WsTrustConfiguration.TokenCacheTime);
            Log.Debug($"Token expires at {_expires:o}");
            _token = response;
        }

        private async Task EnsureToken()
        {
            var tokenExpired = DateTime.UtcNow > _expires;
            if (tokenExpired)
            {
                await RefreshToken();
            }
        }

        public async Task<SamlTokenResponse> SamlTokenResponse()
        {
            await EnsureToken();
            return _token;
        }
        public async Task<string> DeflatedSaml()
        {
            await EnsureToken();
            return new DeflatedSamlEncoder().Encode(_token.TokenXml);
        }
    }
}