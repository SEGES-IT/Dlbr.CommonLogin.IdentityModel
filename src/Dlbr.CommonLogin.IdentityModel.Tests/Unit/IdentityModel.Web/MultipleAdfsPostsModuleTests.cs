//using System.Collections.Specialized;
//using System.IdentityModel.Services;
//using System.IdentityModel.Tokens;
//using System.Web;
//using Dlbr.CommonLogin.IdentityModel.Web;
//using FluentAssertions;
//using NUnit.Framework;
//using TypeMock.ArrangeActAssert;

//namespace Dlbr.CommonLogin.IdentityModel.Tests.Unit.IdentityModel.Web
//{
//    [TestFixture]
//    public class MultipleAdfsPostsModuleTests
//    {
//        [Test]
//        [Category("TypeMock")]
//        public void SessionSecurityTokenReceived_WhenCalled_WSSessionSecurityTokenReceivedSetToTrue()
//        {
//            // Arrange
//            MultipleAdfsPostsModule module = new MultipleAdfsPostsModule();
//            HttpApplication httpApplication = Isolate.Fake.Instance<HttpApplication>();
//            SessionSecurityToken token = Isolate.Fake.Instance<SessionSecurityToken>();
//            SessionSecurityTokenReceivedEventArgs args = new SessionSecurityTokenReceivedEventArgs(token);
//            HttpContext.Current = Isolate.Fake.Instance<HttpContext>();

//            // Act
//            Isolate.Invoke.Method(module, "SessionSecurityTokenReceived", httpApplication, args);

//            // Assert
//            HttpContext.Current.Items["WSSessionSecurityTokenReceived"].Should().Be(true);
//        }

//        [Test]
//        [Isolated]
//        [Category("TypeMock")]
//        public void EndRequest_SessionAuthenticationModuleNull_Ignored()
//        {
//            // Arrange
//            MultipleAdfsPostsModule module = new MultipleAdfsPostsModule();
//            SessionSecurityToken token = Isolate.Fake.Instance<SessionSecurityToken>();
//            SessionSecurityTokenReceivedEventArgs args = new SessionSecurityTokenReceivedEventArgs(token);
//            Isolate.NonPublic.Property.WhenGetCalled(typeof(FederatedAuthentication), "SessionAuthenticationModule").WillReturn(null);

//            // Act
//            Isolate.Invoke.Method(module, "EndRequest", "", args);

//            // Assert
//            Isolate.Verify.WasNotCalled(() => FederatedAuthentication.WSFederationAuthenticationModule.IsSignInResponse(null));
//        }

//        [Test]
//        [Isolated]
//        [Category("TypeMock")]
//        public void EndRequest_WSFederationAuthenticationModuleNull_Ignored()
//        {
//            // Arrange
//            MultipleAdfsPostsModule module = new MultipleAdfsPostsModule();
//            SessionSecurityToken token = Isolate.Fake.Instance<SessionSecurityToken>();
//            SessionSecurityTokenReceivedEventArgs args = new SessionSecurityTokenReceivedEventArgs(token);
//            Isolate.NonPublic.Property.WhenGetCalled(typeof(FederatedAuthentication), "WSFederationAuthenticationModule").WillReturn(null);
//            Isolate.NonPublic.Property.WhenGetCalled(typeof(FederatedAuthentication), "SessionAuthenticationModule").WillReturn(Isolate.Fake.Instance<SessionAuthenticationModule>());

//            // Act
//            Isolate.Invoke.Method(module, "EndRequest", "", args);

//            // Assert
//            Isolate.Verify.WasNotCalled(() => FederatedAuthentication.WSFederationAuthenticationModule.IsSignInResponse(null));
//        }

