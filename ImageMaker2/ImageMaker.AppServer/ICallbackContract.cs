using System.ServiceModel;

namespace ImageMaker.AppServer
{
    public interface ICallbackContract
    {
        [OperationContract(IsOneWay = true)]
        void SendCommand(Command command);
    }
}