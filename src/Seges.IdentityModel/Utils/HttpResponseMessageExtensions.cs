using System.Net.Http;

namespace Seges.IdentityModel.Utils
{
    public static class HttpResponseMessageExtensions
    {
        public static HttpResponseMessage EnsureSuccessStatusCode(this HttpResponseMessage response, string responseContent)
        {
            if (response.IsSuccessStatusCode)
            {
                return response;
            }

            responseContent = responseContent ?? string.Empty;
            const int truncateAt = 2000;
            var truncatedResponse =
                responseContent.Substring(0, responseContent.Length > truncateAt ? truncateAt : responseContent.Length);

            throw new HttpRequestException(
                $"Response status code does not indicate success: {response.StatusCode} {response.ReasonPhrase}. First {truncateAt} response chars:  {truncatedResponse}");
        }
    }
}
