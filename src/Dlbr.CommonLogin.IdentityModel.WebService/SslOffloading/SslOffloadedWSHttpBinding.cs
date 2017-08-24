using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace Dlbr.CommonLogin.IdentityModel.WebService.SslOffloading
{
    public class SslOffloadedWSHttpBinding : WSHttpBinding
    {
        public override System.ServiceModel.Channels.BindingElementCollection CreateBindingElements()
        {
            var elements = base.CreateBindingElements();
            var transportBinding = elements.Find<HttpsTransportBindingElement>();

            var transportBindingIndex = elements.IndexOf(transportBinding);
            elements.Remove(transportBinding);
            elements.Insert(transportBindingIndex, new SslOffloadedHttpTransportBindingElement(transportBinding));
            return elements;
        }
    }
}
