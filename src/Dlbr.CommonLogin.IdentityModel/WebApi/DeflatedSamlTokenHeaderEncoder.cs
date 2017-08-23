using System;
using System.IdentityModel;
using System.Reflection;
using System.Text;
using System.Web;
using Dlbr.CommonLogin.IdentityModel.Logging;

namespace Dlbr.CommonLogin.IdentityModel.WebApi
{
    /// <summary>
    /// A "DeflatedSaml" token header encoder. DeflatedSaml is a semi-standard, in that the encoding/compression steps are described in the SAML standards.
    /// Fiddlers TextWizard has built-in support for decoding DeflatedSaml.
    /// </summary>
    public class DeflatedSamlTokenHeaderEncoder : ITokenHeaderEncoder
    {
        private static readonly ILog Log = LogProvider.For<DeflatedSamlTokenHeaderEncoder>();
        private readonly Encoding _encoding = Encoding.UTF8;
        private readonly DeflateCookieTransform _deflateCookieTransform;

        public DeflatedSamlTokenHeaderEncoder()
        {
            _deflateCookieTransform = new DeflateCookieTransform();
        }

        public string Encode(string token)
        {
            // Before it’s sent, the message is deflated, base64-encoded, and URL-encoded, in that order.
            var bytes = _encoding.GetBytes(token);
            var deflatedBytes = _deflateCookieTransform.Encode(bytes);
            var base64String =  Convert.ToBase64String(deflatedBytes);
            var urlEncodedString = HttpUtility.UrlEncode(base64String);
            Log.Debug(() => string.Format("Encoded token: {0}", urlEncodedString));
            return urlEncodedString;
        }

        public string Decode(string encodedToken)
        {
            var base64String = HttpUtility.UrlDecode(encodedToken);
            var deflatedBytes = Convert.FromBase64String(base64String);
            var bytes = _deflateCookieTransform.Decode(deflatedBytes);
            var token = _encoding.GetString(bytes);
            return token;
        }
    }
}
