using Dlbr.CommonLogin.IdentityModel.WebApi;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Dlbr.CommonLogin.WebApi.Client
{
    public abstract class SecureWebApiConfiguration : WebApiConfiguration, ISecureWebApiConfiguration
    {
        public SecureWebApiConfiguration(ITokenProviderFactory tokenProviderFactory)
        {
            if (this.TokenProviderType == null)
            {
                throw new ArgumentNullException("TokenProviderType");
            }

            try
            {
                this.TokenProvider = tokenProviderFactory.Create(this.TokenProviderType);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Failed to create token provider of type '{0}'", this.TokenProviderType.FullName), ex);
            }
        }

        public abstract string AdfsDnsName { get; }
        public abstract string Realm { get; }
        public abstract string Username { get; }
        public abstract string Password { get; }
        public abstract bool ActAs { get; }
        public abstract Type TokenProviderType { get; }

        public ITokenProvider TokenProvider { get; private set; }

        private DateTimeOffset TokenValidTo { get; set; }

        protected virtual bool TokenIsUseable()
        {
            return DateTimeOffset.Now.AddMinutes(3) < this.TokenValidTo;
        }

        private async Task RefreshToken<TConfiguration>(IWebApiClient<TConfiguration> client)
            where TConfiguration : IWebApiConfiguration
        {
            var token = await this.TokenProvider.GetToken(this).ConfigureAwait(false);
            var encoder = new DeflatedSamlTokenHeaderEncoder();
            var bearer = encoder.Encode(((GenericXmlSecurityToken)token).TokenXml.OuterXml);

            this.TokenValidTo = token.ValidTo;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);
        }

        protected virtual bool IsTokenUseable()
        {
            return this.TokenValidTo != null && DateTimeOffset.Now.AddMinutes(3) < this.TokenValidTo;
        }

        public override async Task Prepare<TConfiguration>(IWebApiClient<TConfiguration> client)
        {
            await base.Prepare<TConfiguration>(client).ConfigureAwait(false);
            if (!IsTokenUseable())
            {
                await this.RefreshToken(client).ConfigureAwait(false);
            }
        }
    }
}
