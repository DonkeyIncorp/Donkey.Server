using System;
using System.Collections.Generic;
using System.Text;

namespace Donkey.Administration.Environment
{
    public class ClientExistsException : Exception
    {
        private readonly string _email;

        public override string Message => "Клиент уже существует";

        public ClientExistsException(string email)
        {
            _email = email;
        }
    }
}
