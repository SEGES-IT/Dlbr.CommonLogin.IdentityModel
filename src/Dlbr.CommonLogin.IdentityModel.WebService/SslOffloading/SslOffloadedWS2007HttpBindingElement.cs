using System;
using System.ServiceModel.Configuration;

namespace Dlbr.CommonLogin.IdentityModel.WebService.SslOffloading
{
    public class SslOffloadedWS2007HttpBindingElement : WS2007HttpBindingElement
    {
        protected override Type BindingElementType
        {
            get { return typeof(SslOffloadedWS2007HttpBinding); }
        }
    }
}
