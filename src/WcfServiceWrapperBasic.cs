using System;
using System.ServiceModel;

namespace Dlbr.CommonLogin.IdentityModel
{
    /// <summary>
    /// Class used to connect to a basic endpoint of a WCF Service (via HTTPS).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WcfServiceWrapperBasic<T> : IDisposable
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

        private ChannelFactory<T> factory;
        private bool connected;
        private T channel;
        private readonly string serviceAddress;
	    private BasicHttpBinding binding;

        public WcfServiceWrapperBasic(string serviceAddress)
        {
            this.serviceAddress = serviceAddress;
        }

        public T Channel
        {
            get
            {
                ConnectToWcfService();
                return channel;
            }
        }

        public void Dispose()
        {
            if (connected)
            {
                if (((IClientChannel)channel).State != CommunicationState.Faulted)
                {
                    ((IClientChannel)channel).Close();
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

		public BasicHttpBinding Binding
		{
			get { return binding; }
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

            binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);

            factory = new ChannelFactory<T>(binding, serviceAddress);
			InvokeConfigureChannel();
            try
            {
                //TODO Upgrade 4.5 - http://social.msdn.microsoft.com/Forums/vstudio/en-US/9cffe4ea-5a0d-4729-9eb1-b56629b175dc/configurechannelfactory-in-net-45?forum=Geneva
                //factory.ConfigureChannelFactory();
                channel = factory.CreateChannel();
                connected = true;
            }
            catch (Exception)
            {
                factory.Abort();
                throw;
            }
        }
    }
}
