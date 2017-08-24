using System.Configuration;
using System.IdentityModel.Services;
using System.IO;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.ServiceModel;
using System.Threading;
using System.Xml;
using Dlbr.CommonLogin.IdentityModel.Windows.ValidateDLIUserService;

namespace Dlbr.CommonLogin.IdentityModel.Windows.Login
{
    public class AdfsLogin
    {
        private static LoginForm form = null;
        private static string tokenOutput;
        /// <summary>
        /// This method brings up the standard ADFS logon dialog for the user to login. 
        /// If login is successful the token string will be returned.
        /// </summary>
        /// <param name="adfs">ADFS options</param>
        /// <param name="options">Options for the logon dialog</param>
        /// <returns>SAML token string</returns>
        public static string LoginAndReturnTokenString(AdfsOptions adfs, LoginOptions options = null)
        {
            options = options ?? new LoginOptions();
            options.TokenOutput = TokenOutput.ReturnTokenString;
            return Authenticate(adfs,options);
        }

        /// <summary>
        /// This method brings up the standard ADFS logon dialog for the user to login. 
        /// If login is successful the token will be returned as a GenericXmlSecurityToken, suitable for use with WCF.
        /// </summary>
        /// <param name="adfs">ADFS options</param>
        /// <param name="options">Options for the logon dialog</param>
        /// <returns>GenericXmlSecurityToken token</returns>
        public static GenericXmlSecurityToken LoginAndReturnGenericXmlSecurityToken(AdfsOptions adfs, LoginOptions options = null)
        {
            options = options ?? new LoginOptions();
            options.TokenOutput = TokenOutput.ReturnRstr;
            var rstr = Authenticate(adfs, options);
            if (rstr == null)
            {
                return null;
            }
            return new RstrHelper().DeserializeTokenFromRstrString(rstr);
        }

        private static string Authenticate(AdfsOptions adfs, LoginOptions options)
        {
            if (TokenIssued())
            {
                return tokenOutput;
            }
            
            if (string.IsNullOrEmpty(adfs.Realm))
            {
                adfs.Realm = GetAudienceUri();
            }

            if (form != null)
            {
                form.Close();
            }
            form = new LoginForm(adfs.IdpEndpoint, adfs.Realm, options);
            form.ShowDialog();
            tokenOutput = form.Output;
            return tokenOutput;
        }

        private static bool TokenIssued()
        {
            return tokenOutput != null;
        }

        /// <summary>
        /// This method brings up the standard ADFS logon dialog for the user to login. If login is successful, true is returned
        /// </summary>
        /// <param name="adfs">ADFS options</param>
        /// <param name="options">Options for the logon dialog</param>
        /// <returns></returns>
        public static bool Login(AdfsOptions adfs, LoginOptions options = null)
        {
            if (IsAuthenticated())
                return true;

            if (options == null)
            {
                options = new LoginOptions();
            }
            if (string.IsNullOrEmpty(adfs.Realm))
            {
                adfs.Realm = GetAudienceUri();
            }

            if (form != null)
            {
                form.Close();
            }
            form = new LoginForm(adfs.IdpEndpoint, adfs.Realm, options);
            form.ShowDialog();
            return IsAuthenticated();
        }

        /// <summary>
        /// Method performing logout, so that the current user is not longer authenticated.
        /// </summary>
        public static void Logout()
        {
            if (form != null)
            {
                form.Logout();
            }
            Thread.CurrentPrincipal = null;
        }

        /// <summary>
        /// This method tries to login the provided user on the specified ADFS IDP. If successful the current principal will be attached to the Thread and it will return the security token.
        /// 
        /// If login is not successful it will throw an AdfsSecurityException specifying the reason. 
        /// 
        /// </summary>
        /// <param name="adfs">Options indicating which IDP to use and a few other options.</param>
        /// <param name="userName">The username to login</param>
        /// <param name="password">The users password</param>
        /// <returns>The obtained security token</returns>
        /// 
        /// <throws>If login is not successful it will throw AdfsSecurityException with one of the reason codes (UserNameOrPasswordIncorrect, PasswordHasExpired, AccountDisabled, AccountLockedOut, PasswordMustChange)</throws>
        public static SecurityToken Login(AdfsOptions adfs, string userName, string password)
        {
            if (string.IsNullOrEmpty(adfs.Realm))
            {
                adfs.Realm = GetAudienceUri();
            }
            Thread.CurrentPrincipal = null;
            if (!string.IsNullOrEmpty(adfs.UserValidationServiceUri))
            {
                ValidateUser(adfs.UserValidationServiceUri, userName, password);
            }

            GenericXmlSecurityToken securityToken = (GenericXmlSecurityToken)AdfsHelper.GetSecurityToken(adfs.IdpEndpoint, adfs.Realm, userName, password);

            SamlSecurityToken token = (SamlSecurityToken) FederatedAuthentication.FederationConfiguration.IdentityConfiguration.SecurityTokenHandlers.ReadToken(new XmlTextReader(new StringReader(securityToken.TokenXml.OuterXml)));
            ClaimsIdentity identity = FederatedAuthentication.FederationConfiguration.IdentityConfiguration.SecurityTokenHandlers.ValidateToken(token).First();

            // Get the IClaimsPrincipal and attach it to the current thread
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);
            Thread.CurrentPrincipal = claimsPrincipal;

            return securityToken;
        }

        private static void ValidateUser(string userValidationServiceUri, string userName, string password)
        {
            using (WcfServiceWrapperBasic<IValidateUser> service = new WcfServiceWrapperBasic<IValidateUser>(userValidationServiceUri))
            {
                try
                {
                    service.Channel.ValidateDliUser(userName, password);
                }
                catch (FaultException faultException)
                {
                    throw new AdfsSecurityException(faultException.Code);
                }
            }
        }

        public static bool IsAuthenticated()
        {
            return (Thread.CurrentPrincipal != null) && Thread.CurrentPrincipal.Identity.IsAuthenticated;
        }

        private static string GetAudienceUri()
        {
            var serviceConfiguration = FederatedAuthentication.FederationConfiguration.IdentityConfiguration;
            if (serviceConfiguration.AudienceRestriction.AllowedAudienceUris.Count <= 0)
                throw new ConfigurationErrorsException("Configure at least one Audience Uri in 'microsoft.identityModel.audienceUris'.");
            if (serviceConfiguration.AudienceRestriction.AllowedAudienceUris.Count > 1)
                throw new ConfigurationErrorsException("More than one Audience Uri is defined in 'microsoft.identityModel.audienceUris'. Use signature specifying the intended realm.");

            return serviceConfiguration.AudienceRestriction.AllowedAudienceUris[0].AbsoluteUri;
        }

    }

    public class AdfsSecurityException : SecurityException
    {
        public FaultCode Reason { get; set; }

        public AdfsSecurityException(FaultCode reason)
        {
            this.Reason = reason;
        }
    }
}
