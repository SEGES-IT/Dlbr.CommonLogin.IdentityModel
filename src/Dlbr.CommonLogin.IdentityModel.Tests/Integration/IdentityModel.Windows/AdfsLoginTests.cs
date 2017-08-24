using System;
using System.Threading;
using Dlbr.CommonLogin.IdentityModel.Windows.Login;
using FluentAssertions;
using NUnit.Framework;

namespace Dlbr.CommonLogin.IdentityModel.Tests.Integration.IdentityModel.Windows
{
    [TestFixture]
    public class AdfsLoginTests
    {
        public static string AdTestUser = "CommonLoginCI";
        public static string AdTestPassword = "CommonLoginIntegrationTest";

        public static string AdTestUserPwdMustBeChanged = "CommonLoginPMC";
        public static string AdTestPasswordPwdMustBeChanged = "CommonLoginPMC";

        public static string AdTestUserAccountLocked = "CommonLoginLocked";
        public static string AdTestPasswordAccountLocked = "CommonLoginLocked";

        public static string AdTestUserAccountDisabled = "CommonLoginDisabled";
        public static string AdTestPasswordAccountDisabled = "CommonLoginDisabled";

        [STAThread]
		[Test]
        [Category("LocalOnly")] // LocalOnly, as we cannot show modal dialog when running at TFS server.
        [Ignore("Disturbs local test runs")] // As it disturbs local test runs
		public void Login_CredentialsOkay_ShouldBeAuthenticated()
		{
			// Arrange
            var options = new LoginOptions { Caption = "Foretag login", DialogLocation = LoginOptions.Location.CenterToParent, Username = AdTestUser, Password = AdTestPassword};

			// Act
			bool ok = AdfsLogin.Login(new AdfsOptions { IdpEndpoint = "https://dev-idp.vfltest.dk/adfs/ls/", Realm = "https://localhost.vfltest.dk/WindowsFormsApplWithWebLogin/" }, 
                options);

			// Assert
		    ok.Should().BeTrue();
			AdfsLogin.IsAuthenticated().Should().BeTrue();
            Assert.AreEqual(Thread.CurrentPrincipal.Identity.Name, "PROD\\CommonLoginCI");
		}
    }
}
