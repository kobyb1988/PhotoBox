using System;
using System.ServiceModel;
using ImageMaker.AppServer;

namespace ImageMaker.AppClient.ServiceHosting
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ClientCallback : ICallbackContract
    {
        public ClientCallback(Action<BaseCommand> onCommandReceived)
        {
            OnCommandReceived = onCommandReceived;
        }

        public Action<BaseCommand> OnCommandReceived { get; private set; }

        public void Ping()
        {
            
        }

        public void SendCommand(BaseCommand command)
        {
            OnCommandReceived(command);
        }
    }
}
