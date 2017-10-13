//This test was useful during implementation, but introduces an unwanted depedency
//using System.Web;
//using Dlbr.CommonLogin.IdentityModel.WebApi;
//using NUnit.Framework;
//using Seges.IdentityModel;
//using Seges.IdentityModel.DeflatedSaml;

//namespace Dlbr.CommonLogin.IdentityModel.Tests.Unit.Seges.IdentityModel
//{
//    [TestFixture]
//    [Category("BuildVerification")]
//    public class DeflatedSamlEncoderTests
//    {

//        [Test]
//        public void Encode_OutputMatchOldImplementation()
//        {
//            DeflatedSamlEncoder encoder = new DeflatedSamlEncoder();
//            var input = Token;
//            var actual = encoder.Encode(input);
//            var expected = new DeflatedSamlTokenHeaderEncoder().Encode(input);
//            AssertAreEqualIgnoringUrlEncodeCase(expected, actual);
//        }

//        private void AssertAreEqualIgnoringUrlEncodeCase(string expected, string actual)
//        {
//            var actualCanonical = HttpUtility.UrlEncode(HttpUtility.UrlDecode(actual));
//            var expectedCanonical = HttpUtility.UrlEncode(HttpUtility.UrlDecode(expected));
//            Assert.AreEqual(expectedCanonical, actualCanonical);
//        }

