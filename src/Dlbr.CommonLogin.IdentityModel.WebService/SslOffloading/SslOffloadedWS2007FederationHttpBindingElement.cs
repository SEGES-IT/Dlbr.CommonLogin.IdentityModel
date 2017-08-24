using System;
using System.ServiceModel.Configuration;

namespace Dlbr.CommonLogin.IdentityModel.WebService.SslOffloading
{
    public class SslOffloadedWS2007FederationHttpBindingElement : WS2007FederationHttpBindingElement
    {
        protected override Type BindingElementType
        {
            get { return typeof(SslOffloadedWS2007FederationHttpBinding); }
        }
    }
}
