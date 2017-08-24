using System.Net.Security;
using System.ServiceModel.Channels;

namespace Dlbr.CommonLogin.IdentityModel.WebService.SslOffloading
{
    public class SslOffloadedSecurityCapabilities : ISecurityCapabilities
    {
        public ProtectionLevel SupportedRequestProtectionLevel
        {
            get { return ProtectionLevel.EncryptAndSign; }
        }

        public ProtectionLevel SupportedResponseProtectionLevel
        {
            get { return ProtectionLevel.EncryptAndSign; }
        }

        public bool SupportsClientAuthentication
        {
            get { return false; }
        }

        public bool SupportsClientWindowsIdentity
        {
            get { return false; }
        }

        public bool SupportsServerAuthentication
        {
            get { return true; }
        }
    }
}