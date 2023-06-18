﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNPM.Core.Interfaces.Api
{
    public interface IApiService
    {
        public Func<string, Task<bool>> GetRemoteVerifier();
    }
}