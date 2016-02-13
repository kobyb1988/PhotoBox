using System.Diagnostics;
using System.Threading;
using ImageMaker.AppClient.ServiceHosting;
using ImageMaker.AppServer;
using ImageMaker.CommonViewModels.Messenger;

namespace ImageMaker.CommonViewModels.Services
{
    public class CommunicationManager
    {
        private readonly ICommandProcessor _commandProcessor;
        private readonly ClientFactory _clientFactory;

        public CommunicationManager(ICommandProcessor commandProcessor, ClientFactory clientFactory )
        {
            _commandProcessor = commandProcessor;
            _clientFactory = clientFactory;
        }

        public virtual void Connect()
        {
            _clientFactory.CreateClient((command) => command.Process(_commandProcessor));
        }

        public virtual void SendHideCommand()
        {
            _clientFactory.SendCommand(new Command());
        }

        public virtual void SendCloseCommand()
        {
            _clientFactory.SendCommand(new CloseCommand(), true);
        }
        public virtual void Ping()
        {
            _clientFactory.Ping();
        }

    }

    public class CommandProcessor : ICommandProcessor
    {
        private readonly IMessenger _messenger;

        public CommandProcessor(IMessenger messenger)
        {
            _messenger = messenger;
        }

        public void ProcessCommand(BaseCommand command)
        {
            CommandMessage message = _messenger.CreateMessage<CommandMessage>();
            message.Command = command;
            _messenger.Send(message);
        }

        public void ProcessCloseCommand(BaseCommand command)
        {
            CloseCommandMessage message = _messenger.CreateMessage<CloseCommandMessage>();
            message.Command = command;
            _messenger.Send(message);
        }
    }

    public class CommandMessage
    {
        public BaseCommand Command { get; set; }
    }

    public class CloseCommandMessage
    {
        public BaseCommand Command { get; set; }
    }
}