//        [Test]
//        [Isolated]
//        [Category("TypeMock")]
//        public void EndRequest_IsNotSignInResponse_Ignored()
//        {
//            // Arrange
//            MultipleAdfsPostsModule module = new MultipleAdfsPostsModule();
//            SessionSecurityToken token = Isolate.Fake.Instance<SessionSecurityToken>();
//            SessionSecurityTokenReceivedEventArgs args = new SessionSecurityTokenReceivedEventArgs(token);
//            WSFederationAuthenticationModule wsFederationAuthenticationModule = Isolate.Fake.Instance<WSFederationAuthenticationModule>();
//            Isolate.NonPublic.Property.WhenGetCalled(typeof(FederatedAuthentication), "WSFederationAuthenticationModule").WillReturn(wsFederationAuthenticationModule);
//            Isolate.NonPublic.Property.WhenGetCalled(typeof(FederatedAuthentication), "SessionAuthenticationModule").WillReturn(Isolate.Fake.Instance<SessionAuthenticationModule>());
//            Isolate.WhenCalled(() => FederatedAuthentication.WSFederationAuthenticationModule.IsSignInResponse(null)).WillReturn(false);
//            HttpContext.Current = Isolate.Fake.Instance<HttpContext>();
//            Isolate.Fake.StaticMethods(typeof(HttpContext));

//            // Act
//            Isolate.Invoke.Method(module, "EndRequest", "", args);

//            // Assert
//            Isolate.Verify.WasNotCalled(() => HttpContext.Current.Items["WSSessionSecurityTokenReceived"]);
//        }

//        [Test]
//        [Isolated]
//        [Category("TypeMock")]
//        public void EndRequest_SessionSecurityTokenReceivedNotCalled_Ignored()
//        {
//            // Arrange
//            MultipleAdfsPostsModule module = new MultipleAdfsPostsModule();
//            SessionSecurityToken token = Isolate.Fake.Instance<SessionSecurityToken>();
//            SessionSecurityTokenReceivedEventArgs args = new SessionSecurityTokenReceivedEventArgs(token);
//            WSFederationAuthenticationModule wsFederationAuthenticationModule = Isolate.Fake.Instance<WSFederationAuthenticationModule>();
//            Isolate.NonPublic.Property.WhenGetCalled(typeof(FederatedAuthentication), "WSFederationAuthenticationModule").WillReturn(wsFederationAuthenticationModule);
//            Isolate.NonPublic.Property.WhenGetCalled(typeof(FederatedAuthentication), "SessionAuthenticationModule").WillReturn(Isolate.Fake.Instance<SessionAuthenticationModule>());
//            Isolate.WhenCalled(() => FederatedAuthentication.WSFederationAuthenticationModule.IsSignInResponse(null)).WillReturn(true);
//            HttpContext.Current = Isolate.Fake.Instance<HttpContext>();
//            Isolate.Fake.StaticMethods(typeof(HttpContext));

//            // Act
//            Isolate.Invoke.Method(module, "EndRequest", "", args);

//            // Assert
//            Isolate.Verify.WasNotCalled(() => HttpContext.Current.Request.Form);
//        }

//        [Test]
//        [Isolated]
//        [Category("TypeMock")]
//        public void EndRequest_ContextRequestFormNull_Ignored()
//        {
//            // Arrange
//            MultipleAdfsPostsModule module = new MultipleAdfsPostsModule();
//            SessionSecurityToken token = Isolate.Fake.Instance<SessionSecurityToken>();
//            SessionSecurityTokenReceivedEventArgs args = new SessionSecurityTokenReceivedEventArgs(token);
//            WSFederationAuthenticationModule wsFederationAuthenticationModule = Isolate.Fake.Instance<WSFederationAuthenticationModule>();
//            HttpApplication httpApplication = Isolate.Fake.Instance<HttpApplication>();
//            Isolate.NonPublic.Property.WhenGetCalled(typeof(FederatedAuthentication), "WSFederationAuthenticationModule").WillReturn(wsFederationAuthenticationModule);
//            Isolate.NonPublic.Property.WhenGetCalled(typeof(FederatedAuthentication), "SessionAuthenticationModule").WillReturn(Isolate.Fake.Instance<SessionAuthenticationModule>());
//            Isolate.WhenCalled(() => FederatedAuthentication.WSFederationAuthenticationModule.IsSignInResponse(null)).WillReturn(true);
//            HttpContext.Current = Isolate.Fake.Instance<HttpContext>();
//            Isolate.Fake.StaticMethods(typeof(HttpContext));
//            Isolate.WhenCalled(() => HttpContext.Current.Request.Form).WillReturn(null);

//            // Act
//            Isolate.Invoke.Method(module, "SessionSecurityTokenReceived", httpApplication, args);
//            Isolate.Invoke.Method(module, "EndRequest", "", args);

