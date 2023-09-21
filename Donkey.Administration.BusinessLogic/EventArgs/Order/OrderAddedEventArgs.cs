using ObjectSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace Donkey.Administration.BusinessLogic
{
    public class OrderAddedEventArgs : EventArgs
    {
        public Order AddedOrder { get; }
        public OrderAddedEventArgs(Order addedOrder)
        {
            AddedOrder = addedOrder;
        }
    }
}
