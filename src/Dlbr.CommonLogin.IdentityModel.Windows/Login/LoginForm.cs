using System;
using System.Configuration;
using System.Drawing;
using System.IdentityModel.Selectors;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.IO;
using System.Security.Claims;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using System.Xml;

namespace Dlbr.CommonLogin.IdentityModel.Windows.Login
{
	public partial class LoginForm : Form
	{
		private const string FederationSigninWaValue = "wsignin1.0";
        private const string FederationSignoutWaValue = "wsignout1.0";
		private readonly string idpEndpoint;
		private readonly string realm;
	    private readonly LoginOptions options;
	    private readonly string username;
		private readonly string password;
        private readonly string brandingId;

        public string Output { get; private set; }

	    internal LoginForm(string idpEndpoint, string realm, LoginOptions options)
		{
			this.idpEndpoint = idpEndpoint;
			this.realm = realm;
		    this.options = options;
		    this.username = options.Username;
			this.password = options.Password;
            this.brandingId = options.BrandingId;

			InitializeComponent();
			InitForm(options);

			string wsFederationRequest = string.Format("{0}?wa={1}&wtrealm={2}&bid={3}", this.idpEndpoint, FederationSigninWaValue, HttpUtility.UrlEncode(this.realm), this.brandingId);
			webBrowserControl.Navigate(wsFederationRequest);
		}

        public void Logout()
        {
            string wsSignoutRequest = string.Format("{0}?wa={1}", this.idpEndpoint, FederationSignoutWaValue);
            webBrowserControl.Navigate(wsSignoutRequest);
        }

	    private void InitForm(LoginOptions options)
		{
			switch (options.DialogLocation)
			{
				case LoginOptions.Location.CenterToParent:
					StartPosition = FormStartPosition.CenterParent;
					break;
				case LoginOptions.Location.CenterToScreen:
					StartPosition = FormStartPosition.CenterScreen;
					break;
			}
			Text = options.Caption;
			if (options.DialogBorder)
			{
				cancelButton.Visible = false;
				FormBorderStyle = FormBorderStyle.Fixed3D;
			}
		}

		private void webBrowserControl_Navigating(object sender, WebBrowserNavigatingEventArgs e)
		{
			var webControl = sender as WebBrowser;
			if (e.Url != null && !e.Url.AbsoluteUri.StartsWith(this.idpEndpoint, StringComparison.InvariantCultureIgnoreCase) && webControl != null && webControl.Document != null)
			{
				var form = webControl.Document.Forms["hiddenform"];
				if (form != null)
				{
					var wa = GetFormAttributeValue(form, "wa", "value");
					var wresult = GetFormAttributeValue(form, "wresult", "value");

					// Validate wa element
					if (!FederationSigninWaValue.Equals(wa, StringComparison.InvariantCulture))
						throw new ArgumentException("WS-Federation respons is not a sign-in respons");

					var tokenXml = ReadSamlTokenRequestSecurityTokenResponse(wresult);
				    switch (options.TokenOutput)
				    {
				        case TokenOutput.ReturnTokenString:
				            this.Output = tokenXml;
				            break;
                        case TokenOutput.ReturnRstr:
				            this.Output = wresult;
				            break;
				        default:
				            ReadAndValidateSamlToken(tokenXml);
				            break;
				    }
                    e.Cancel = true;
                    Hide();
				}
			}
		}

		private static string GetFormAttributeValue(HtmlElement form, string elementId, string attributeName)
		{
			var item = form.Children[elementId];
			if (item == null)
				throw new ArgumentNullException(string.Format("WS-Federation {0} respons element", elementId));

			var value = item.GetAttribute(attributeName);
			if (string.IsNullOrWhiteSpace(value))
				throw new ArgumentNullException(string.Format("WS-Federation {0} respons element attribute {1}", elementId, attributeName));

			return value;
		}

		private static string ReadSamlTokenRequestSecurityTokenResponse(string xml)
		{
			using (StringReader sr = new StringReader(xml))
			{
				using (XmlReader xr = XmlReader.Create(sr))
				{
					var dom = new XmlDocument { PreserveWhitespace = true };
					dom.Load(xr);
					var element = dom.GetElementsByTagName("t:RequestedSecurityToken");
					return element[0].InnerXml;
				}
			}
		}

		private static void ReadAndValidateSamlToken(string tokenXml)
		{
			using (var stringReader = new StringReader(tokenXml))
			using (XmlReader reader = XmlReader.Create(stringReader))
			{
			    var serviceConfiguration = FederatedAuthentication.FederationConfiguration.IdentityConfiguration;
			    var handler = serviceConfiguration.SecurityTokenHandlers;

				SecurityToken samlToken = handler.ReadToken(reader);
				Thread.CurrentPrincipal = new ClaimsPrincipal(handler.ValidateToken(samlToken));
			}

		}

		private static X509CertificateValidator ReadCertificateValidator(X509CertificateValidationMode mode)
		{
			switch (mode)
			{
				case X509CertificateValidationMode.None:
					return X509CertificateValidator.None;
				case X509CertificateValidationMode.ChainTrust:
					return X509CertificateValidator.ChainTrust;
				case X509CertificateValidationMode.PeerTrust:
					return X509CertificateValidator.PeerTrust;
				case X509CertificateValidationMode.PeerOrChainTrust:
					return X509CertificateValidator.PeerOrChainTrust;
			}
			throw new ConfigurationErrorsException(string.Format("Certificate validation mode {0} not supported", mode));
		}

		private void webBrowserControl_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			var webControl = sender as WebBrowser;

			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			this.BackColor = Color.Transparent;

			if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
				return; // No automatic login

			var inputElements = webControl.Document.GetElementsByTagName("input");

			HtmlElement loginButton = null;
			foreach (var element in inputElements)
			{
				var htmlElement = element as HtmlElement;
				if (htmlElement.Name.Contains("UsernameTextBox"))
					htmlElement.SetAttribute("value", this.username);
				else if (htmlElement.Name.Contains("PasswordTextBox"))
					htmlElement.SetAttribute("value", this.password);
				else if (htmlElement.Name.Contains("SubmitButton"))
					loginButton = htmlElement;
			}

			if (loginButton == null)
				return; // Second roundtrip is ignored

			loginButton.InvokeMember("click");

			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			this.BackColor = Color.Transparent;
		}

	}
}
