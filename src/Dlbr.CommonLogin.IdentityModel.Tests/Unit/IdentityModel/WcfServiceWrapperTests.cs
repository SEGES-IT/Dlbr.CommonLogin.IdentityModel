//using System;
//using System.IdentityModel.Tokens;
//using System.Security.Cryptography.X509Certificates;
//using System.ServiceModel;
//using System.ServiceModel.Channels;
//using System.ServiceModel.Security;
//using FluentAssertions;
//using NUnit.Framework;
//using TypeMock.ArrangeActAssert;

//namespace Dlbr.CommonLogin.IdentityModel.Tests.Unit.IdentityModel
//{
//    [TestFixture]
//    public class WcfServiceWrapperTests
//    {
//        [Test]
//        [Isolated]
//        [Category("TypeMock")]
//        public void WcfServiceWrapper__CreateChannelThrows_FactoryAbortShouldBeCalled()
//        {
//            // Arrange
//            SecurityToken token = new UserNameSecurityToken("user", "pwd");
//            var serviceWrapper = new WcfServiceWrapper<ITestServiceInterface>(token, "https://some.test.address/");

//            var channelFactory = Isolate.Fake.Instance<ChannelFactory<ITestServiceInterface>>();
//            Isolate.Swap.NextInstance<ChannelFactory<ITestServiceInterface>>().With(channelFactory);
//            Isolate.WhenCalled(() => channelFactory.CreateChannelWithIssuedToken(null)).WillThrow(new Exception("TEST"));

//            // Act
//            Action action = () => serviceWrapper.Channel.TestMethod();

//            // Assert
//            action.ShouldThrow<Exception>().WithMessage("TEST");
//            Isolate.Verify.WasCalledWithExactArguments(() => channelFactory.Abort());
//        }

//        [Test]
//        [Isolated]
//        [Category("TypeMock")]
//        public void WcfServiceWrapper_IdpInfoProvided_ServiceReferenceReturned()
//        {
//            // Arrange
//            var channelFactory = Isolate.Fake.Instance<ChannelFactory<ITestServiceInterface>>();
//            Isolate.Swap.NextInstance<ChannelFactory<ITestServiceInterface>>().With(channelFactory);
//            Isolate.WhenCalled(() => channelFactory.CreateChannelWithIssuedToken(null)).WillReturn(new TestService());

//            Isolate.WhenCalled(() => AdfsHelper.GetActAsToken("", "", "", ""))
//                .WillReturn(new UserNameSecurityToken("user", "pwd"));

//            // Act
//            string answerFromService;
//            Binding binding;
//            using (
//                var serviceWrapper = new WcfServiceWrapper<ITestServiceInterface>("idp", "realm", "user", "pwd",
//                    "https://some.test.address/"))
//            {
//                var service = serviceWrapper.Channel;
//                answerFromService = service.TestMethod();
//                binding = serviceWrapper.Binding;
//            }

//            // Assert
//            answerFromService.Should().Be("1");
//            var cr = channelFactory.Credentials;
//            cr.Should().NotBeNull();
//            if (cr == null) return;
//            cr.ServiceCertificate.Authentication.RevocationMode.Should().Be(X509RevocationMode.NoCheck);
//            cr.ServiceCertificate.Authentication.CertificateValidationMode.Should()
//                .Be(X509CertificateValidationMode.None);
//            cr.SupportInteractive.Should().BeFalse();

//            binding.Should().BeAssignableTo<WS2007FederationHttpBinding>();
//            ((WS2007FederationHttpBinding) binding).Security.Message.IssuedKeyType.Should()
//                .Be(SecurityKeyType.BearerKey);
//            ((WS2007FederationHttpBinding) binding).Security.Message.EstablishSecurityContext.Should().BeFalse();
//        }

//        [Test]
//        [Isolated]
//        [Category("TypeMock")]
//        public void WcfServiceWrapper_IdpInfoProvidedActAsFalse_ServiceReferenceReturned()
//        {
//            // Arrange
//            var channelFactory = Isolate.Fake.Instance<ChannelFactory<ITestServiceInterface>>();
//            Isolate.Swap.NextInstance<ChannelFactory<ITestServiceInterface>>().With(channelFactory);
//            Isolate.WhenCalled(() => channelFactory.CreateChannelWithIssuedToken(null)).WillReturn(new TestService());

//            Isolate.WhenCalled(() => AdfsHelper.GetSecurityToken("", "", "", ""))
//                .WillReturn(new UserNameSecurityToken("user", "pwd"));

//            // Act
//            string answerFromService;
//            Binding binding;
//            using (
//                var serviceWrapper = new WcfServiceWrapper<ITestServiceInterface>("idp", "realm", "user", "pwd",
//                    "https://some.test.address/", false))
//            {
//                var service = serviceWrapper.Channel;
//                answerFromService = service.TestMethod();
//                binding = serviceWrapper.Binding;
//            }

