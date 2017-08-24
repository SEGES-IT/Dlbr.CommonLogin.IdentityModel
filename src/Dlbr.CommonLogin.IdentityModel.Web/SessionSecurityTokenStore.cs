using System;
using System.IdentityModel.Tokens;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Dlbr.CommonLogin.IdentityModel.Web
{
    public abstract class SessionSecurityTokenStore
    {
        protected SessionSecurityTokenStore()
        {
            m_Serializer = new DataContractSerializer(typeof(Tuple<DateTime, SessionSecurityToken>));
        }

        private DataContractSerializer m_Serializer;
        protected static string GenerateCompositeCacheKey(SessionSecurityTokenCacheKey cacheKey)
        {
            return String.Format("{0}{1}", cacheKey.ContextId, cacheKey.KeyGeneration);
        }

        protected static UniqueId GetContextId(string contextIdString)
        {
            return contextIdString == null ? null : new UniqueId(contextIdString);
        }

        protected static UniqueId GetKeyGeneration(string keyGenerationString)
        {
            return keyGenerationString == null ? null : new UniqueId(keyGenerationString);
        }


        protected string Serialize(DateTime expiryTime, SessionSecurityToken value)
        {
            var valueToSerialize = new Tuple<DateTime, SessionSecurityToken>(expiryTime, value);
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                using (var xtw = new XmlTextWriter(sw))
                {
                    m_Serializer.WriteObject(xtw, valueToSerialize);
                    xtw.Flush();
                }
                sw.Flush();
            }
            return sb.ToString();
        }

        protected Tuple<DateTime, SessionSecurityToken> Deserialize(string securityTokenSerialized)
        {
            using (var sr = new StringReader(securityTokenSerialized))
            {
                using (var xtr = new XmlTextReader(sr))
                {
                    return m_Serializer.ReadObject(xtr) as Tuple<DateTime, SessionSecurityToken>;
                }
            }
        }

        public abstract void RemoveTokenFromStore(SessionSecurityTokenCacheKey cacheKey);
        public abstract void UpdateTokenInStore(SessionSecurityTokenCacheKey key, SessionSecurityToken value, DateTime expiryTime);
        public abstract Tuple<DateTime, SessionSecurityToken> ReadTokenFromStore(SessionSecurityTokenCacheKey key);
    }
}