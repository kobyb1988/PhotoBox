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
        public static string GetDescription<TEnum>(this TEnum value)
        {
            var attributes =
                (DescriptionAttribute[])value.GetType().GetField(value.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
        public static IEnumerable<EnumType> GetEnumValues<EnumType>()
          where EnumType : struct, IConvertible
        {
            Array enumValues = Enum.GetValues(typeof(EnumType));

            return enumValues.Cast<EnumType>();
        }

        public static EnumType GetNextEnumValue<EnumType>(this EnumType value)
        {
            var next=Enum.GetValues(typeof(EnumType)).Cast<EnumType>().
                SkipWhile(e => !Equals(e, value)).Skip(1).FirstOrDefault();
            if (next == null) return Enum.GetValues(typeof (EnumType)).Cast<EnumType>().First();
            return next;
        }
    }

}
