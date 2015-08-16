using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace EDSDKLib.CommandQueue
{
    internal class ActionInfo
    {
        private readonly Action<Camera> _action;
        private readonly bool _invokeRequired;
        private readonly Dispatcher _dispatcher;
        private readonly Camera _camera;

        public ActionInfo(Action<Camera> action, bool invokeRequired, Dispatcher dispatcher, Camera camera)
        {
            _action = action;
            _invokeRequired = invokeRequired;
            _dispatcher = dispatcher;
            _camera = camera;
        }

        public void Exec()
        {
            if (_invokeRequired)
            {
                _dispatcher.BeginInvoke(new Action(() => _action(_camera)));
                return;
            }

            _action(_camera);
        }
    }
}
