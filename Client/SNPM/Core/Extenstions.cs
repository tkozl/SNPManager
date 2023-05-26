﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNPM.Core
{
   public static class Extenstions
   {
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
