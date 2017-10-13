using Seges.IdentityModel.Log.Logging;
using System;
using System.Text;

namespace Seges.IdentityModel.DeflatedSaml
{
    public class DeflatedSamlEncoder 
    {

        private static readonly ILog Log = LogProvider.For<DeflatedSamlEncoder>();
        private readonly Encoding _encoding = Encoding.UTF8;
        private readonly DeflateCookieTransform _deflateCookieTransform = new DeflateCookieTransform();

        public virtual string Encode(string token)
        {
            // Before it’s sent, the message is deflated, base64-encoded, and URL-encoded, in that order.
            Log.Debug(() => string.Format("Raw token: {0}", token));
            var bytes = _encoding.GetBytes(token);
            var deflatedBytes = _deflateCookieTransform.Encode(bytes);
            var base64String =  Convert.ToBase64String(deflatedBytes);
            var encodedToken = System.Net.WebUtility.UrlEncode(base64String);
            Log.Debug(() => string.Format("Encoded token: {0}", encodedToken));
            return encodedToken;
        }

        public virtual string Decode(string encodedToken)
        {
            Log.Debug(() => string.Format("Encoded token: {0}", encodedToken));
            var base64String = System.Net.WebUtility.UrlDecode(encodedToken);
            var deflatedBytes = Convert.FromBase64String(base64String);
            var bytes = _deflateCookieTransform.Decode(deflatedBytes);
            var token = _encoding.GetString(bytes);
            Log.Debug(() => string.Format("Raw token: {0}", token));
            return token;
        }
    }
}
