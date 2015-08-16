using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageMaker.Common.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this object value)
        {
            var attributes =
                (DescriptionAttribute[])value.GetType().GetField(value.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }
}
