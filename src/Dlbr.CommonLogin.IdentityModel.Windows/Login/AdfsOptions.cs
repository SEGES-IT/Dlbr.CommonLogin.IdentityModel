using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dlbr.CommonLogin.IdentityModel.Windows.Login
{
    public class AdfsOptions
    {
        /// <summary>
        /// This property indicates which ADFS-IDP endpoint to use, e.g. "https://dev-idp.vfltest.dk/adfs/services/trust/13/usernamemixed". 
        /// This property must be set.
        /// </summary>
        public string IdpEndpoint { get; set; }

        /// <summary>
        /// This property indicates which realm your application is registered under in the ADFS IDP.
        /// You may opt not to specify this property. If so, the realm is read from your config-file in the <audienceUris></audienceUris> section. In this case you 
        /// must have exactly one audienceUri specified.
        /// </summary>
        public string Realm { get; set; }

        /// <summary>
        /// Property indicating where to call the user validation service checking the identity of the user specified. 
        /// This option is only valid in the custom logon scenario, i.e. if you do not use the ADFS logon site as your clients logon-dialog.
        /// If not set, the component will not try to call the user validation service.
        /// </summary>
        public string UserValidationServiceUri { get; set; }
    }
}
