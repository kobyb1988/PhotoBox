using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageMaker.Common.Extensions
{
    public static class LinkedListExtensions
    {
        public static LinkedListNode<TItem> TryGet<TItem>(this LinkedList<TItem> list, TItem item)
        {
            LinkedListNode<TItem> node = list.Find(item);
            if (node == null)
                throw new InvalidOperationException();

            return node;
        } 
    }
}
