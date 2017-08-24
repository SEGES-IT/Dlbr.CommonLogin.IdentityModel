using System;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Hosting;
using Dlbr.CommonLogin.Owin;
using Microsoft.IdentityModel.Extensions;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.WsFederation;
using Microsoft.Owin;

namespace Owin
{
    public static class AppBuilderExtensions
    {
        private const string AppSettingsKeyRealm = "ida:Wtrealm";

        private const string AppSettingsKeyAdfsMetadata = "ida:ADFSMetadata";

        public static WsFederationAuthenticationOptions CreateDefaultWSFederationOptionsFromConfig(this IAppBuilder app)
        {
            string realm = ConfigurationManager.AppSettings[AppSettingsKeyRealm];
            string adfsMetadata = ConfigurationManager.AppSettings[AppSettingsKeyAdfsMetadata];
            return app.CreateDefaultWSFederationOptions(realm, adfsMetadata);
        }

        public static WsFederationAuthenticationOptions CreateDefaultWSFederationOptions(this IAppBuilder app, string realm, string adfsMetadata)
        {
            return new WsFederationAuthenticationOptions
            {
                Wtrealm = realm,
                MetadataAddress = adfsMetadata,
                TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = ClaimTypes.NameIdentifier
                },
                Notifications = new WsFederationAuthenticationNotifications
                {
                    RedirectToIdentityProvider = args =>
                    {
                        DoNotRedirectToIdpWhenAuthenticatedButNotAuthorized(args);
                        DoNotRedirectToIdpWhenAuthorizationHeaderIsPresent(args);
                        return Task.FromResult(0);
                    },
                    SecurityTokenValidated = args =>
                    {
                        StripSchemeFromReturnUrl(args);
                        return Task.FromResult<object>(null);
                    }
                }
            };
        }

        private static void DoNotRedirectToIdpWhenAuthorizationHeaderIsPresent(RedirectToIdentityProviderNotification<WsFederationMessage, WsFederationAuthenticationOptions> args)
        {
            if (args.OwinContext.Request.Headers.ContainsKey("Authorization"))
            {
                args.State = NotificationResultState.HandledResponse;
            }
        }

        private static void DoNotRedirectToIdpWhenAuthenticatedButNotAuthorized(RedirectToIdentityProviderNotification<WsFederationMessage, WsFederationAuthenticationOptions> args)
        {
            if (args.OwinContext.Authentication.User.Identity.IsAuthenticated && args.ProtocolMessage.IsSignInMessage)
            {
                args.State = NotificationResultState.HandledResponse;
            }
        }

        private static void StripSchemeFromReturnUrl(SecurityTokenValidatedNotification<WsFederationMessage, WsFederationAuthenticationOptions> args)
        {
            try
            {
                var uri = new Uri(args.AuthenticationTicket.Properties.RedirectUri);
                args.AuthenticationTicket.Properties.RedirectUri = String.Concat(uri.PathAndQuery, uri.Fragment);
            }
            catch (Exception) { }
        }


        public static Func<string, ClaimsPrincipal> CreateOwinWsFederationTokenValidatorFromConfig(this IAppBuilder app)
        {
            string realm = ConfigurationManager.AppSettings[AppSettingsKeyRealm];
            string adfsMetadata = ConfigurationManager.AppSettings[AppSettingsKeyAdfsMetadata];
            return app.CreateOwinWsFederationTokenValidator(realm, adfsMetadata);
        }

