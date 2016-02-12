using System.Threading.Tasks;
using System.Windows.Input;

namespace ImageMaker.CommonViewModels.Async
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
}
