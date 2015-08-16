using System.Collections.Generic;
using ImageMaker.Common.Extensions;
using ImageMaker.CommonViewModels.Messenger;

namespace ImageMaker.CommonViewModels.ViewModels.Navigation
{
    public class ViewModelNavigator : IViewModelNavigator
    {
        private readonly IMessenger _messenger;
        private readonly LinkedList<BaseViewModel> _navigationOrder;

        public ViewModelNavigator(IMessenger messenger)
        {
            _navigationOrder = new LinkedList<BaseViewModel>();
            _messenger = messenger;
        }

        public void NavigateBack(BaseViewModel viewModel)
        {
            var listNode = _navigationOrder.TryGet(viewModel);

            if (listNode.Previous == null)
                return;

            RaiseContentChanged(listNode.Previous.Value);
        }

        private void RaiseContentChanged(BaseViewModel content)
        {
            ContentChangedMessage message = _messenger.CreateMessage<ContentChangedMessage>();
            message.Content = content;
            _messenger.Send(message);
        }

        public void NavigateForward(BaseViewModel from, BaseViewModel to)
        {
            var listNode = _navigationOrder.TryGet(from);
            LinkedListNode<BaseViewModel> node = null;
            if (listNode.Next != null)
            {
                listNode.Next.Value = to;
                node = listNode.Next;
            }
            else
            {
                node = _navigationOrder.AddAfter(listNode, to);    
            }
            
            RaiseContentChanged(node.Value);
        }

        public void NavigateForward(BaseViewModel to)
        {
            var firstNode = _navigationOrder.AddFirst(to);
            RaiseContentChanged(firstNode.Value);
        }
    }
}
