using ObjectSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace Donkey.Administration.BusinessLogic
{
    public class OrderStatusChangedEventArgs : EventArgs
    {
        public Order Order { get; }
        public OrderStatus CurrentStatus { get; }
        public OrderStatusChangedEventArgs(Order order, OrderStatus currentStatus)
        {
            Order = order;
            CurrentStatus = currentStatus;
        }
    }
}
