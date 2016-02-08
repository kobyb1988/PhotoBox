using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using ImageMaker.AppServer;
using ImageMaker.Runner.Annotations;
using NLog;

namespace ImageMaker.Runner
{
    public class Server : IDisposable
    {
        [UsedImplicitly] private static Lazy<Server> _server = new Lazy<Server>();

        public static Server Instance
        {
            get { return _server.Value; }
        }

        private ServiceHost _host;
        public void Launch()
        {
            _host = new ServiceHost(typeof(AppService), new Uri("net.tcp://localhost:8732/"));
            try
            {
                _host.Open();
                LogManager.GetCurrentClassLogger().Info("start server");
            }
            catch (TimeoutException ex)
            {
                LogManager.GetCurrentClassLogger().Error(ex);
            }
            catch (CommunicationException ex)
            {
                LogManager.GetCurrentClassLogger().Error(ex);
            }
        }

        private readonly ConcurrentDictionary<string, ICallbackContract> _channels = 
            new ConcurrentDictionary<string, ICallbackContract>();
 
        public void AddSession(ICallbackContract callback, string sessionId)
        {
            _channels.TryAdd(sessionId, callback);
        }

        public void EnumerateClients(Action<ICallbackContract> action, string sessonId)
        {
            Console.WriteLine("message from {0}", sessonId);

            foreach (var channelInfo in _channels.ToArray().Where(x => string.Compare(x.Key, sessonId, StringComparison.OrdinalIgnoreCase) != 0))
            {
// ReSharper disable once SuspiciousTypeConversion.Global
                var commObj = channelInfo.Value as ICommunicationObject;
                if (commObj == null)
                    continue;

                if (commObj.State != CommunicationState.Opened)
                {
                    ICallbackContract channel;
                    _channels.TryRemove(channelInfo.Key, out channel);
                    continue;
                }

                action(channelInfo.Value);
            }
        }

        public void Dispose()
        {
            try
            {
                _host.Abort();
            }
            finally
            {
                
            }
        }
    }
}
