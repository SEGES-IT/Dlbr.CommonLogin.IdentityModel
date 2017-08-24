using System.Collections.Specialized;

namespace System.Web.Helpers
{
    // This class is copied "as is" from System.Web.WebPages.dll. The reasoning is
    // - reduce dependency to be on the relatively stable Microsoft.Infrastructure instead of System.Web.WebPages
    // - avoid _any_ modification of the sample from http://social.technet.microsoft.com/wiki/contents/articles/1725.windows-identity-foundation-wif-a-potentially-dangerous-request-form-value-was-detected-from-the-client-wresult-t-requestsecurityto.aspx
    internal sealed class UnvalidatedRequestValues
    {
        // Fields
        private readonly Func<NameValueCollection> _formGetter;
        private readonly Func<NameValueCollection> _queryStringGetter;
        private readonly HttpRequestBase _request;

        // Methods
        internal UnvalidatedRequestValues(HttpRequestBase request, Func<NameValueCollection> formGetter, Func<NameValueCollection> queryStringGetter)
        {
            this._request = request;
            this._formGetter = formGetter;
            this._queryStringGetter = queryStringGetter;
        }

        // Properties
        public NameValueCollection Form
        {
            get
            {
                return this._formGetter();
            }
        }

        public string this[string key]
        {
            get
            {
                string str = this.QueryString[key];
                if (str != null)
                {
                    return str;
                }
                string str2 = this.Form[key];
                if (str2 != null)
                {
                    return str2;
                }
                HttpCookie cookie = this._request.Cookies[key];
                if (cookie != null)
                {
                    return cookie.Value;
                }
                string str3 = this._request.ServerVariables[key];
                if (str3 != null)
                {
                    return str3;
                }
                return null;
            }
        }

        public NameValueCollection QueryString
        {
            get
            {
                return this._queryStringGetter();
            }
        }
    }
}