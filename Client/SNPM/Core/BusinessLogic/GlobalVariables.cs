using SNPM.Core.BusinessLogic.Interfaces;
using System;

namespace SNPM.Core.BusinessLogic
{
    internal class GlobalVariables : IGlobalVariables
    {
        public IntPtr WindowHandle { get; set; }
    }
}
