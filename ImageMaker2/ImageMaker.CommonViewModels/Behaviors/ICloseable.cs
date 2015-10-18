using System;

namespace ImageMaker.CommonViewModels.Behaviors
{
    public interface ICloseable
    {
        event EventHandler<bool> StateChanged;

        event Action<WindowState> RequestWindowVisibilityChanged;

        void OnClose();
    }

    public enum WindowState
    {
        Closed = 0,
        Hidden = 1,
        Visible = 2
    }
}
