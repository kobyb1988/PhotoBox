using System;
using System.Collections.Generic;
using System.Linq;
using ImageMaker.Common.Extensions;

namespace ImageMaker.AdminViewModels.Helpers
{
    public static class IEnumerableHelpers
    {
        public static IList<KeyValuePair<T, string>> ToKeyValue<T>(this IList<T> values) 
        {
            return values.Select(x => new KeyValuePair<T, string>(x, x.GetDescription())).ToList();
        }
    }
}
