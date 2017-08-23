using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Policy;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.IO;
using System.Text;
using System.Xml;

namespace Dlbr.CommonLogin.IdentityModel.Windows
{
    public class RstrHelper
    {
        public const string WsTrustvOldVersion = "http://schemas.xmlsoap.org/ws/2005/02/trust";
        public const string WSTrust13Version = "http://docs.oasis-open.org/ws-sx/ws-trust/200512";

        private WSTrustResponseSerializer GetRstrSerializer(string wsFederationRequestSecurityTokenResponse)
        {
            if (wsFederationRequestSecurityTokenResponse.Contains(WSTrust13Version))
            {
                return new WSTrust13ResponseSerializer();
            }
            return  new WSTrustFeb2005ResponseSerializer();
        }

        public string SerializeToRstrString(RequestSecurityTokenResponse rstr, string wsTrustVersion = WSTrust13Version)
        {
            var buffer = new StringBuilder();
            using (var sw = new StringWriter(buffer))
            {
                using (var xw = new XmlTextWriter(sw))
                {
                    var serializer = GetRstrSerializer(wsTrustVersion);
                    serializer.WriteXml(rstr, xw, new WSTrustSerializationContext());
                }
            }
            return buffer.ToString();
        }

        public RequestSecurityTokenResponse DeserializeRstrFromRstrString(string rstrString)
        {
            using (var sr = new StringReader(rstrString))
            {
                using (var xr = new XmlTextReader(sr))
                {
                    var serializer = GetRstrSerializer(rstrString);
                    return serializer.ReadXml(xr, new WSTrustSerializationContext());
                }
            }
        }

        public GenericXmlSecurityToken DeserializeTokenFromRstrString(string rstrString)
        {
            var rstr = DeserializeRstrFromRstrString(rstrString);
            return DeserializeTokenFromRstr(rstr);
        }

        public GenericXmlSecurityToken DeserializeTokenFromRstr(RequestSecurityTokenResponse rstr)
        {
            SecurityToken proofKey = null;
            DateTime? created = null;
            DateTime? expires = null;
            if (rstr.Lifetime != null)
            {
                created = rstr.Lifetime.Created;
                expires = rstr.Lifetime.Expires;
            }
            if (!created.HasValue)
            {
                throw new Exception("Created unspecified");
            }
            if (!expires.HasValue)
            {
                throw new Exception("Expires unspecified");
            }

            GenericXmlSecurityToken securityToken = new GenericXmlSecurityToken(
                rstr.RequestedSecurityToken.SecurityTokenXml,
                proofKey,
                created.Value,
                expires.Value,
                rstr.RequestedAttachedReference,
                rstr.RequestedUnattachedReference,
                new ReadOnlyCollection<IAuthorizationPolicy>(new List<IAuthorizationPolicy>())
                );
            return securityToken;
        }
    }
}