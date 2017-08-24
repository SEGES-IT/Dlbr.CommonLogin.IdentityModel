using System.IdentityModel.Tokens;
using System.Threading.Tasks;

namespace Dlbr.CommonLogin.WebApi.Client
{
    public interface ITokenProvider
    {
        Task<SecurityToken> GetToken(ISecureWebApiConfiguration configuration);
    }
}
