using System.Collections.Specialized;
using Microsoft.Web.Infrastructure.DynamicValidationHelper;

namespace System.Web.Helpers
{
    // This class is copied "as is" from System.Web.WebPages.dll. The reasoning is
    // - reduce dependency to be on the relatively stable Microsoft.Infrastructure instead of System.Web.WebPages
    // - avoid _any_ modification of the sample from http://social.technet.microsoft.com/wiki/contents/articles/1725.windows-identity-foundation-wif-a-potentially-dangerous-request-form-value-was-detected-from-the-client-wresult-t-requestsecurityto.aspx
    internal static class Validation
    {
        public static UnvalidatedRequestValues Unvalidated(this HttpRequest request)
        {
            Func<NameValueCollection> func;
            Func<NameValueCollection> func2;
            HttpContext current = HttpContext.Current;
            ValidationUtility.GetUnvalidatedCollections(current, out func, out func2);
            return new UnvalidatedRequestValues(new HttpRequestWrapper(current.Request), func, func2);
        }
    }
}