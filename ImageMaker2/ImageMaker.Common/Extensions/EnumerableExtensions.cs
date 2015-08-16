using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageMaker.Common.Extensions
{
    public static class EnumerableExtensions
    {
        public static void CopyTo<TItem>(this IEnumerable<TItem> source, IList<TItem> destination)
        {
            destination.Clear();
            foreach (var item in source)
            {
                destination.Add(item);
            }
        }
    }
}
