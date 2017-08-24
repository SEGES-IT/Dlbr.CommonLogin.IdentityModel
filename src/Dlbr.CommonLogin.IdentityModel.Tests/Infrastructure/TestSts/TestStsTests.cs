using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using NUnit.Framework;

namespace Dlbr.CommonLogin.IdentityModel.Tests.Infrastructure.TestSts
{
    [TestFixture]
    public class TestStsTests
    {
        [SetUp]
        public void Init()
        {
        }

        [TearDown]
        public void Dispose()
        {
        }

        [Test]
        public void TestSts_Thumbprint_NotNull()
        {
            var sts = Infrastructure.TestSts.TestSts.Create();

            var thumbprint = sts.Thumbprint;

            Assert.IsNotNull(thumbprint);
        }

        [Test]
        public void IssueTokenAsXml_PrincipalWithClaims_IssuesTokenWithClaims()
        {
            var sts = Infrastructure.TestSts.TestSts.Create();
            var principal =
                new ClaimsPrincipal(new[]
                {
                    new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, "My test user"), 
                        new Claim(ClaimTypes.Role, "My test role 1"),
                        new Claim(ClaimTypes.Role, "My test role 2"),
                    }),
                });

            var tokenXml = sts.IssueTokenAsXml(principal, "http://doesnotmatterforthistest");
            var token = XDocument.Parse(tokenXml);

            XNamespace samlXmlNs = "urn:oasis:names:tc:SAML:1.0:assertion";
            var claims = token.Descendants(samlXmlNs + "Attribute");
            var nameClaimValue = claims.Where(ele => ele.Attribute("AttributeName").Value == "name").Descendants().Single().Value;
            var roleClaimValues = claims.Where(ele => ele.Attribute("AttributeName").Value == "role").Descendants().Select(element => element.Value);
            
            Assert.AreEqual(2,claims.Count());
            Assert.AreEqual("My test user",nameClaimValue);
            CollectionAssert.AreEquivalent(new [] {"My test role 1", "My test role 2"}, roleClaimValues);
        }

        [Test]
        public void IssueTokenAsXml_AnyInput_KeyInfoCertificateHasSigningCertificateThumbprint()
        {
            var sts = Infrastructure.TestSts.TestSts.Create();
            var principal = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[] {new Claim("http://somenamespace/doesnotmatterforthistest","")})});

            var tokenXml = sts.IssueTokenAsXml(principal, "http://doesnotmatterforthistest");
            var token = XDocument.Parse(tokenXml);

            XNamespace xmlSigNs = "http://www.w3.org/2000/09/xmldsig#";
            var certElement = token.Descendants(xmlSigNs + "X509Certificate").Single();
            var encodedCert = certElement.Value;

            var keyInfoCertificate = new X509Certificate2(Convert.FromBase64String(encodedCert));

            Assert.AreEqual(sts.Thumbprint, keyInfoCertificate.Thumbprint);
        }

    }
}