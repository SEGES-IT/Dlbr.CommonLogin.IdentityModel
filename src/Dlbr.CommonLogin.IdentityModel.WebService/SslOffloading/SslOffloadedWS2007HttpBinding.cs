using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Dlbr.CommonLogin.IdentityModel.WebService.SslOffloading
{
    public class SslOffloadedWS2007HttpBinding : WS2007HttpBinding
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