using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;

namespace Dlbr.CommonLogin.IdentityModel
{
    public class WcfServiceWrapper<T> : IDisposable
    {
        public delegate void ConfigureChannelEventHandler(ChannelFactory<T> factory);

        /// <summary>
        /// You may setup this event to configure the WCF channel before use. You may e.g. add custom behaviors. Here is some example usages from DCF
        /// Services:
        /// 
        ///    service.ConfigureChannel += factory => factory.Endpoint.Behaviors.Add(new InternationalizationBehavior());
        /// 
        ///    service.ConfigureChannel += factory =>
        ///    {
        ///        service.Binding.MaxReceivedMessageSize = 200000000;
        ///        var operation = factory.Endpoint.Contract.Operations.Find("GetAnimalDataByDistrictNumber");
        ///        operation.Behaviors.Find<DataContractSerializerOperationBehavior>().MaxItemsInObjectGraph = 2000000;
        ///    };
        /// 
        /// </summary>
        public event ConfigureChannelEventHandler ConfigureChannel;

        private SecurityToken token;
        private readonly string serviceAddress;
        private ChannelFactory<T> factory;
        private bool connected;
        private T channel;
        private readonly string idpEndpoint;
        private readonly string realm;
        private readonly string serviceUserName;
        private readonly string servicePassword;
        private WS2007FederationHttpBinding binding;
        private readonly bool useActAs;
        private ObservableCollection<WcfHeaderInfo> headers;
        private OperationContextScope contextScope = null;

        public WcfServiceWrapper(SecurityToken token, string serviceAddress, IEnumerable<WcfHeaderInfo> headers = null)
        {
            this.token = token;
            this.serviceAddress = serviceAddress;
            SetWchHeaders(headers);
        }

        public WcfServiceWrapper(string idpEndpoint, string realm, string serviceUserName, string servicePassword, string serviceAddress,
            bool useActAs = true, IEnumerable<WcfHeaderInfo> headers = null)
        {
            this.idpEndpoint = idpEndpoint;
            this.realm = realm;
            this.serviceUserName = serviceUserName;
            this.servicePassword = servicePassword;
            this.serviceAddress = serviceAddress;
            this.useActAs = useActAs;
            SetWchHeaders(headers);
        }

        public T Channel
        {
            get
            {
                ConnectToWcfService();
                return channel;
            }
        }

        public ICollection<WcfHeaderInfo> Headers
        {
            get { return headers; }
            set 
            {
                SetWchHeaders(value);
                if (connected)
                {
                    SetupWcfHeader();
                }
                else
                {
                    ConnectToWcfService();
                }
            }
        }

        public WS2007FederationHttpBinding Binding
        {
            get { return binding; }
        }

        private void SetWchHeaders(IEnumerable<WcfHeaderInfo> newHeaders)
        {
            if (newHeaders != null)
            {
                headers = new ObservableCollection<WcfHeaderInfo>(newHeaders);
                headers.CollectionChanged += HandleWcfHeaderChange;
            }
        }

        private void HandleWcfHeaderChange(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (connected)
            {
                SetupWcfHeader();
            }
        }

        private void InvokeConfigureChannel()
        {
            ConfigureChannelEventHandler handler = ConfigureChannel;
            if (handler != null)
                handler(factory);
        }

        private void ConnectToWcfService()
        {
            if (connected)
            {
                return;
            }
            if (token == null)
            {
                if (useActAs)
                {
                    token = AdfsHelper.GetActAsToken(idpEndpoint, realm, serviceUserName, servicePassword);
                }
                else
                {
                    token = AdfsHelper.GetSecurityToken(idpEndpoint, realm, serviceUserName, servicePassword);
                }
            }

            binding = new WS2007FederationHttpBinding(WSFederationHttpSecurityMode.TransportWithMessageCredential);
            binding.Security.Message.IssuedKeyType = SecurityKeyType.BearerKey;
            binding.Security.Message.EstablishSecurityContext = false;

            // create factory and enable WIF plumbing
            factory = new ChannelFactory<T>(binding, serviceAddress);
            InvokeConfigureChannel();
            try
            {
                factory.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
                factory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;

                //TODO Upgrade 4.5 - http://social.msdn.microsoft.com/Forums/vstudio/en-US/9cffe4ea-5a0d-4729-9eb1-b56629b175dc/configurechannelfactory-in-net-45?forum=Geneva
                //factory.ConfigureChannelFactory();

                factory.Credentials.SupportInteractive = false;

                channel = factory.CreateChannelWithIssuedToken(token);
                SetupWcfHeader();
                connected = true;
            }
            catch (Exception)
            {
                factory.Abort();
                throw;
            }
        }

        private void SetupWcfHeader()
        {
            if (contextScope != null)
                contextScope.Dispose();

            if (headers == null || headers.Count == 0)
            {
                return;
            }

            contextScope = new OperationContextScope((IContextChannel)channel);
            foreach (WcfHeaderInfo info in headers)
            {
                MessageHeader<string> messageHeader = new MessageHeader<string>(info.Value);
                MessageHeader header = messageHeader.GetUntypedHeader(info.Key, info.Namespace);
                OperationContext.Current.OutgoingMessageHeaders.Add(header);
            }
        }

        public void Dispose()
        {
            if (contextScope != null)
                contextScope.Dispose();
            if (connected)
            {
                if (((IClientChannel) channel).State != CommunicationState.Faulted)
                {
                    ((IClientChannel) channel).Close();
                }
                else
                {
                    ((IClientChannel)channel).Abort();
                }
                if (factory.State != CommunicationState.Faulted)
                {
                    factory.Close();
                }
                else
                {
                    factory.Abort();
                }
            }
        }
    }
}
