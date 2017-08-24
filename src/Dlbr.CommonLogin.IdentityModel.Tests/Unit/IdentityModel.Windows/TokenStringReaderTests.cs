using System;
using System.Linq;
using Dlbr.CommonLogin.IdentityModel.Windows.Login;
using NUnit.Framework;

namespace Dlbr.CommonLogin.IdentityModel.Tests.Unit.IdentityModel.Windows
{
    [TestFixture]
    [Category("BuildVerification")]
    public class TokenStringReaderTests
    {
        [Test]
        public void CanReadExpiredTokenFromDevTestIdp()
        {
            var expiredTokenFromDevTestIdp = @"<saml:Assertion MajorVersion=""1"" MinorVersion=""1"" AssertionID=""_e1736368-808e-4553-94a8-4b7166f55c7f"" Issuer=""http://devtest-idp.vfltest.dk/adfs/services/trust"" IssueInstant=""2016-02-15T12:46:05.383Z"" xmlns:saml=""urn:oasis:names:tc:SAML:1.0:assertion""><saml:Conditions NotBefore=""2016-02-15T12:46:05.368Z"" NotOnOrAfter=""2016-02-15T13:46:05.368Z""><saml:AudienceRestrictionCondition><saml:Audience>https://localhost.vfltest.dk/OBAPI/</saml:Audience></saml:AudienceRestrictionCondition></saml:Conditions><saml:AttributeStatement><saml:Subject><saml:NameIdentifier>cvruser3@PROD.DLI</saml:NameIdentifier><saml:SubjectConfirmation><saml:ConfirmationMethod>urn:oasis:names:tc:SAML:1.0:cm:bearer</saml:ConfirmationMethod></saml:SubjectConfirmation></saml:Subject><saml:Attribute AttributeName=""role"" AttributeNamespace=""http://schemas.microsoft.com/ws/2008/06/identity/claims""><saml:AttributeValue>GBTC009Landmand</saml:AttributeValue><saml:AttributeValue>GOAlleSelvOprettere</saml:AttributeValue><saml:AttributeValue>GBTOLandmand</saml:AttributeValue><saml:AttributeValue>Domain Users</saml:AttributeValue><saml:AttributeValue>GTALCCSSystembrugere</saml:AttributeValue><saml:AttributeValue>GTALCDLIBasis</saml:AttributeValue><saml:AttributeValue>GTALCPlanteITBasis</saml:AttributeValue><saml:AttributeValue>GTAFocusFinderAdgang</saml:AttributeValue><saml:AttributeValue>GBTDLLandmand</saml:AttributeValue><saml:AttributeValue>GTALCLandmandDkMedlem</saml:AttributeValue><saml:AttributeValue>GTALCDCFFodertjek</saml:AttributeValue><saml:AttributeValue>GBTSOLandmand</saml:AttributeValue></saml:Attribute></saml:AttributeStatement><saml:AuthenticationStatement AuthenticationMethod=""urn:oasis:names:tc:SAML:1.0:am:password"" AuthenticationInstant=""2016-02-15T12:46:05.352Z""><saml:Subject><saml:NameIdentifier>cvruser3@PROD.DLI</saml:NameIdentifier><saml:SubjectConfirmation><saml:ConfirmationMethod>urn:oasis:names:tc:SAML:1.0:cm:bearer</saml:ConfirmationMethod></saml:SubjectConfirmation></saml:Subject></saml:AuthenticationStatement><ds:Signature xmlns:ds=""http://www.w3.org/2000/09/xmldsig#""><ds:SignedInfo><ds:CanonicalizationMethod Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#"" /><ds:SignatureMethod Algorithm=""http://www.w3.org/2001/04/xmldsig-more#rsa-sha256"" /><ds:Reference URI=""#_e1736368-808e-4553-94a8-4b7166f55c7f""><ds:Transforms><ds:Transform Algorithm=""http://www.w3.org/2000/09/xmldsig#enveloped-signature"" /><ds:Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#"" /></ds:Transforms><ds:DigestMethod Algorithm=""http://www.w3.org/2001/04/xmlenc#sha256"" /><ds:DigestValue>Pc6RqG0JPFoNf6ARSinZuSiFUWLUvPhSfNopLNwQ1Zk=</ds:DigestValue></ds:Reference></ds:SignedInfo><ds:SignatureValue>xCxcUt3MNSDwzIgIt6jZGKA/57e67hJjTphvTUA7dJ/NBXfpX7pwqjFAKtnhXHpcfpXGulhtTlqqK2pZwoQXIhLMLgX0C/jJbJ0zKVmn5l4/ZfwNaXJ5ViwJoJH5lufoGcUYYRu2Kw7JP5n+sA2Vi+G+CulTQCsL+qrD7c2xLvsOUETKoPSyDICSHFsI0y4+1e/vgarXtQ2huSM4iGr9KSMYcOrZGwQ7WvY1r9YNZbt4MwsTZ4niQXZswPWjLbDTBtlzpHpPjw7zZNDWox6qInmlS/LfBQJI9uQcNDsYefs+mo7jgAG7+2H4snJva7n84obaxLZjUuhS3OUVxGTEkQ==</ds:SignatureValue><KeyInfo xmlns=""http://www.w3.org/2000/09/xmldsig#""><X509Data><X509Certificate>MIIDkzCCAn+gAwIBAgIQeabFHpwukJJNo/hx3eMN+DAJBgUrDgMCHQUAMEExPzA9BgNVBAMTNkFERlMgU2lnbmluZyAtIGRldnRlc3QtaWRwLWxhbmRtYW5kZGsudmZsdGVzdC5kayBkbGJyMjAgFw0xMjA4MTcwNjUwMTdaGA8yOTk4MTIzMTIyMDAwMFowQTE/MD0GA1UEAxM2QURGUyBTaWduaW5nIC0gZGV2dGVzdC1pZHAtbGFuZG1hbmRkay52Zmx0ZXN0LmRrIGRsYnIyMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAxXf3AdAgxpJcpHgKc2I155whL8qUrIKu3QDEWCkzSLj5owzrxWE7fM/hTINIUWXhimUM2Imt9QIxlqEN+PbZBiwAUrbjE+bueSJRe34kLOSlLK5VxInMCUuvx4xfdF/3B0NDaSv77IM73SZEv1R0jaswz7OIuVzkW8udsxdiFBUYfgjkuI8QefgyUUizZVLJthrXkLIb9vG6/xHixzvoKA+Uwr14JzoDZL5GFqY5Bv2MlIsuYwIqWEkzZotZQ++W2zesUEWIf+nTjrfYOWJx9aKaBgeAdF9kOoZO9+qUL8w15T3QoaHl0fkbP75m7xE5Zui/YVcoHrTproER+bbh4QIDAQABo4GMMIGJMBMGA1UdJQQMMAoGCCsGAQUFBwMBMHIGA1UdAQRrMGmAECb73e9fE3NGkfPzFpdMrN6hQzBBMT8wPQYDVQQDEzZBREZTIFNpZ25pbmcgLSBkZXZ0ZXN0LWlkcC1sYW5kbWFuZGRrLnZmbHRlc3QuZGsgZGxicjKCEHmmxR6cLpCSTaP4cd3jDfgwCQYFKw4DAh0FAAOCAQEAelggKg6ShmJo1JHf+HAkwoWW40RYVAS3k12MU9MowO/OaotHhZ+bHduqMEhgEqqEIklVMo0mV58ofINRaNBGf/oLvQoJTohEvbhq6m/Da9d8kpQBdxG2SwsczsGqSml5FR7FlpYe3gxzdnpXa8cGj9R+OnauNdbU61w336WbiKDTUorh9n+wlOCUckV1ddcClVvwmcyq/zevrbN2euhvQN1+9RopP5hKl5wYPSLUP8OXZuA1X6lFG/QNjAT2wbjkR5UPDGXnollz977VXc1wSLaE1oDBkXF0Rfwafci9OSm4pYwqw61PE33IXq0t9SdGWOzgVFwCy1GOj8mpL7Kxcw==</X509Certificate></X509Data></KeyInfo></ds:Signature></saml:Assertion>";

            var r = new TokenStringReader();
            var principal = r.ReadUnvalidated(expiredTokenFromDevTestIdp);
            foreach (var claim in principal.Identities.First().Claims)
            {
                Console.WriteLine("{0} -> {1} -> {2}", claim.Issuer, claim.Type, claim.Value);
            }
            Assert.IsTrue(principal.Identity.IsAuthenticated);
        }

    }
}
