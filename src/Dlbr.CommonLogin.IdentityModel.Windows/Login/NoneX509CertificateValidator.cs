using System;
using System.IdentityModel.Selectors;
using System.Security.Cryptography.X509Certificates;

namespace Dlbr.CommonLogin.IdentityModel.Windows.Login
{
    internal class NoneX509CertificateValidator : X509CertificateValidator
    {
        public override void Validate(X509Certificate2 certificate)
        {
            if (certificate == null)
            {
                throw new ArgumentNullException("certificate");
            }
        }
    }
}