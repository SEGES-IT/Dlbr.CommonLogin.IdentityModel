using System;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.Reflection;
using Dlbr.CommonLogin.IdentityModel.Web.Logging;


namespace Dlbr.CommonLogin.IdentityModel.Web
{
    public class CacheMissAwareSessionAuthenticationModule : SessionAuthenticationModule
    {
        private static readonly ILog Log = LogProvider.For<CacheMissAwareSessionAuthenticationModule>();


        protected override void OnAuthenticateRequest(object sender, EventArgs eventArgs)
        {
            // See http://stackoverflow.com/questions/20639344/message-id4243-could-not-create-a-securitytoken-a-token-was-not-found-in-the
            try
            {
                base.OnAuthenticateRequest(sender, eventArgs);
            }
            catch (SecurityTokenException ex)
            {
                // ID4243: Could not create a SecurityToken. A token was not found in the token cache and no cookie was found in the context.
                if (ex.Message.IndexOf("ID4243", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    // Returning directly without setting any token will cause the FederationAuthenticationModule
                    // to redirect back to the token issuer.
                    Log.InfoException("Token cache backing store cache miss, probably a stale cookie referencing a deleted entry in the database", ex);
                    FederatedAuthentication.SessionAuthenticationModule.DeleteSessionTokenCookie();
                    return;
                }
                throw;
            }
        }
    }
}