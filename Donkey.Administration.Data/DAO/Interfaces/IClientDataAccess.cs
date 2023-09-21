using ObjectSystem;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Donkey.Administration.Data
{
    public interface IClientDataAccess
    {
        Task AddClientAsync(Client client);
        Task RemoveClientAsync(string email);
        Task<bool> ClientExistsAsync(string email);
        Task<Client> GetClientAsync(string email);
        Task<Client> GetClientAsync(long id);

        Task<IEnumerable<Order>> GetClientOrders(string email);

        Task<IEnumerable<Client>> GetAllClientsAsync();
    }
}
