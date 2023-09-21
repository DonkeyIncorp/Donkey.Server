using System;

namespace Donkey.Administration.Environment
{
    public class ClientNotFoundException : Exception
    {
        private readonly string _email;
        public override string Message => $"Не удалось найти клиента {_email}";

        public ClientNotFoundException(string email)
        {
            _email = email;
        }
    }
}
