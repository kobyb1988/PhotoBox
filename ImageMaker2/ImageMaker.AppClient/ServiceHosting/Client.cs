using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using ImageMaker.AppServer;

namespace ImageMaker.AppClient.ServiceHosting
{
    public class Client : DuplexClientBase<ICallingContract>, ICallingContract
    {
        public Client(object callbackInstance) : base(callbackInstance)
        {
        }

        public Client(object callbackInstance, string endpointConfigurationName) : base(callbackInstance, endpointConfigurationName)
        {
        }

        public Client(object callbackInstance, string endpointConfigurationName, string remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        public Client(object callbackInstance, string endpointConfigurationName, EndpointAddress remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        public Client(object callbackInstance, Binding binding, EndpointAddress remoteAddress) : base(callbackInstance, binding, remoteAddress)
        {
        }

        public Client(object callbackInstance, ServiceEndpoint endpoint) : base(callbackInstance, endpoint)
        {
        }

        public Client(InstanceContext callbackInstance) : base(callbackInstance)
        {
        }

        public Client(InstanceContext callbackInstance, string endpointConfigurationName) : base(callbackInstance, endpointConfigurationName)
        {
        }

        public Client(InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        public Client(InstanceContext callbackInstance, string endpointConfigurationName, EndpointAddress remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        public Client(InstanceContext callbackInstance, Binding binding, EndpointAddress remoteAddress) : base(callbackInstance, binding, remoteAddress)
        {
        }

        public Client(InstanceContext callbackInstance, ServiceEndpoint endpoint) : base(callbackInstance, endpoint)
        {
        }

        public void SendCommand(BaseCommand command)
        {
            this.Channel.SendCommand(command);
        }

        public void Connect()
        {
            this.Channel.Connect();
        }

        public void Ping()
        {
            Channel.Ping();
        }
    }
}
