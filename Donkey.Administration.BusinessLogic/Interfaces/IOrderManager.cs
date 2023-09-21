using ObjectSystem;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Donkey.Administration.BusinessLogic
{
    public interface IOrderManager
    {
        Task AddOrderAsync(Order order, string email);
        Task RemoveOrderAsync(string orderId);
        Task ChangeOrderStatusAsync(string orderId, OrderStatus newStatus);

        Task UpdateOrderAsync(Order updatedOrder, string updaterLogin);

        Task<IEnumerable<Order>> GetAllOrdersAsync();

        Task<Order> GetOrderFullInfoAsync(string guid);

        event Func<OrderAddedEventArgs, Task> OrderAdded;

        event Action<OrderAddedEventArgs> OrderAdding;
        event Action<OrderRemovedEventArgs> OrderRemoved;
        event Action<OrderRemovedEventArgs> OrderRemoving;
        event Action<OrderStatusChangedEventArgs> OrderStatusChanged;
    }
}
