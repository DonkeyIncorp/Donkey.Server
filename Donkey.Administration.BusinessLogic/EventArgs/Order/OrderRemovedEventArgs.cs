using ObjectSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace Donkey.Administration.BusinessLogic
{
    public class OrderRemovedEventArgs : EventArgs
    {
        public Order RemovedOrder { get; }
        public OrderRemovedEventArgs(Order removedOrder)
        {
            RemovedOrder = removedOrder;
        }
    }
}
