using ObjectSystem;
using System;
using System.Threading.Tasks;
using NLog;
using Donkey.Administration.Data;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Donkey.Administration.BusinessLogic
{
    public class ClientManager : IClientManager
    {
        public event Action<ClientAddedEventArgs> ClientAdding;
        public event Action<ClientAddedEventArgs> ClientAdded;
        public event Action<ClientRemovedEventArgs> ClientRemoving;
        public event Action<ClientRemovedEventArgs> ClientRemoved;

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IClientDataAccess _clientDataAccess;

        public ClientManager(IClientDataAccess clientDataAccess)
        {
            _clientDataAccess = clientDataAccess;
        }

        public async Task AddClientAsync(Client client)
        {
            try
            {
                if(client == null)
                {
                    throw new ArgumentNullException(nameof(Client));
                }

                OnClientAdding(client);

                await _clientDataAccess.AddClientAsync(client).ConfigureAwait(false);

                OnClientAdded(client);
            }
            catch(Exception ex)
            {
                _logger.Error(ex, $"Не удалось добавить клиента {client.SecondName}");
                throw;
            }
        }

        public async Task<bool> ClientExistsAsync(string email)
        {
            if (email == null)
            {
                throw new ArgumentNullException(nameof(email));
            }

            try
            {
                bool exists = await _clientDataAccess.ClientExistsAsync(email).ConfigureAwait(false);

                return exists;
            }
            catch(Exception ex)
            {
                _logger.Error(ex, $"Не удалось получить информацию о существовании клиента {email ?? "[null]"}");
                throw;
            }
        }

        public async Task<Client> GetClientInfoAsync(string email)
        {
            if (email == null)
            {
                throw new ArgumentNullException(nameof(email));
            }

            try
            {
                var info = await _clientDataAccess.GetClientAsync(email).ConfigureAwait(false);
                return info;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Не удалось получить информацию о клиенте {email ?? "[null]"}");
                throw;
            }
        }

        public Task<bool> RemoveClientAsync(string email)
        {
            if (email == null)
            {
                throw new ArgumentNullException(nameof(email));
            }

            try
            {
                
                return _clientDataAccess.ClientExistsAsync(email);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Не удалось удалить клиента {email ?? "[null]"}");
                throw;
            }
        }

        private void OnClientAdded(Client client)
        {
            ClientAdded?.Invoke(new ClientAddedEventArgs(client));
        }

        private void OnClientAdding(Client client)
        {
            ClientAdding?.Invoke(new ClientAddedEventArgs(client));
        }

        private void OnClientRemoved(Client client)
        {
            ClientRemoved?.Invoke(new ClientRemovedEventArgs(client));
        }

        private void OnClientRemoving(Client client)
        {
            ClientRemoving?.Invoke(new ClientRemovedEventArgs(client));
        }

        public async Task<IEnumerable<Client>> GetAllClientsInfoAsync()
        {
            try
            {
                var info = await _clientDataAccess.GetAllClientsAsync().ConfigureAwait(false);
                return info;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Не удалось получить информацию обо всех клиентах");
                throw;
            }
        }

        public async Task<IEnumerable<Order>> GetClientOrdersAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException(nameof(email));
            }

            try
            {

                var orders = await _clientDataAccess.GetClientOrders(email).ConfigureAwait(false);

                return orders;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Не удалось получить заказы клиента {email}");
                throw;
            }
        }

        public async Task<Client> GetClientInfoAsync(long id)
        {
            try
            {
                return await _clientDataAccess.GetClientAsync(id).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Не удалось получить информацию о клиенте {id}");
                throw;
            }
        }
    }
}
