using System;
using System.Collections.Generic;
using System.Text;

namespace Donkey.Administration.WebApi
{
    public class AuthModel
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }

        public bool IsCorrect => !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(PasswordHash);
    }
}
