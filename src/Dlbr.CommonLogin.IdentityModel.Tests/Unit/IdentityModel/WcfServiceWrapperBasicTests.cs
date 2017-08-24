//using System;
//using System.ServiceModel;
//using System.ServiceModel.Channels;
//using FluentAssertions;
//using NUnit.Framework;
//using TypeMock.ArrangeActAssert;

//namespace Dlbr.CommonLogin.IdentityModel.Tests.Unit.IdentityModel
//{
//    [TestFixture]
//    public class WcfServiceWrapperBasicTests
//    {
//        [Test]
//        [Isolated]
//        [Category("TypeMock")]
//        public void WcfServiceWrapperBasic_AllOkay_ServiceShouldBeCalled()
//        {
//            // Arrange
//            var channelFactory = Isolate.Fake.Instance<ChannelFactory<ITestBasicServiceInterface>>();
//            Isolate.Swap.NextInstance<ChannelFactory<ITestBasicServiceInterface>>().With(channelFactory);
//            Isolate.WhenCalled(() => channelFactory.CreateChannel()).WillReturn(new TestBasicService());

//            // Act
//            string answerFromService;
//            using (var service = new WcfServiceWrapperBasic<ITestBasicServiceInterface>("https://some.test.address/"))
//            {
//                answerFromService = service.Channel.TestMethod();
//            }

//            // Assert
//            answerFromService.Should().Be("1");
//        }

//        [Test]
//        [Isolated]
//        [Category("TypeMock")]
//        public void WcfServiceWrapperBasic_CreateChannelThrows_FactoryIsAborted()
//        {
//            // Arrange
//            var channelFactory = Isolate.Fake.Instance<ChannelFactory<ITestBasicServiceInterface>>();
//            Isolate.Swap.NextInstance<ChannelFactory<ITestBasicServiceInterface>>().With(channelFactory);
//            Isolate.WhenCalled(() => channelFactory.CreateChannel()).WillThrow(new Exception("TEST"));

//            // Act
//            using (var service = new WcfServiceWrapperBasic<ITestBasicServiceInterface>("https://some.test.address/"))
//            {
//                Action action = () => service.Channel.TestMethod();

//                // Assert
//                action.ShouldThrow<Exception>().WithMessage("TEST");
//            }
//        }

//        [Test]
//        [Isolated]
//        [Category("TypeMock")]
//        public void WcfServiceWrapperBasic_TwoServiceCalls_OnlyConnectsOnce()
//        {
//            // Arrange
//            var channelFactory = Isolate.Fake.Instance<ChannelFactory<ITestBasicServiceInterface>>();
//            Isolate.Swap.NextInstance<ChannelFactory<ITestBasicServiceInterface>>().With(channelFactory);
//            Isolate.WhenCalled(() => channelFactory.CreateChannel()).WillReturn(new TestBasicService());

//            // Act
//            string answerFromService;
//            using (var service = new WcfServiceWrapperBasic<ITestBasicServiceInterface>("https://some.test.address/"))
//            {
//                service.Channel.TestMethod();
//                answerFromService = service.Channel.TestMethod();
//            }

//            // Assert
//            answerFromService.Should().Be("1");
//        }
//    }

//    public interface ITestBasicServiceInterface : IClientChannel
//    {
//        string TestMethod();
//    }

//    internal class TestBasicService : ITestBasicServiceInterface
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
//        public event EventHandler Closed = delegate {};
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