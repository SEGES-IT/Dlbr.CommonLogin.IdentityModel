﻿/*
 * Copyright (c) Dominick Baier.  All rights reserved.
 * see license.txt
 */

using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Dlbr.CommonLogin.IdentityModel.WSTrustBindings
{
    public class WindowsWSTrustBinding : WSTrustBinding
    {
        // Methods
        public WindowsWSTrustBinding()
            : this(SecurityMode.Message)
        { }

        public WindowsWSTrustBinding(SecurityMode securityMode)
            : base(securityMode)
        { }

        protected override void ApplyTransportSecurity(HttpTransportBindingElement transport)
        {
            transport.AuthenticationScheme = AuthenticationSchemes.Negotiate;
        }

        protected override SecurityBindingElement CreateSecurityBindingElement()
        {
            if (SecurityMode.Message == base.SecurityMode)
            {
                return SecurityBindingElement.CreateSspiNegotiationBindingElement(true);
            }
          
            if (SecurityMode.TransportWithMessageCredential == base.SecurityMode)
            {
                return SecurityBindingElement.CreateSspiNegotiationOverTransportBindingElement(true);
            }
            
            return null;
        }
    }
}