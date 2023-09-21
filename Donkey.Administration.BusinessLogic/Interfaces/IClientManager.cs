using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ObjectSystem;

namespace Donkey.Administration.BusinessLogic
{
    public interface IClientManager
    {
        Task AddClientAsync(Client client);
        Task<bool> RemoveClientAsync(string email);
        Task<bool> ClientExistsAsync(string email);
        Task<Client> GetClientInfoAsync(string email);
        Task<Client> GetClientInfoAsync(long id);
        Task<IEnumerable<Client>> GetAllClientsInfoAsync();

        Task<IEnumerable<Order>> GetClientOrdersAsync(string email);

        event Action<ClientAddedEventArgs> ClientAdding;
        event Action<ClientAddedEventArgs> ClientAdded;

        event Action<ClientRemovedEventArgs> ClientRemoving;
        event Action<ClientRemovedEventArgs> ClientRemoved;
    }
}
