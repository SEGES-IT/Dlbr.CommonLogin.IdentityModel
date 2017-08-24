using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Xml;

namespace Dlbr.CommonLogin.WebApi.Client
{
    internal static class BootstrapTokenProvider
    {
        public static SecurityToken GetBootstrapTokenFromContext()
        {
            var identity = ClaimsPrincipal.Current.Identities.FirstOrDefault();
            var context = identity?.BootstrapContext as BootstrapContext;
            if (context == null)
            {
                return null;
            }
            SecurityToken token = null;
            if (context.SecurityToken != null)
            {
                token = context.SecurityToken;
            }
            else if (!string.IsNullOrWhiteSpace(context.Token))
            {
                var handlers = SecurityTokenHandlerCollection.CreateDefaultSecurityTokenHandlerCollection();
                token = handlers.ReadToken(new XmlTextReader(new StringReader(context.Token)));
            }
            return token;
        }
    }
}