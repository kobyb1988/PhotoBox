using System;

namespace ImageMaker.CommonViewModels.Behaviors
{
    public interface IWindowContainer
    {
        event EventHandler<ShowWindowEventArgs> ShowWindow; 
    }
}
