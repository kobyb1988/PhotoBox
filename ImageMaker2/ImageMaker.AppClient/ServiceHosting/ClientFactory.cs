using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMaker.AppServer;

namespace ImageMaker.AppClient.ServiceHosting
{
    public class ClientFactory
    {
        private Client _client;

        public virtual void CreateClient(Action<Command> onCommand)
        {
            if (_client != null)
                return;

            _client = new Client(new ClientCallback(onCommand));

            try
            {
                _client.Connect();
            }
            catch (Exception)
            {
            }
        }

        public virtual void SendCommand(Command command)
        {
            if (_client != null)
            {
                Task.Run(() =>
                         {
                             try
                             {
                                 _client.SendCommand(command);
                             }
                             catch (Exception)
                             {
                             }
                         });
            }
        }

        public virtual void Abort()
        {
            if (_client == null)
                return;

            try
            {
                _client.Close();
                _client = null;
            }
            catch (Exception)
            {
            }
        }
    }
}
