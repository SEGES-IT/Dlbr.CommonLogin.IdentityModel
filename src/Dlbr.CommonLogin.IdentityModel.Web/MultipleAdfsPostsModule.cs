using System;
using System.IdentityModel.Services;
using System.Linq;
using System.Web;

namespace Dlbr.CommonLogin.IdentityModel.Web
{
    /// <summary>
    /// Handles POSTs of a SignIn response to an already authenticated RP by redirecting to the reply url instead of throwing an exception
    /// This situation typically arise when the same RP is displayed more than once on the same page in two or more iframes, e.g. on Landmand.dk
    /// </summary>
    public class MultipleAdfsPostsModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.EndRequest += EndRequest;
            FederatedAuthentication.SessionAuthenticationModule.SessionSecurityTokenReceived += SessionSecurityTokenReceived;
        }

        public void Dispose()
        {
        }

        private void SessionSecurityTokenReceived(object sender, SessionSecurityTokenReceivedEventArgs e)
        {
            HttpContext.Current.Items["WSSessionSecurityTokenReceived"] = true;
        }

        private void EndRequest(object sender, EventArgs e)
        {
            if (FederatedAuthentication.SessionAuthenticationModule == null ||
                FederatedAuthentication.WSFederationAuthenticationModule == null)
            {
                //We need both modules
                return;
            }

            if (!FederatedAuthentication.WSFederationAuthenticationModule.IsSignInResponse(new HttpRequestWrapper(HttpContext.Current.Request)))
            {
                //We're only in it to handle duplicate sign in responses
                return;
            }

            object o = HttpContext.Current.Items["WSSessionSecurityTokenReceived"];
            if (o == null)
            {
                return;
            }

            HttpContext context = HttpContext.Current;

            string wctx = context.Request.Form != null ? context.Request.Form["wctx"] : null;
            if (String.IsNullOrEmpty(wctx))
            {
                //We need a wctx form value
                return;
            }

            string pair = GetFirstKeyValuePairWithKeyRU(wctx);

            if (String.IsNullOrEmpty(pair))
            {
                //No return url found in wctx
                return;
            }

            string url = ExtractUrl(pair);
            if (String.IsNullOrEmpty(url))
            {
                //No return url found (&ru=)
                return;
            }

            HttpContext.Current.Response.Redirect(HttpContext.Current.Server.UrlDecode(url), false);
        }

        private string ExtractUrl(string pair)
        {
            return pair.Split('=')[1];
        }

        private string GetFirstKeyValuePairWithKeyRU(string wctx)
        {
            return wctx.Split('&') //wctx is formatted as querystring, so split on '&'
                .Select(x => x.Trim()) //Trim all key/value pairs for leading and trailing whitespace
                .Where(x => !String.IsNullOrEmpty(x)) //Only include actual key/value pairs
                .FirstOrDefault(x => x.StartsWith("ru=", StringComparison.OrdinalIgnoreCase));
        }
    }
}