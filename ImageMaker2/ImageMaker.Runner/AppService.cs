using System;
using System.ServiceModel;
using ImageMaker.AppServer;

namespace ImageMaker.Runner
{

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class AppService : ICallingContract
    {
        public void SendCommand(Command command)
        {
            Server.Instance.EnumerateClients(x => x.SendCommand(command), OperationContext.Current.SessionId);
        }

        public void Connect()
        {
            Server.Instance.AddSession(Callback, OperationContext.Current.SessionId);
        }

        ICallbackContract Callback
        {
            get { return OperationContext.Current.GetCallbackChannel<ICallbackContract>(); }
        }
    }
}
