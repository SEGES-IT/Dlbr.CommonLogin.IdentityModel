using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Text;

namespace Dlbr.CommonLogin.IdentityModel.WebService.SslOffloading
{
    public class SslOffloadedBasicHttpBindingElement : BasicHttpBindingElement
    {
        protected override Type BindingElementType
        {
            get { return typeof(SslOffloadedBasicHttpBinding); }
        }
    }
}
