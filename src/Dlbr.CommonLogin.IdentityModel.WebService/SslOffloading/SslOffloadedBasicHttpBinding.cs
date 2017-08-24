using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Dlbr.CommonLogin.IdentityModel.WebService.SslOffloading
{
    public class SslOffloadedBasicHttpBinding : BasicHttpBinding
    {
        public override BindingElementCollection CreateBindingElements()
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
