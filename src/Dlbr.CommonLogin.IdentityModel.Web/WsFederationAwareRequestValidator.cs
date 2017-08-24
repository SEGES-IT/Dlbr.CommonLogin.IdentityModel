using System;
using System.Collections.Specialized;
using System.IdentityModel.Services;
using System.Web;
using System.Web.Util;
using Dlbr.CommonLogin.IdentityModel.Web.Internals;

namespace Dlbr.CommonLogin.IdentityModel.Web
{
    /// <summary>
    /// This class explicitly excludes WIF-handled SignIn responses from request validator
    /// </summary>
    public class WsFederationAwareRequestValidator : RequestValidator
    {
        // See http://social.technet.microsoft.com/wiki/contents/articles/1725.windows-identity-foundation-wif-a-potentially-dangerous-request-form-value-was-detected-from-the-client-wresult-t-requestsecurityto/history.aspx
        protected override bool IsValidRequestString(HttpContext context, string value, RequestValidationSource requestValidationSource, string collectionKey, out int validationFailureIndex)
        {
            validationFailureIndex = 0;
            if (requestValidationSource == RequestValidationSource.Form && !String.IsNullOrEmpty(collectionKey) && collectionKey.Equals(WSFederationConstants.Parameters.Result, StringComparison.Ordinal))
            {
                var unvalidatedFormValues = GetUnvalidatedFormValues(context);
                SignInResponseMessage message = WSFederationMessage.CreateFromNameValueCollection(WSFederationMessage.GetBaseUrl(context.Request.Url), unvalidatedFormValues) as SignInResponseMessage;
                if (message != null)
                {
                    return true;
                }
            }
            return base.IsValidRequestString(context, value, requestValidationSource, collectionKey, out validationFailureIndex);
        }

        protected virtual NameValueCollection GetUnvalidatedFormValues(HttpContext context)
        {
            var unvalidatedFormValues = System.Web.Helpers.Validation.Unvalidated(context.Request).Form;
            return unvalidatedFormValues;
        }
    }
}