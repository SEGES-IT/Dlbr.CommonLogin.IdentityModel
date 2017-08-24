using System;
using System.Collections.Generic;
using System.IdentityModel.Configuration;
using System.IdentityModel.Tokens;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using Dlbr.CommonLogin.IdentityModel.Web.Logging;

namespace Dlbr.CommonLogin.IdentityModel.Web
{
    
    public class DatabaseSecurityTokenCache : SessionSecurityTokenCache, ICustomIdentityConfiguration
    {
        private static readonly ILog Log = LogProvider.For<DatabaseSecurityTokenCache>();

        // Common causes of this functionality not working
        // - incorrect hook in Global.asax (fails silently)
        // - connection string issues (will throw exceptions under some circumstances)
        // - incorrect application of cookie transforms without copying the configuration (the web component does this correctly, some older implementations may not)
        // This instance will actually be a MruSessionSecurityTokenCache
        protected SessionSecurityTokenCache m_MemoryCache;
        protected SessionSecurityTokenStore m_SessionSecurityTokenStore;
        public DatabaseSecurityTokenCache()
        {
            m_MemoryCache = new IdentityModelCaches().SessionSecurityTokenCache;
            m_SessionSecurityTokenStore = new SqlServerSessionSecurityTokenStore();
        }




        public override IEnumerable<SessionSecurityToken> GetAll(string endpointId, UniqueId contextId)
        {
            Log.Warn("Call to NotImplemented IEnumerable<SessionSecurityToken> GetAll(string endpointId, UniqueId contextId)");
            throw new NotImplementedException();
        }

        public override void Remove(SessionSecurityTokenCacheKey key)
        {
            // Completely removing from the memory cache is difficult, since either we have to forego memory caching or distribute the memory 
            // cache (remove it on all nodes). 
            // Since this cache is only intended for web scenarios, we assume that the client will purge the key, making the clearing 
            // of the memory cache less essential.
            Log.DebugFormat("Removing token key {0} from local machine cache (it will still exist on any other nodes)", key);
            m_MemoryCache.Remove(key);

            Log.DebugFormat("Removing token key {0} from persistent store", key);
            var cacheKey = key;
            m_SessionSecurityTokenStore.RemoveTokenFromStore(cacheKey);
        }

        public override void RemoveAll(string endpointId, UniqueId contextId)
        {
            Log.Warn("Call to NotImplemented RemoveAll(string endpointId, UniqueId contextId)");
        }

        public override void RemoveAll(string endpointId)
        {
            Log.Warn("Call to NotImplemented RemoveAll(string endpointId)");
        }

        //public override bool TryAddEntry(object key, SecurityToken value)
        public override void AddOrUpdate(SessionSecurityTokenCacheKey key, SessionSecurityToken value, DateTime expiryTime)
        {
            Log.DebugFormat("Key: {0} Value: {1} Expiry: {2}", key, value.Id, expiryTime);
            Log.DebugFormat("Adding or updating in memory cache");
            m_MemoryCache.AddOrUpdate(key, value, expiryTime);
            Log.DebugFormat("Adding or updating in persistent store");
            m_SessionSecurityTokenStore.UpdateTokenInStore(key, value, expiryTime);
        }


        public override SessionSecurityToken Get(SessionSecurityTokenCacheKey key)
        {
            Log.DebugFormat("Key: {0}",key);
            var token = m_MemoryCache.Get(key);
            if (token != null)
            {
                Log.DebugFormat("Token: {0} - found in memory cache", token.Id);
                return token;
            }

            var tokenWithExpiry = m_SessionSecurityTokenStore.ReadTokenFromStore(key);
            if (tokenWithExpiry == null ||tokenWithExpiry.Item2 == null)
            {
                Log.DebugFormat("Token key: {0} - not in cache", key);
                return null;
            }
            var expiry = tokenWithExpiry.Item1;
            token = tokenWithExpiry.Item2;
            Log.DebugFormat("Token: {0} Expiry: {1} - found in persistent cache", token.Id, expiry);
            Log.DebugFormat("Refreshing token {0} in memory cache", token.Id);
            m_MemoryCache.AddOrUpdate(key, token, expiry);
            return token;
        }
    }
}