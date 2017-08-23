namespace Dlbr.CommonLogin.IdentityModel.WebApi
{
    public interface ITokenHeaderEncoder
    {
        string Encode(string token);
        string Decode(string encodedToken);
    }
}