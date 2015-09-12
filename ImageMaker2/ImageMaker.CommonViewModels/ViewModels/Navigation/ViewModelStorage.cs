using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMaker.Common.Extensions;

namespace ImageMaker.CommonViewModels.ViewModels.Navigation
{
    public class ViewModelStorage
    {
        private readonly LinkedList<BaseViewModel> _navigationOrder = new LinkedList<BaseViewModel>();

        public BaseViewModel Next(BaseViewModel to)
        {
            return _navigationOrder.AddFirst(to).Value;
        }

        public BaseViewModel Next(BaseViewModel from, BaseViewModel to)
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

            return node.Value;
        }

        public BaseViewModel Previous(BaseViewModel from)
        {
            var listNode = _navigationOrder.TryGet(from);

            return listNode.Previous == null ? null : listNode.Previous.Value;
        } 
    }
}
