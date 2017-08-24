using System.ServiceModel.Channels;

namespace Dlbr.CommonLogin.IdentityModel.WebService.SslOffloading
{
    public class SslOffloadedHttpTransportBindingElement : HttpTransportBindingElement, ITransportTokenAssertionProvider
    {
        public SslOffloadedHttpTransportBindingElement()
            : base()
        {
        }

        protected internal SslOffloadedHttpTransportBindingElement(HttpTransportBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (typeof(T) == typeof(ISecurityCapabilities))
            {
                return (T)(object)new SslOffloadedSecurityCapabilities();
            }
            return base.GetProperty<T>(context);
        }

        public override BindingElement Clone()
        {
            return new SslOffloadedHttpTransportBindingElement(this);
        }

        public System.Xml.XmlElement GetTransportTokenAssertion()
        {
            return null;
        }
    }
}