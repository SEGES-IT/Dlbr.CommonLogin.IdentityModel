using System;
using System.Collections.ObjectModel;
using System.IdentityModel.Configuration;
using System.IdentityModel.Selectors;
using System.IdentityModel.Services;
using System.IO;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.ServiceModel.Security;
using System.Text;
using System.Web;
using System.Xml;

namespace Dlbr.CommonLogin.IdentityModel
{
    public static class SecurityTokenUtilities
	{
		#region Public static methods

		public static string SerializeToken(SecurityToken token, bool urlEncode = false)
        {
            StringBuilder sb = new StringBuilder();
            if (token is GenericXmlSecurityToken)
            {
                WSSecurityTokenSerializer wss = new WSSecurityTokenSerializer(SecurityVersion.WSSecurity11);
                XmlWriter writer = XmlWriter.Create(sb);
                wss.WriteToken(writer, token);
            }
            else
            {
                SecurityTokenHandlerCollection handlers = FederatedAuthentication.FederationConfiguration.IdentityConfiguration.SecurityTokenHandlers;
                handlers.WriteToken(new XmlTextWriter(new StringWriter(sb)), token);
            }

            string serialized = sb.ToString();
            return urlEncode ? HttpUtility.UrlEncode(serialized) : serialized;
		}

        public static ReadOnlyCollection<ClaimsIdentity> ReadAndValidateSamlToken(string token)
		{
			return ValidateToken(token);
		}

		public static string WriteSamlToken(SecurityToken token)
		{
			using (var stringWriter = new StringWriter())
			using (var writer = XmlWriter.Create(stringWriter))
			{
				if (writer == null)
					throw new Exception("Could not create XMlWriter based on empty stringwriter");

				var handler = new SamlSecurityTokenHandler();
				handler.WriteToken(writer, token);

				writer.Flush();
				var tokenString = stringWriter.ToString();
				return tokenString;
			}
		}

		#endregion

		#region Private static helpers

        private static ReadOnlyCollection<ClaimsIdentity> ValidateToken(string token)
		{
			using (var stringReader = new StringReader(token))
			{
				using (var reader = XmlReader.Create(stringReader))
				{
					var serviceConfiguration = new IdentityConfiguration(true)
					{
						CertificateValidator = X509CertificateValidator.None,
                        SaveBootstrapContext = true
					};

					foreach (var securityTokenHandler in serviceConfiguration.SecurityTokenHandlers)
					{
						if (securityTokenHandler.GetType().Name == "Saml11SecurityTokenHandler")
						{
							var samlToken = securityTokenHandler.ReadToken(reader);
							return securityTokenHandler.ValidateToken(samlToken);
						}
					}
				}
			}

			throw new Exception("Could not find Saml11SecurityTokenHandler");
		}

		#endregion
	}
}
