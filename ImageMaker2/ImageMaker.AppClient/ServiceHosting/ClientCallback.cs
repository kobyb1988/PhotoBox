using System;
using System.ServiceModel;
using ImageMaker.AppServer;

namespace ImageMaker.AppClient.ServiceHosting
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ClientCallback : ICallbackContract
    {
        public ClientCallback(Action<Command> onCommandReceived)
        {
            OnCommandReceived = onCommandReceived;
        }

        public Action<Command> OnCommandReceived { get; private set; }

        public void SendCommand(Command command)
        {
            OnCommandReceived(command);
        }
    }
}