//        private const string Token = "<saml:Assertion MajorVersion=\"1\" MinorVersion=\"1\" AssertionID=\"_b6f31526-cafa-4dc" +
//            "2-abac-cca3f56e8a3f\" Issuer=\"http://si-idp.vfltest.dk/adfs/services/trust\" Issue" +
//            "Instant=\"2017-10-12T13:04:43.961Z\" xmlns:saml=\"urn:oasis:names:tc:SAML:1.0:asser" +
//            "tion\"><saml:Conditions NotBefore=\"2017-10-12T13:04:43.961Z\" NotOnOrAfter=\"2017-1" +
//            "0-12T14:04:43.961Z\"><saml:AudienceRestrictionCondition><saml:Audience>https://lo" +
//            "calhost.vfltest.dk/CustomerSampleTier1Service/</saml:Audience></saml:AudienceRes" +
//            "trictionCondition></saml:Conditions><saml:AttributeStatement><saml:Subject><saml" +
//            ":SubjectConfirmation><saml:ConfirmationMethod>urn:oasis:names:tc:SAML:1.0:cm:bea" +
//            "rer</saml:ConfirmationMethod></saml:SubjectConfirmation></saml:Subject><saml:Att" +
//            "ribute AttributeName=\"name\" AttributeNamespace=\"http://schemas.xmlsoap.org/ws/20" +
//            "05/05/identity/claims\"><saml:AttributeValue>PROD\\cvr01</saml:AttributeValue></sa" +
//            "ml:Attribute><saml:Attribute AttributeName=\"emailaddress\" AttributeNamespace=\"ht" +
//            "tp://schemas.xmlsoap.org/ws/2005/05/identity/claims\"><saml:AttributeValue>mac@vf" +
//            "l.dk</saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"upn\" A" +
//            "ttributeNamespace=\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims\"><saml:" +
//            "AttributeValue>cvr01@PROD.DLI</saml:AttributeValue></saml:Attribute><saml:Attrib" +
//            "ute AttributeName=\"givenname\" AttributeNamespace=\"http://schemas.xmlsoap.org/ws/" +
//            "2005/05/identity/claims\"><saml:AttributeValue> </saml:AttributeValue></saml:Attr" +
//            "ibute><saml:Attribute AttributeName=\"surname\" AttributeNamespace=\"http://schemas" +
//            ".xmlsoap.org/ws/2005/05/identity/claims\"><saml:AttributeValue>cvr01 (89999995)</" +
//            "saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName=\"actor\" Attri" +
//            "buteNamespace=\"http://schemas.xmlsoap.org/ws/2009/09/identity/claims\"><saml:Attr" +
//            "ibuteValue>&lt;Actor&gt;&lt;saml:Attribute AttributeName=\"name\" AttributeNamespa" +
//            "ce=\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims\" xmlns:saml=\"urn:oasis" +
//            ":names:tc:SAML:1.0:assertion\"&gt;&lt;saml:AttributeValue&gt;PROD\\customersamplea" +
//            "ctas&lt;/saml:AttributeValue&gt;&lt;/saml:Attribute&gt;&lt;saml:Attribute Attrib" +
//            "uteName=\"upn\" AttributeNamespace=\"http://schemas.xmlsoap.org/ws/2005/05/identity" +
//            "/claims\" xmlns:saml=\"urn:oasis:names:tc:SAML:1.0:assertion\"&gt;&lt;saml:Attribut" +
//            "eValue&gt;customersampleactas@PROD.DLI&lt;/saml:AttributeValue&gt;&lt;/saml:Attr" +
//            "ibute&gt;&lt;saml:Attribute AttributeName=\"emailaddress\" AttributeNamespace=\"htt" +
//            "p://schemas.xmlsoap.org/ws/2005/05/identity/claims\" xmlns:saml=\"urn:oasis:names:" +
//            "tc:SAML:1.0:assertion\"&gt;&lt;saml:AttributeValue&gt;test@example.com&lt;/saml:A" +
//            "ttributeValue&gt;&lt;/saml:Attribute&gt;&lt;saml:Attribute AttributeName=\"givenn" +
//            "ame\" AttributeNamespace=\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims\" " +
//            "xmlns:saml=\"urn:oasis:names:tc:SAML:1.0:assertion\"&gt;&lt;saml:AttributeValue&gt" +
//            "; &lt;/saml:AttributeValue&gt;&lt;/saml:Attribute&gt;&lt;saml:Attribute Attribut" +
//            "eName=\"surname\" AttributeNamespace=\"http://schemas.xmlsoap.org/ws/2005/05/identi" +
//            "ty/claims\" xmlns:saml=\"urn:oasis:names:tc:SAML:1.0:assertion\"&gt;&lt;saml:Attrib" +
//            "uteValue&gt;customersampleactas&lt;/saml:AttributeValue&gt;&lt;/saml:Attribute&g" +
//            "t;&lt;saml:Attribute AttributeName=\"authenticationmethod\" AttributeNamespace=\"ht" +
//            "tp://schemas.microsoft.com/ws/2008/06/identity/claims\" xmlns:saml=\"urn:oasis:nam" +
//            "es:tc:SAML:1.0:assertion\"&gt;&lt;saml:AttributeValue&gt;http://schemas.microsoft" +
//            ".com/ws/2008/06/identity/authenticationmethod/password&lt;/saml:AttributeValue&g" +
//            "t;&lt;/saml:Attribute&gt;&lt;saml:Attribute AttributeName=\"authenticationinstant" +
//            "\" AttributeNamespace=\"http://schemas.microsoft.com/ws/2008/06/identity/claims\" x" +
//            "mlns:saml=\"urn:oasis:names:tc:SAML:1.0:assertion\"&gt;&lt;saml:AttributeValue a:t" +
//            "ype=\"tn:dateTime\" xmlns:tn=\"http://www.w3.org/2001/XMLSchema\" xmlns:a=\"http://ww" +
//            "w.w3.org/2001/XMLSchema-instance\"&gt;2017-10-12T13:04:43.946Z&lt;/saml:Attribute" +
//            "Value&gt;&lt;/saml:Attribute&gt;&lt;/Actor&gt;</saml:AttributeValue></saml:Attri" +
//            "bute></saml:AttributeStatement><saml:AuthenticationStatement AuthenticationMetho" +
//            "d=\"urn:oasis:names:tc:SAML:1.0:am:password\" AuthenticationInstant=\"2017-10-12T13" +
//            ":04:43.524Z\"><saml:Subject><saml:SubjectConfirmation><saml:ConfirmationMethod>ur" +
//            "n:oasis:names:tc:SAML:1.0:cm:bearer</saml:ConfirmationMethod></saml:SubjectConfi" +
//            "rmation></saml:Subject></saml:AuthenticationStatement><ds:Signature xmlns:ds=\"ht" +
//            "tp://www.w3.org/2000/09/xmldsig#\"><ds:SignedInfo><ds:CanonicalizationMethod Algo" +
//            "rithm=\"http://www.w3.org/2001/10/xml-exc-c14n#\" /><ds:SignatureMethod Algorithm=" +
//            "\"http://www.w3.org/2001/04/xmldsig-more#rsa-sha256\" /><ds:Reference URI=\"#_b6f31" +
//            "526-cafa-4dc2-abac-cca3f56e8a3f\"><ds:Transforms><ds:Transform Algorithm=\"http://" +
//            "www.w3.org/2000/09/xmldsig#enveloped-signature\" /><ds:Transform Algorithm=\"http:" +
//            "//www.w3.org/2001/10/xml-exc-c14n#\" /></ds:Transforms><ds:DigestMethod Algorithm" +
//            "=\"http://www.w3.org/2001/04/xmlenc#sha256\" /><ds:DigestValue>h5vM3c1hsbwUJ7daOK2" +
//            "5ODvIsIu59kW7J7p0avMRBI8=</ds:DigestValue></ds:Reference></ds:SignedInfo><ds:Sig" +
//            "natureValue>VPavX82SqyETqzuFUA0UGToJPQc9RIPoiNY4dKoWUUzTXeKiXScGmIcCYWJVL5Bby/aJ" +
//            "oT20jKb81VJl+OHxlu0ctb0Ytr4eoZzoGczWCwZYl5w7h5vRwxRc/pR6umCSgdy3d1NVt0s5CsycjQQY" +
//            "lgHRsQMWk7G2FL2P1rc1zuArATNade9okAP7CmO7i7aw7UPsTglEiCWY8nLFfUiy5e1HyTab/Vw05ETH" +
//            "A+/ZqT7N5POYHkynogf9todX11NDB/4Xh8Y4WETb6sBioKbOoZlQYe//DHxu3AxDZseDg/nLURO7P3sV" +
//            "9qO/uKVhElXWNIgAhjlL+LVV/SMcOnEALw==</ds:SignatureValue><KeyInfo xmlns=\"http://w" +
//            "ww.w3.org/2000/09/xmldsig#\"><X509Data><X509Certificate>MIIDQjCCAiqgAwIBAgIQGuO/V" +
//            "sFEcJ1BS46QHMNn9DANBgkqhkiG9w0BAQsFADArMSkwJwYDVQQDEyBBREZTIFNpZ25pbmcgLSBzaS1pZ" +
//            "HAudmZsdGVzdC5kazAgFw0xNTEyMzEyMjAwMDBaGA8zOTk5MTIzMTIyMDAwMFowKzEpMCcGA1UEAxMgQ" +
//            "URGUyBTaWduaW5nIC0gc2ktaWRwLnZmbHRlc3QuZGswggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKA" +
//            "oIBAQC4MsHOghdlzNfQXZrLFq+oL1oqcEi0vPplGOWqCj9boRaKyzJ4oNnFcvZQvKIYv+w46mADxYd0R" +
//            "XVK/dzinSZ3Qb7jnYdGAC1OxfSeTzWd+usNzIm/HvYgw6UjfEhwao0mWQYlELHSb88kzMTPrPRYyu+kr" +
//            "vpAmVysGzeVRU6nt2tQk5xx8aTh3x+Kfqcr405OSul7uzgogz+R+hOhhImvt/i6msituw+RpmgyReEy7" +
//            "qZE12yZKLCbKeVf821uud+fkJzGsqsJ54sgpgbanQWB5eUDWLDV0wKoW8bCMpuFWWanCpw3nHQeZzZr9" +
//            "vb2Y5iq0RcMzsK/CVzBeIeCIQBBAgMBAAGjYDBeMFwGA1UdAQRVMFOAEJaBJFyizaDteaBULprNsymhL" +
//            "TArMSkwJwYDVQQDEyBBREZTIFNpZ25pbmcgLSBzaS1pZHAudmZsdGVzdC5ka4IQGuO/VsFEcJ1BS46QH" +
//            "MNn9DANBgkqhkiG9w0BAQsFAAOCAQEATe4nVccZXgJlohDTNQeDfDrvOoiagsLA739oVVWqQG4qIY9yg" +
//            "Rzy8XTPbtTfxNIRU4uXnwzcdhqEUykNBo0V6HcCR7WuQ/4xMuhKvHW4fcjds7Ezrvz9y+ij0hmiAinW8" +
//            "K7UXjH3FmlwevOlrIbKKLS3DdSC1PkYBMp4qnPz8zT0zsQEo3ZpnBnxzKnQXhXV/rTvrUUwjFoRJeILk" +
//            "xvqg6+A/IJMD8seXs5Y1kq2Igr1QRn/sxi0EapVltLLcVRHfP2TIqn+2jFQLQ4vbU7KLL3BWJJjXFGFL" +
//            "e90f4wmJHs82Ru1Znxxn/YptFDeYOxsJyeRa2msijBkJn911ue2sA==</X509Certificate></X509D" +
//            "ata></KeyInfo></ds:Signature></saml:Assertion>";

//    }
//}
