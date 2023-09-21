using System;
using System.Collections.Generic;
using System.Text;

namespace Donkey.Administration.WebApi
{
    public class GetKeyModel
    {
        public string Receiver { get; set; }
        public string FirstKey { get; set; }

        public bool IsCorrect => !string.IsNullOrEmpty(Receiver) && !string.IsNullOrEmpty(FirstKey);
    }
}
