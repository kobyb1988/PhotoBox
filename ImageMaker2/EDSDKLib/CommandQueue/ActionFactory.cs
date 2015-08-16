using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using ConcurrentPriorityQueue;

namespace EDSDKLib.CommandQueue
{
    internal class ActionFactory
    {
        private readonly ConcurrentPriorityQueue<ActionInfo, Priority> _queue = new ConcurrentPriorityQueue<ActionInfo, Priority>();
        private readonly Dispatcher _dispatcher;

        internal ActionFactory()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        internal void Enqueue(Action<Camera> queueAction, Camera camera, PriorityValue priority)
        {
            _queue.Enqueue(new ActionInfo(queueAction, true, _dispatcher, camera), new Priority(priority));

            if (_queue.Count > 1)
                return;

            ActionInfo result = null;
            result = _queue.Peek();
            //_queue.TryPeek(out result);
            if (result != null)
            {
                _queue.UpdatePriority(result, new Priority(PriorityValue.Critical));
                try
                {
                    result.Exec();
                }
                catch (Exception e)
                {
                    Debug.WriteLine("message: {0}; stacktrace: {1}", e.Message, e.StackTrace);
                }
            }
        }

        internal void Dequeue()
        {
            if (_queue.Count <= 0)
                return;

            ActionInfo result = null;
            result = _queue.Dequeue();
            if (result == null)
                return;
            
            if (_queue.Count <= 0)
                return;

            result = _queue.Peek();
            if (result != null)
            {
                _queue.UpdatePriority(result, new Priority(PriorityValue.Critical));
                try
                {
                    result.Exec();
                }
                catch (Exception e)
                {
                    Debug.WriteLine("message: {0}; stacktrace: {1}", e.Message, e.StackTrace);
                }
            }
        }

        internal void Clear()
        {
            _queue.Clear();
        }

        internal int Count
        {
            get { return _queue.Count; }
        }
    }
}
