using System.Diagnostics;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;

namespace Dlbr.CommonLogin.IdentityModel.Tests.Unit.IdentityModel
{
    [TestFixture]
    [Category("BuildVerification")]
    public class WSTrustClientTests
    {
        internal class TestableWsTrustClient : WsTrustClient
        {
            public WSTrustChannelFactory UsernameWSTrustChannelFactory;
            public WSTrustChannelFactory FactoryCreatingChannel;
            public RequestSecurityToken Rst;

            public TestableWsTrustClient(string adfsDnsName) : base(adfsDnsName)
            {
            }

            public TestableWsTrustClient(string usernameMixedEndpoint, string certificateMixedEndpoint) : base(usernameMixedEndpoint)
            {
            }

            protected override WSTrustChannelFactory CreateUsernameWSTrustChannelFactory(string serviceUsername, string servicePassword)
            {
                UsernameWSTrustChannelFactory =  base.CreateUsernameWSTrustChannelFactory(serviceUsername, servicePassword);
                return UsernameWSTrustChannelFactory;
            }

            protected override TokenResponse IssueTokenForRst(WSTrustChannelFactory factory, RequestSecurityToken rst)
            {
                FactoryCreatingChannel = factory;
                Rst = rst;
                return new TokenResponse(null,null);
            }
        }

        [Test]
        public void GetActAsTokenWithUserName_WithBootstrapInContext_FactoryAndRstConfiguredCorrectly()
        {
            // Arrange
            var bootstrapToken = new UserNameSecurityToken("doesnot", "matter");
            Thread.CurrentPrincipal = new ClaimsPrincipal(new[] { new ClaimsIdentity { BootstrapContext = bootstrapToken } });
            var client = new TestableWsTrustClient("fictive.adfs");
            
            // Act
            client.GetActAsToken("https://fictiverpidentifier/", "doesnot", "matter");
            
            // Assert
            AssertUsernameFactory(
                client: client,
                expectedFactoryEndpoint: "https://fictive.adfs/adfs/services/trust/13/usernamemixed",
                expectedUsername: "doesnot",
                expectedPassword: "matter");

            AssertActAsRst(
                client: client,
                expectedRstAppliesTo: "https://fictiverpidentifier/",
                bootstrap:bootstrapToken);
        }

        [Test]
        public void GetActAsTokenWithUserName_WithExplicitBootstrap_FactoryAndRstConfiguredCorrectly()
        {
            // Arrange
            var bootstrapToken = new UserNameSecurityToken("doesnot", "matter");
            var client = new TestableWsTrustClient("fictive.adfs");

            // Act
            client.GetActAsToken("https://fictiverpidentifier/", "doesnot", "matter", bootstrapToken);

            // Assert
            AssertUsernameFactory(
                client: client,
                expectedFactoryEndpoint: "https://fictive.adfs/adfs/services/trust/13/usernamemixed",
                expectedUsername: "doesnot",
                expectedPassword: "matter");

            AssertActAsRst(
                client: client,
                expectedRstAppliesTo: "https://fictiverpidentifier/",
                bootstrap: bootstrapToken);
        }

    
        [Test]
        public void GetSecurityTokenWithUserName_FactoryAndRstConfiguredCorrectly()
        {
            // Arrange
            var client = new TestableWsTrustClient("fictive.adfs");

            // Act
            client.GetSecurityToken("https://fictiverpidentifier/", "doesnot", "matter");

            // Assert
            AssertUsernameFactory(
                client: client,
                expectedFactoryEndpoint: "https://fictive.adfs/adfs/services/trust/13/usernamemixed",
                expectedUsername: "doesnot",
                expectedPassword: "matter");
            AssertRst(
                client: client,
                expectedRstAppliesTo: "https://fictiverpidentifier/");
        }


        private static void AssertUsernameFactory(TestableWsTrustClient client, string expectedFactoryEndpoint, string expectedUsername, string expectedPassword)
        {
            client.UsernameWSTrustChannelFactory.Should().NotBeNull();
            client.FactoryCreatingChannel.ShouldBeEquivalentTo(client.UsernameWSTrustChannelFactory);
            client.FactoryCreatingChannel.Endpoint.Address.Uri.AbsoluteUri.Should()
                .BeEquivalentTo(expectedFactoryEndpoint);

            ClientCredentials credentials = client.FactoryCreatingChannel.Credentials;
            credentials.Should().NotBeNull();
            Debug.Assert(credentials != null, "credentials != null");
            credentials.ClientCertificate.Certificate.Should().BeNull();
            credentials.UserName.Should().NotBeNull();
            credentials.UserName.UserName.Should().Be(expectedUsername);
            credentials.UserName.Password.Should().Be(expectedPassword);
        }

        private static void AssertRst(TestableWsTrustClient client, string expectedRstAppliesTo)
        {
            RequestSecurityToken rst = client.Rst;
            rst.Should().NotBeNull();
            rst.ActAs.Should().BeNull();
            rst.AppliesTo.Uri.AbsoluteUri.Should().BeEquivalentTo(expectedRstAppliesTo);
        }

        private static void AssertActAsRst(TestableWsTrustClient client, string expectedRstAppliesTo, SecurityToken bootstrap)
        {
            RequestSecurityToken rst = client.Rst;
            rst.Should().NotBeNull();
            rst.ActAs.Should().NotBeNull();
            rst.AppliesTo.Uri.AbsoluteUri.Should().BeEquivalentTo(expectedRstAppliesTo);
            rst.ActAs.GetSecurityToken().Should().NotBeNull();
            rst.ActAs.GetSecurityToken().ShouldBeEquivalentTo(bootstrap);
        }
    }
}
