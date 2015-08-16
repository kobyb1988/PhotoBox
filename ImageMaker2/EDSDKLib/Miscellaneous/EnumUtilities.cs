using System;
using System.Collections.Generic;
using System.Linq;

namespace EDSDKLib.Miscellaneous
{
    public static class EnumUtilities
    {
        public static IEnumerable<EnumType> GetEnumValues<EnumType>()
            where EnumType : struct, IConvertible
        {
            Array enumValues = Enum.GetValues(typeof(EnumType));

            return enumValues.Cast<EnumType>();
        }
    }
}