        public static Func<string, ClaimsPrincipal> CreateOwinWsFederationTokenValidator(this IAppBuilder app, string wtrealm, string metadataAddress)
        {
            // Need to supply an HttpClient instance, otherwise fails with "Digest verification failed for Reference '#_<guid>'. sometimes???
            using (var httpClient = new HttpClient())
            {
                var configurationManager = new ConfigurationManager<WsFederationConfiguration>(metadataAddress, httpClient);
                var wsFederationConfiguration = configurationManager.GetConfigurationAsync().Result;
                var tokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = ClaimTypes.NameIdentifier,
                    IssuerSigningKeys = wsFederationConfiguration.SigningKeys,
                    ValidIssuer = wsFederationConfiguration.Issuer,
                    ValidAudience = wtrealm
                };
                Func<string, ClaimsPrincipal> tokenValidator = tokenString =>
                {
                    SecurityToken securityToken;
                    var handlers = SecurityTokenHandlerCollectionExtensions.GetDefaultHandlers();
                    var principal = handlers.ValidateToken(tokenString, tokenValidationParameters, out securityToken);
                    return principal;
                };
                return tokenValidator;
            }
        }

        public static IAppBuilder UseDeflatedSamlBearerAuthentication(this IAppBuilder app)
        {
            app.UseDeflatedSamlBearerAuthentication(app.CreateOwinWsFederationTokenValidatorFromConfig());
            return app;
        }

        public static IAppBuilder UseDeflatedSamlBearerAuthentication(this IAppBuilder app, Func<string, ClaimsPrincipal> tokenValidator)
        {
            if (tokenValidator == null) throw new ArgumentNullException("tokenValidator");
            var options = new OAuthBearerAuthenticationOptions
            {
                AccessTokenFormat = new DeflatedSamlStrippingTokenFormat(tokenValidator, app.CreateLogger<DeflatedSamlStrippingTokenFormat>())
            };
            app.UseOAuthBearerAuthentication(options);
            return app;
        }

        public static CookieAuthenticationOptions CreateDefaultCookieAuthenticationOptions(this IAppBuilder app, string connectionStringName = "SecurityTokenCache")
        {
            var options = new CookieAuthenticationOptions()
            {

                //Workaround to problem described here http://stackoverflow.com/questions/20737578/asp-net-sessionid-owin-cookies-do-not-send-to-browser/21234614#21234614
                CookieManager = new SystemWebCookieManager(),
                CookieSecure = CookieSecureOption.Never,
                SessionStore = new DbAuthenticationSessionStore(
                    new TicketDataFormat(app.CreateDataProtector(typeof(DbAuthenticationSessionStore).FullName)),
                    ConfigurationManager.ConnectionStrings[connectionStringName],
                    app.CreateLogger<DbAuthenticationSessionStore>())
            };
            // https://jira.seges.dk/browse/SVAT-81 workaround. Can be scrapped if a release containing this fix
            // https://github.com/aspnet/Security/blob/d6b82b87993489d5be1b48d6f46ee6d5cc034a9e/src/Microsoft.AspNet.Security.Cookies/CookieAuthenticationOptions.cs
            // is ever released.
            if (!string.IsNullOrWhiteSpace(HostingEnvironment.ApplicationVirtualPath))
            {
                options.CookiePath = HostingEnvironment.ApplicationVirtualPath;
            }

            return options;

        }

        public static IAppBuilder UseCookieAuthenticationWithDbStore(this IAppBuilder app, string connectionStringName = "SecurityTokenCache")
        {
            var cookieOptions = app.CreateDefaultCookieAuthenticationOptions(connectionStringName);
            app.UseCookieAuthentication(cookieOptions);
            return app;
        }

        public static IAppBuilder UseWsFederationSignout(this IAppBuilder app, Func<IOwinContext, Task> signOutCallback = null)
        {
            // http://leastprivilege.com/2015/07/08/federated-logout-with-the-katana-ws-federation-middleware/
            app.Use(async (ctx, next) =>
            {
                var wa = ctx.Request.Query.Get("wa");
                if (wa != null && wa == "wsignoutcleanup1.0")
                {
                    if (signOutCallback != null)
                    {
                        await signOutCallback(ctx);
                    }
                    ctx.Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
                }
                await next();
            });
            return app;
        }

        public static IAppBuilder UseWsFederationAndCookieAuthenticationWithDefaults(this IAppBuilder app, WsFederationAuthenticationOptions wsFederationAuthenticationOptions = null, CookieAuthenticationOptions cookieAuthenticationOptions = null, Func<IOwinContext, Task> signOutCallback = null)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(cookieAuthenticationOptions ?? app.CreateDefaultCookieAuthenticationOptions());
            app.UseWsFederationAuthentication(wsFederationAuthenticationOptions ?? app.CreateDefaultWSFederationOptionsFromConfig());
            app.UseWsFederationSignout(signOutCallback);
            return app;
        }
    }
}