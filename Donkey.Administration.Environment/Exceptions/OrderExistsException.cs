using System;
using System.Collections.Generic;
using System.Text;

namespace Donkey.Administration.Environment
{
    public class OrderExistsException : Exception
    {
        private readonly string _email;
        private readonly string _orderName;

        public override string Message => $"Заказ {_orderName} клиента {_email} уже существуетв системе";
        public OrderExistsException(string email, string orderName)
        {
            _email = email;
            _orderName = orderName;
        }
    }
}
