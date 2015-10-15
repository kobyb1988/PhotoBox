using ImageMaker.AppClient.ServiceHosting;
using ImageMaker.AppServer;
using ImageMaker.CommonViewModels.Messenger;

namespace ImageMaker.CommonViewModels.Services
{
    public class CommunicationManager
    {
        private readonly IMessenger _messenger;
        private readonly ClientFactory _clientFactory;

        public CommunicationManager(IMessenger messenger, ClientFactory clientFactory )
        {
            _messenger = messenger;
            _clientFactory = clientFactory;
        }

        public virtual void Connect()
        {
            _clientFactory.CreateClient((command) =>
                                        {
                                            CommandMessage message = _messenger.CreateMessage<CommandMessage>();
                                            message.Command = command;
                                            _messenger.Send(message);
                                        });
        }

        public virtual void SendHideCommand()
        {
            _clientFactory.SendCommand(new Command());
        }

        public virtual void Abort()
        {
            _clientFactory.Abort();
        }
    }

    public class CommandMessage
    {
        public Command Command { get; set; }
    }
}
