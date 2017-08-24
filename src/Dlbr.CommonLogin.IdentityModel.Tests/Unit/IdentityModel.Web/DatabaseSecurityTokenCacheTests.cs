using System;
using System.IdentityModel.Configuration;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Threading;
using System.Xml;
using Dlbr.CommonLogin.IdentityModel.Tests.Infrastructure;
using Dlbr.CommonLogin.IdentityModel.Web;
using FluentAssertions;
using NUnit.Framework;

namespace Dlbr.CommonLogin.IdentityModel.Tests.Unit.IdentityModel.Web
{
    [TestFixture]
    public class DatabaseSecurityTokenCacheTests
    {
        static readonly object[] CacheAndStoreTestCases = new object[]
        {
            // This tests that the backing store gets called correctly in case of cache misses
            new object[]{new AlwaysMissingSessionSecurityTokenCache(), new InMemorySessionSecurityTokenStore()},
            // This tests that the cache will be used on read when it does not miss
            new object[]{new IdentityModelCaches().SessionSecurityTokenCache, new AlwaysMissingSessionSecurityTokenStore()},
            // This tests an as-close-as-possible-to-the-real-thing-in-a-unit-test configuration
            new object[]{new IdentityModelCaches().SessionSecurityTokenCache, new SqlServerSessionSecurityTokenStore {ConnectionStringName = SqlServerSessionSecurityTokenStoreTests.ConnectionStringName}},
        };

        [Test]
        public void TestableDatabaseSecurityTokenCache_ParametersGetsExposedCorrectly()
        {
            // Tests / documents that the specific C# constructor precedence relied on in this test works as intended.
            var cache = new TestableDatabaseSecurityTokenCache(new AlwaysMissingSessionSecurityTokenCache(), new AlwaysMissingSessionSecurityTokenStore());

            cache.Should().NotBeNull();
            cache.SessionSecurityTokenCache.Should().BeOfType<AlwaysMissingSessionSecurityTokenCache>();
            cache.SessionSecurityTokenStore.Should().BeOfType<AlwaysMissingSessionSecurityTokenStore>();
        }

        [Test, TestCaseSource("CacheAndStoreTestCases")]
        public void AddOrUpdate_EternalSessionSecurityToken_GetReturnsToken(SessionSecurityTokenCache memoryCache, SessionSecurityTokenStore store)
        {
            var token = CreateToken(DateTime.UtcNow, TimeSpan.MaxValue);
            var key = CreateKeyFromToken(token);

            var cache = new TestableDatabaseSecurityTokenCache(memoryCache, store);
            cache.AddOrUpdate(key, token, token.KeyExpirationTime);
            var roundtrippedToken = cache.Get(key);

            roundtrippedToken.ShouldBeEquivalentTo(token);
        }

        [Test, TestCaseSource("CacheAndStoreTestCases")]
        public void AddOrUpdate_SessionSecurityTokenExpiredInCache_GetReturnsToken(SessionSecurityTokenCache memoryCache, SessionSecurityTokenStore store)
        {
            // It is, unfortunately, impossible to create a SessionSecurityToken that is expired...
            var token = CreateToken(DateTime.UtcNow, TimeSpan.FromMilliseconds(500));
            var key = CreateKeyFromToken(token);

            var cache = new TestableDatabaseSecurityTokenCache(memoryCache, store);
            cache.AddOrUpdate(key, token, token.KeyExpirationTime);

            Thread.Sleep(TimeSpan.FromSeconds(1));

            var roundtrippedToken = cache.Get(key);

            roundtrippedToken.ShouldBeEquivalentTo(token);
        }

        [Test, TestCaseSource("CacheAndStoreTestCases")]
        public void Get_SessionSecurityTokenNotInCache_ReturnsNull(SessionSecurityTokenCache memoryCache, SessionSecurityTokenStore store)
        {
            var token = CreateToken(DateTime.UtcNow, TimeSpan.MaxValue);
            var key = CreateKeyFromToken(token);

            var cache = new TestableDatabaseSecurityTokenCache(memoryCache, store);

            var roundtrippedToken = cache.Get(key);

            roundtrippedToken.Should().BeNull();
        }

        [Test, TestCaseSource("CacheAndStoreTestCases")]
        public void Get_SessionSecurityTokenDeletedFromCache_ReturnsNull(SessionSecurityTokenCache memoryCache, SessionSecurityTokenStore store)
        {
            var token = CreateToken(DateTime.UtcNow, TimeSpan.MaxValue);
            var key = CreateKeyFromToken(token);

            var cache = new TestableDatabaseSecurityTokenCache(memoryCache, store);

            cache.AddOrUpdate(key, token, token.KeyExpirationTime);
            cache.Remove(key);

            var roundtrippedToken = cache.Get(key);

            roundtrippedToken.Should().BeNull();
        }

        private static SessionSecurityTokenCacheKey CreateKeyFromToken(SessionSecurityToken token)
        {
            return new SessionSecurityTokenCacheKey(token.EndpointId,token.ContextId,token.KeyGeneration);
        }

        private static SessionSecurityToken CreateToken(DateTime validFrom, TimeSpan lifeTime)
        {
            return new SessionSecurityToken(new ClaimsPrincipal(), new UniqueId(Guid.NewGuid()), "context", "endpoint", validFrom, lifeTime, null);
        }
    }
}
