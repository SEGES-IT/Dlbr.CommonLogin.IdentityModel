using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dlbr.CommonLogin.IdentityModel.Windows.Login
{
    public class LoginOptions
    {
        public LoginOptions()
        {
            Caption = "LoginForm";
            DialogBorder = true;
            TokenOutput = TokenOutput.ValidateAndSetPrincipal;
        }

        internal TokenOutput TokenOutput { get; set; }

        public enum Location { CenterToParent, CenterToScreen };
        public string Caption { get; set; }
        public Location DialogLocation { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
        public bool DialogBorder { get; set; }
        public string BrandingId { get; set; }
    }
}
