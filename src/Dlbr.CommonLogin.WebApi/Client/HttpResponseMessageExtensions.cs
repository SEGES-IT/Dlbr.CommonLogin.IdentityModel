using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dlbr.CommonLogin.WebApi.Client
{
    public static class HttpResponseMessageExtensions
    {
        private class ApiException
        {
            public string Message { get; set; }
            public string ExceptionMessage { get; set; }
            public string ExceptionType { get; set; }
            public string StackTrace { get; set; }
        }

        private static async Task ThrowIfError(this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var apiException = await response.Content.ReadAsAsync<ApiException>();
                throw new HttpRequestException(String.Format("{0}: {1}", response.ReasonPhrase, apiException.ExceptionMessage));
            }
        }

        public static async Task<T> SafeReadAs<T>(this Task<HttpResponseMessage> task)
        {
            using (var response = await task)
            {
                await response.ThrowIfError();
                return await response.Content.ReadAsAsync<T>();
            }
        }

        public static async Task SafeFireAndForget(this Task<HttpResponseMessage> task)
        {
            using (var response = await task)
            {
                await response.ThrowIfError();
            }
        }
    }
}