//            // Assert
//            Isolate.Verify.NonPublic.WasNotCalled(module, "GetFirstKeyValuePairWithKeyRU");
//        }

//        [Test]
//        [Isolated]
//        [Category("TypeMock")]
//        public void EndRequest_FormWctxIsNull_Ignored()
//        {
//            // Arrange
//            MultipleAdfsPostsModule module = new MultipleAdfsPostsModule();
//            SessionSecurityToken token = Isolate.Fake.Instance<SessionSecurityToken>();
//            SessionSecurityTokenReceivedEventArgs args = new SessionSecurityTokenReceivedEventArgs(token);
//            WSFederationAuthenticationModule wsFederationAuthenticationModule = Isolate.Fake.Instance<WSFederationAuthenticationModule>();
//            HttpApplication httpApplication = Isolate.Fake.Instance<HttpApplication>();
//            Isolate.NonPublic.Property.WhenGetCalled(typeof(FederatedAuthentication), "WSFederationAuthenticationModule").WillReturn(wsFederationAuthenticationModule);
//            Isolate.NonPublic.Property.WhenGetCalled(typeof(FederatedAuthentication), "SessionAuthenticationModule").WillReturn(Isolate.Fake.Instance<SessionAuthenticationModule>());
//            Isolate.WhenCalled(() => FederatedAuthentication.WSFederationAuthenticationModule.IsSignInResponse(null)).WillReturn(true);
//            HttpContext.Current = Isolate.Fake.Instance<HttpContext>();
//            Isolate.Fake.StaticMethods(typeof(HttpContext));
//            Isolate.WhenCalled(() => HttpContext.Current.Request.Form).WillReturn(new NameValueCollection());

//            // Act
//            Isolate.Invoke.Method(module, "SessionSecurityTokenReceived", httpApplication, args);
//            Isolate.Invoke.Method(module, "EndRequest", "", args);

//            // Assert
//            Isolate.Verify.NonPublic.WasNotCalled(module, "GetFirstKeyValuePairWithKeyRU");
//        }

//        [Test]
//        [Isolated]
//        [Category("TypeMock")]
//        public void EndRequest_NoKeyValuePairWithKeyRU_Ignored()
//        {
//            // Arrange
//            MultipleAdfsPostsModule module = new MultipleAdfsPostsModule();
//            SessionSecurityToken token = Isolate.Fake.Instance<SessionSecurityToken>();
//            SessionSecurityTokenReceivedEventArgs args = new SessionSecurityTokenReceivedEventArgs(token);
//            WSFederationAuthenticationModule wsFederationAuthenticationModule = Isolate.Fake.Instance<WSFederationAuthenticationModule>();
//            HttpApplication httpApplication = Isolate.Fake.Instance<HttpApplication>();
//            Isolate.NonPublic.Property.WhenGetCalled(typeof(FederatedAuthentication), "WSFederationAuthenticationModule").WillReturn(wsFederationAuthenticationModule);
//            Isolate.NonPublic.Property.WhenGetCalled(typeof(FederatedAuthentication), "SessionAuthenticationModule").WillReturn(Isolate.Fake.Instance<SessionAuthenticationModule>());
//            Isolate.WhenCalled(() => FederatedAuthentication.WSFederationAuthenticationModule.IsSignInResponse(null)).WillReturn(true);
//            HttpContext.Current = Isolate.Fake.Instance<HttpContext>();
//            Isolate.Fake.StaticMethods(typeof(HttpContext));
//            NameValueCollection nameValueCollection = new NameValueCollection();
//            nameValueCollection.Add("wctx", "test");
//            Isolate.WhenCalled(() => HttpContext.Current.Request.Form).WillReturn(nameValueCollection);

//            // Act
//            Isolate.Invoke.Method(module, "SessionSecurityTokenReceived", httpApplication, args);
//            Isolate.Invoke.Method(module, "EndRequest", "", args);

//            // Assert
//            Isolate.Verify.NonPublic.WasNotCalled(module, "ExtractUrl");
//        }

