using System;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Xml;
using Dlbr.CommonLogin.IdentityModel.Web;
using FluentAssertions;
using NUnit.Framework;

namespace Dlbr.CommonLogin.IdentityModel.Tests.Unit.IdentityModel.Web
{
    [TestFixture]
    public class SqlServerSessionSecurityTokenStoreTests
    {
        public const string ConnectionStringName = "UnitTestSecurityTokenCacheContext";

        [Test]
        public void UpdateTokenInStore_EmptyStore_CanBeReadBack()
        {
            var s = new SqlServerSessionSecurityTokenStore {ConnectionStringName = ConnectionStringName};
            var token = CreateToken(DateTime.UtcNow, TimeSpan.FromHours(8));
            var key = CreateKeyFromToken(token);

            s.UpdateTokenInStore(key,token,token.KeyExpirationTime);
            var roundTrippedTokenTuple = s.ReadTokenFromStore(key);

            roundTrippedTokenTuple.Should().NotBeNull();
            roundTrippedTokenTuple.Item2.ShouldBeEquivalentTo(token);
        }

        [Test]
        public void DeleteTokenFromStore_EmptyStore_DoesNotThrow()
        {
            var s = new SqlServerSessionSecurityTokenStore { ConnectionStringName = ConnectionStringName };
            var token = CreateToken(DateTime.UtcNow, TimeSpan.FromHours(8));
            var key = CreateKeyFromToken(token);

            Assert.DoesNotThrow(() => s.RemoveTokenFromStore(key));
        }

        [Test]
        public void ReadTokenFromStore_EmptyStore_ReturnsNull()
        {
            var s = new SqlServerSessionSecurityTokenStore { ConnectionStringName = ConnectionStringName };
            var token = CreateToken(DateTime.UtcNow, TimeSpan.FromHours(8));
            var key = CreateKeyFromToken(token);

            var tokenTuple = s.ReadTokenFromStore(key);
            
            tokenTuple.Should().BeNull();
        }


        private static SessionSecurityTokenCacheKey CreateKeyFromToken(SessionSecurityToken token)
        {
            return new SessionSecurityTokenCacheKey(token.EndpointId, token.ContextId, token.KeyGeneration);
        }

        private static SessionSecurityToken CreateToken(DateTime validFrom, TimeSpan lifeTime)
        {
            return new SessionSecurityToken(
                new ClaimsPrincipal(
                    new[]
                    {
                        new ClaimsIdentity(new Claim [0])
                    }), 
                    new UniqueId(Guid.NewGuid()), "context", "endpoint", validFrom, lifeTime, null);
        }
    }
}
