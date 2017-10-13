using NUnit.Framework;
using Seges.IdentityModel.WsTrust;

namespace Seges.IdentityModel.Tests
{
    [TestFixture]
    [Category("SegesIdentityModel")]
    public class WsTrustClientTests
    {

        [Test]
        public void CanRequestToken()
        {
            var client = new global::Seges.IdentityModel.WsTrust.WsTrustClient("si-idp.vfltest.dk");
            var request = new SamlTokenRequest
            {
                Audience = "https://devtest-www-landmanddk.vfltest.dk",
                Username = "cvruser1",
                Password = "Pass1word"
            };
            var token = client.RequestTokenAsync(request).Result;
            Assert.IsNotNull(token);
            Assert.IsTrue(token.TokenXml.StartsWith("<saml:Assertion"));
        }

    }
}
