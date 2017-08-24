using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using Dlbr.CommonLogin.IdentityModel.Web;

namespace Dlbr.CommonLogin.IdentityModel.Tests.Infrastructure
{
    internal class InMemorySessionSecurityTokenStore : SessionSecurityTokenStore
    {
        private Dictionary<SessionSecurityTokenCacheKey, Tuple<DateTime, SessionSecurityToken>> m_Lookup = new Dictionary<SessionSecurityTokenCacheKey, Tuple<DateTime, SessionSecurityToken>>();
        public override void RemoveTokenFromStore(SessionSecurityTokenCacheKey cacheKey)
        {
            m_Lookup.Remove(cacheKey);
        }

        public override void UpdateTokenInStore(SessionSecurityTokenCacheKey key, SessionSecurityToken value, DateTime expiryTime)
        {
            if (m_Lookup.ContainsKey(key))
            {
                m_Lookup.Remove(key);
            }
            m_Lookup.Add(key,new Tuple<DateTime,SessionSecurityToken>(expiryTime,value));
        }

        public override Tuple<DateTime, SessionSecurityToken> ReadTokenFromStore(SessionSecurityTokenCacheKey key)
        {
            return m_Lookup.ContainsKey(key) ? m_Lookup[key] : null;
        }
    }
}