using ObjectSystem;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Donkey.Administration.Data
{
    public interface IOrderDataAccess
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<IEnumerable<Order>> GetActualOrdersAsync();
        Task<IEnumerable<Order>> GetAllClientOrdersAsync(string email);

        Task<Order> GetOrderFullInfoAsync(string guid);

        Task AddOrderAsync(Order order, string email);

        Task UpdateOrderAsync(Order updatedOrder, string updaterLogin);
        Task RemoveOrderAsync(string id);
    }
}
