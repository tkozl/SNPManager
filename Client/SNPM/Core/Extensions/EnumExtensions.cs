using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SNPM.Core.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum enumValue)
        {
            object[] attr = enumValue
                .GetType().GetField(enumValue.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attr.Length > 0) // a DescriptionAttribute exists; use it
            {
                return ((DescriptionAttribute)attr[0]).Description;
            }
            else
            {
                throw new KeyNotFoundException("No DescriptionAttribute found for given enum value");
            }
        }
    }
}
