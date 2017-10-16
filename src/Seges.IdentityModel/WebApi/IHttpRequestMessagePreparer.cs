using System.Net.Http;
using System.Threading.Tasks;

namespace Seges.IdentityModel.WebApi
{
    public interface IHttpRequestMessagePreparer
    {
        Task PrepareRequest(HttpRequestMessage message);
    }
}