//        [Test]
//        [Isolated]
//        [Category("TypeMock")]
//        public void EndRequest_NoReturnUrlFound_Ignored()
//        {
//            // Arrange
//            MultipleAdfsPostsModule module = new MultipleAdfsPostsModule();
//            SessionSecurityToken token = Isolate.Fake.Instance<SessionSecurityToken>();
//            SessionSecurityTokenReceivedEventArgs args = new SessionSecurityTokenReceivedEventArgs(token);
//            WSFederationAuthenticationModule wsFederationAuthenticationModule = Isolate.Fake.Instance<WSFederationAuthenticationModule>();
//            HttpApplication httpApplication = Isolate.Fake.Instance<HttpApplication>();
//            Isolate.NonPublic.Property.WhenGetCalled(typeof(FederatedAuthentication), "WSFederationAuthenticationModule").WillReturn(wsFederationAuthenticationModule);
//            Isolate.NonPublic.Property.WhenGetCalled(typeof(FederatedAuthentication), "SessionAuthenticationModule").WillReturn(Isolate.Fake.Instance<SessionAuthenticationModule>());
//            Isolate.WhenCalled(() => FederatedAuthentication.WSFederationAuthenticationModule.IsSignInResponse(null)).WillReturn(true);
//            HttpContext.Current = Isolate.Fake.Instance<HttpContext>();
//            Isolate.Fake.StaticMethods(typeof(HttpContext));
//            NameValueCollection nameValueCollection = new NameValueCollection();
//            nameValueCollection.Add("wctx", "ru=");
//            Isolate.WhenCalled(() => HttpContext.Current.Request.Form).WillReturn(nameValueCollection);

//            // Act
//            Isolate.Invoke.Method(module, "SessionSecurityTokenReceived", httpApplication, args);
//            Isolate.Invoke.Method(module, "EndRequest", "", args);

//            // Assert
//            Isolate.Verify.WasNotCalled(() => HttpContext.Current.Response.Redirect(string.Empty, false));
//        }

//        [Test]
//        [Isolated]
//        [Category("TypeMock")]
//        public void EndRequest_RedirectToReturnUrl_ReturnUrlAsExpected()
//        {
//            // Arrange
//            MultipleAdfsPostsModule module = new MultipleAdfsPostsModule();
//            SessionSecurityToken token = Isolate.Fake.Instance<SessionSecurityToken>();
//            SessionSecurityTokenReceivedEventArgs args = new SessionSecurityTokenReceivedEventArgs(token);
//            WSFederationAuthenticationModule wsFederationAuthenticationModule = Isolate.Fake.Instance<WSFederationAuthenticationModule>();
//            HttpApplication httpApplication = Isolate.Fake.Instance<HttpApplication>();
//            Isolate.NonPublic.Property.WhenGetCalled(typeof(FederatedAuthentication), "WSFederationAuthenticationModule").WillReturn(wsFederationAuthenticationModule);
//            Isolate.NonPublic.Property.WhenGetCalled(typeof(FederatedAuthentication), "SessionAuthenticationModule").WillReturn(Isolate.Fake.Instance<SessionAuthenticationModule>());
//            Isolate.WhenCalled(() => FederatedAuthentication.WSFederationAuthenticationModule.IsSignInResponse(null)).WillReturn(true);
//            HttpContext.Current = Isolate.Fake.Instance<HttpContext>();
//            Isolate.Fake.StaticMethods(typeof(HttpContext));
//            NameValueCollection nameValueCollection = new NameValueCollection();
//            nameValueCollection.Add("wctx", "ru=http://localhost");
//            Isolate.WhenCalled(() => HttpContext.Current.Request.Form).WillReturn(nameValueCollection);
//            string Url = string.Empty;
//            Isolate.WhenCalled(() => HttpContext.Current.Response.Redirect(string.Empty, false)).DoInstead(context => { Url = (string)context.Parameters[0]; });
//            Isolate.WhenCalled(() => HttpContext.Current.Server.UrlDecode(string.Empty)).DoInstead(context => { return (string)context.Parameters[0]; });

//            // Act
//            Isolate.Invoke.Method(module, "SessionSecurityTokenReceived", httpApplication, args);
//            Isolate.Invoke.Method(module, "EndRequest", "", args);

//            // Assert
//            Url.Should().Be("http://localhost");
//        }
//    }
//}
