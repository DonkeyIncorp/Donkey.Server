﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Donkey.Administration.WebApi
{
    public interface IKeyManager
    {
        //Task<string> GetKeyAsync(string receiver, string firstKey);
        Task<bool> KeyValidAsync(string key);
    }
}
