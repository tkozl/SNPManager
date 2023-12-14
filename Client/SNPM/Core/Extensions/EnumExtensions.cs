using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace SNPM.Core.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum enumValue)
        {
            object[] attr = enumValue
                .GetType().GetField(enumValue.ToString())!.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attr.Length > 0) 
            {
                return ((DescriptionAttribute)attr[0]).Description;
            }
            else
            {
                throw new KeyNotFoundException("No DescriptionAttribute found for given enum value");
            }
        }

        public static IEnumerable<Enum> GetUniqueFlags(this Enum flags)
        {
            ulong flag = 1;
            foreach (var value in Enum.GetValues(flags.GetType()).Cast<Enum>())
            {
                ulong bits = Convert.ToUInt64(value);
                while (flag < bits)
                {
                    flag <<= 1;
                }

                if (flag == bits && flags.HasFlag(value))
                {
                    yield return value;
                }
            }
        }
    }
}
