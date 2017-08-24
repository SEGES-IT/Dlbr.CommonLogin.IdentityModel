using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Xml;

namespace Dlbr.CommonLogin.IdentityModel.Tests.Infrastructure
{
    internal class AlwaysMissingSessionSecurityTokenCache : SessionSecurityTokenCache
    {
        public override void AddOrUpdate(SessionSecurityTokenCacheKey key, SessionSecurityToken value, DateTime expiryTime)
        {
        }

        public override IEnumerable<SessionSecurityToken> GetAll(string endpointId, UniqueId contextId)
        {
            return new SessionSecurityToken[0];
        }

        public override SessionSecurityToken Get(SessionSecurityTokenCacheKey key)
        {
            return null;
        }

        public override void RemoveAll(string endpointId, UniqueId contextId)
        {
        }

        public override void RemoveAll(string endpointId)
        {
        }

        public override void Remove(SessionSecurityTokenCacheKey key)
        {
        }
    }
}