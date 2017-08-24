using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Dlbr.CommonLogin.WebApi.Client
{
    public class WebApiClient<TConfiguration> : IWebApiClient<TConfiguration>, IDisposable
        where TConfiguration : IWebApiConfiguration
    {
        public WebApiClient(TConfiguration configuration, HttpClient httpClient)
        {
            this.HttpClient = httpClient;
            this.Configuration = configuration;
            this.Configuration.Configure(this);
        }

        private TConfiguration Configuration { get; set; }
        private HttpClient HttpClient { get; set; }

        public Uri BaseAddress
        {
            get { return this.HttpClient.BaseAddress; }
            set { this.HttpClient.BaseAddress = value; }
        }

        public async Task<HttpResponseMessage> PostAsJson<T>(string uri, T t)
        {
            await this.Configuration.Prepare(this).ConfigureAwait(false);
            return await this.HttpClient.PostAsJsonAsync(uri, t).ConfigureAwait(false);
        }

        public async Task<HttpResponseMessage> Get(string uri)
        {
            await this.Configuration.Prepare(this).ConfigureAwait(false);
            return await this.HttpClient.GetAsync(uri).ConfigureAwait(false);
        }

        public async Task<HttpResponseMessage> Delete(string uri)
        {
            await this.Configuration.Prepare(this).ConfigureAwait(false);
            return await this.HttpClient.DeleteAsync(uri).ConfigureAwait(false);
        }

        public HttpRequestHeaders DefaultRequestHeaders
        {
            get { return this.HttpClient.DefaultRequestHeaders; }
        }

        #region IDisposable

        private bool _disposed = false;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this._disposed) return;

            if (disposing)
            {
                if (this.HttpClient != null)
                {
                    this.HttpClient.Dispose();
                }
            }

            this._disposed = true;
        }

        #endregion
    }
}
