using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ImageMaker.CommonViewModels.Commands
{
    public class ThreadTools
    {
        public static void RunInDispatcher(Dispatcher dispatcher, Action action)
        {
            RunInDispatcher(dispatcher, DispatcherPriority.Normal, action);
        }

        public static void RunInDispatcher(Dispatcher dispatcher, DispatcherPriority priority, Action action)
        {
            if (action == null) { return; }

            if (dispatcher.CheckAccess())
            {
                // we are already on thread associated with the dispatcher -> just call action
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    //Log error here!
                }
            }
            else
            {
                // we are on different thread, invoke action on dispatcher's thread
                dispatcher.BeginInvoke(
                    priority,
                    (Action)(
                    () =>
                    {
                        try
                        {
                            action();
                        }
                        catch (Exception ex)
                        {
                        //Log error here!
                    }
                    })
                );
            }
        }
    }
}
