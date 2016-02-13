using System.ServiceModel;

namespace ImageMaker.AppServer
{
    public interface ICallbackContract
    {
        [OperationContract(IsOneWay = true)]
        void SendCommand(BaseCommand command);
        [OperationContract(IsOneWay = true)]
        void Ping();

    }
}