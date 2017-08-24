using System;
using System.IdentityModel.Tokens;
using Dlbr.CommonLogin.IdentityModel.Web;

namespace Dlbr.CommonLogin.IdentityModel.Tests.Infrastructure
{
    internal class AlwaysMissingSessionSecurityTokenStore : SessionSecurityTokenStore
    {
        public override void RemoveTokenFromStore(SessionSecurityTokenCacheKey cacheKey)
        {
        }

        public override void UpdateTokenInStore(SessionSecurityTokenCacheKey key, SessionSecurityToken value, DateTime expiryTime)
        {
        }

        public override Tuple<DateTime, SessionSecurityToken> ReadTokenFromStore(SessionSecurityTokenCacheKey key)
        {
            return null;
        }
    }
}