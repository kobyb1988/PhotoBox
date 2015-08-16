using System;

namespace ImageMaker.CommonViewModels.Behaviors
{
    public interface ICloseable
    {
        event EventHandler<bool> StateChanged;

        event Action RequestClose;
    }
}
