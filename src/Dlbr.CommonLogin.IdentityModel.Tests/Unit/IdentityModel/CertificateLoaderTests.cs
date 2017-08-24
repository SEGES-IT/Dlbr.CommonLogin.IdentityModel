using System.Security.Cryptography.X509Certificates;
using NUnit.Framework;

namespace Dlbr.CommonLogin.IdentityModel.Tests.Unit.IdentityModel
{
    [TestFixture]
    [Category("BuildVerification")]
    public class CertificateLoaderTests
    {
        [Test]
        public void GetCertificateByThumbprint_VeriSignCA_FindsCertificate()
        {
            const string veriSignClass3PublicPrimaryCertificationAuthorityG5Thumbprint = "4e b6 d5 78 49 9b 1c cf 5f 58 1e ad 56 be 3d 9b 67 44 a5 e5";
            var cert = CertificateLoader.GetCertificateByThumbprint(StoreName.Root,
                                                                    StoreLocation.LocalMachine,
                                                                    veriSignClass3PublicPrimaryCertificationAuthorityG5Thumbprint);
            Assert.AreEqual(@"CN=VeriSign Class 3 Public Primary Certification Authority - G5, OU=""(c) 2006 VeriSign, Inc. - For authorized use only"", OU=VeriSign Trust Network, O=""VeriSign, Inc."", C=US", cert.Subject);
        }
    }
}
