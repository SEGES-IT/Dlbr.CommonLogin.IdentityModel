using System.IdentityModel.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dlbr.CommonLogin.IdentityModel.Web;

namespace Dlbr.CommonLogin.IdentityModel.Tests.Infrastructure
{
    class TestableDatabaseSecurityTokenCache : DatabaseSecurityTokenCache
    {
        public TestableDatabaseSecurityTokenCache(SessionSecurityTokenCache sessionSecurityTokenCache, SessionSecurityTokenStore sessionSecurityTokenStore)
        {
            m_MemoryCache = sessionSecurityTokenCache;
            m_SessionSecurityTokenStore = sessionSecurityTokenStore;
        }

        public SessionSecurityTokenCache SessionSecurityTokenCache
        {
            get
            {
                return m_MemoryCache;
            }
            set
            {
                m_MemoryCache = value;

            }
        }

        public SessionSecurityTokenStore SessionSecurityTokenStore
        {
            get
            {
                return m_SessionSecurityTokenStore;
            }
            set
            {
                m_SessionSecurityTokenStore = value;
            }
        }
    }
}