//            // Assert
//            answerFromService.Should().Be("1");
//            var cr = channelFactory.Credentials;
//            cr.Should().NotBeNull();
//            if (cr == null) return;
//            cr.ServiceCertificate.Authentication.RevocationMode.Should().Be(X509RevocationMode.NoCheck);
//            cr.ServiceCertificate.Authentication.CertificateValidationMode.Should()
//                .Be(X509CertificateValidationMode.None);
//            cr.SupportInteractive.Should().BeFalse();

//            binding.Should().BeAssignableTo<WS2007FederationHttpBinding>();
//            ((WS2007FederationHttpBinding) binding).Security.Message.IssuedKeyType.Should()
//                .Be(SecurityKeyType.BearerKey);
//            ((WS2007FederationHttpBinding) binding).Security.Message.EstablishSecurityContext.Should().BeFalse();
//        }

//        [Test]
//        [Isolated]
//        [Category("TypeMock")]
//        public void WcfServiceWrapper_SecurityTokenProvided_ServiceReferenceReturned()
//        {
//            // Arrange
//            SecurityToken token = new UserNameSecurityToken("user", "pwd");

//            var channelFactory = Isolate.Fake.Instance<ChannelFactory<ITestServiceInterface>>();
//            Isolate.Swap.NextInstance<ChannelFactory<ITestServiceInterface>>().With(channelFactory);
//            Isolate.WhenCalled(() => channelFactory.CreateChannelWithIssuedToken(null)).WillReturn(new TestService());

//            // Act
//            string answerFromService;
//            Binding binding;
//            using (
//                var serviceWrapper = new WcfServiceWrapper<ITestServiceInterface>(token, "https://some.test.address/"))
//            {
//                var service = serviceWrapper.Channel;
//                answerFromService = service.TestMethod();
//                binding = serviceWrapper.Binding;
//            }

//            // Assert
//            answerFromService.Should().Be("1");
//            var cr = channelFactory.Credentials;
//            cr.Should().NotBeNull();
//            if (cr == null) return;
//            cr.ServiceCertificate.Authentication.RevocationMode.Should().Be(X509RevocationMode.NoCheck);
//            cr.ServiceCertificate.Authentication.CertificateValidationMode.Should()
//                .Be(X509CertificateValidationMode.None);
//            cr.SupportInteractive.Should().BeFalse();

//            binding.Should().BeAssignableTo<WS2007FederationHttpBinding>();
//            ((WS2007FederationHttpBinding) binding).Security.Message.IssuedKeyType.Should()
//                .Be(SecurityKeyType.BearerKey);
//            ((WS2007FederationHttpBinding) binding).Security.Message.EstablishSecurityContext.Should().BeFalse();
//        }
//    }

//    public interface ITestServiceInterface : IClientChannel
//    {
//        string TestMethod();
//    }

//    internal class TestService : ITestServiceInterface
//    {
//        public string TestMethod()
//        {
//            return "1";
//        }

//        public void Abort()
//        {
//        }

//        public void Close()
//        {
//        }

//        public void Close(TimeSpan timeout)
//        {
//        }

//        public IAsyncResult BeginClose(AsyncCallback callback, object state)
//        {
//            return null;
//        }

//        public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
//        {
//            return null;
//        }

//        public void EndClose(IAsyncResult result)
//        {
//        }

//        public void Open()
//        {
//        }

//        public void Open(TimeSpan timeout)
//        {
//        }

//        public IAsyncResult BeginOpen(AsyncCallback callback, object state)
//        {
//            return null;
//        }

//        public IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
//        {
//            return null;
//        }

//        public void EndOpen(IAsyncResult result)
//        {
//        }

//        public CommunicationState State { get; private set; }
//        public event EventHandler Closed = delegate { };
//        public event EventHandler Closing = delegate { };
//        public event EventHandler Faulted = delegate { };
//        public event EventHandler Opened = delegate { };
//        public event EventHandler Opening = delegate { };
//        public event EventHandler<UnknownMessageReceivedEventArgs> UnknownMessageReceived = delegate { };

//        public T GetProperty<T>() where T : class
//        {
//            return null;
//        }

//        public IExtensionCollection<IContextChannel> Extensions { get; private set; }
//        public bool AllowOutputBatching { get; set; }
//        public IInputSession InputSession { get; private set; }
//        public EndpointAddress LocalAddress { get; private set; }
//        public TimeSpan OperationTimeout { get; set; }
//        public IOutputSession OutputSession { get; private set; }
//        public EndpointAddress RemoteAddress { get; private set; }
//        public string SessionId { get; private set; }

//        public void Dispose()
//        {
//        }

//        public void DisplayInitializationUI()
//        {
//        }

//        public IAsyncResult BeginDisplayInitializationUI(AsyncCallback callback, object state)
//        {
//            return null;
//        }

//        public void EndDisplayInitializationUI(IAsyncResult result)
//        {
//        }

//        public bool AllowInitializationUI { get; set; }
//        public bool DidInteractiveInitialization { get; private set; }
//        public Uri Via { get; private set; }
//    }
//}