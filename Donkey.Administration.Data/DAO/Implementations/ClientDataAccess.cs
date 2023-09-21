using Microsoft.EntityFrameworkCore;
using ObjectSystem;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NLog;
using System.Linq;
using Donkey.Administration.Environment;
using Donkey.Administration.Data.Extensions;

namespace Donkey.Administration.Data
{
    internal class ClientDataAccess : DataAccess, IClientDataAccess
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ClientDataAccess(IDbContextFactory<DataContext> contextFactory) : base(contextFactory)
        {
            
        }
        public async Task AddClientAsync(Client client)
        {
            try
            {
                if (client == null)
                {
                    throw new ArgumentNullException(nameof(Client));
                }

                using (var context = ContextFactory.CreateDbContext())
                {
                    var dbClient = await GetDbClientAsync(client.EmailAddress, context);

                    if(dbClient == null)
                    {
                        await context.Clients.AddAsync(new DataModel.Client()
                        {
                            Name = client.Name,
                            SecondName = client.SecondName,
                            ThirdName = client.ThirdName,
                            Email = client.EmailAddress,
                            Contacts = client.Contacts
                        }).ConfigureAwait(false);

                        await context.SaveChangesAsync().ConfigureAwait(false);
                    }
                    else
                    {
                        throw new ClientExistsException(client.EmailAddress);
                    }

                }
            }
            catch(Exception ex)
            {
                _logger.Error(ex, $"Не удалось добавить клиента {client.SecondName ?? "null"}");
                throw;
            }
            
        }

        public async Task<bool> ClientExistsAsync(string email)
        {
            try
            {
                DataModel.Client dbClient;

                using(var context = ContextFactory.CreateDbContext())
                {
                    dbClient = await GetDbClientAsync(email, context).ConfigureAwait(false);
                }

                return dbClient != null;
            }
            catch(Exception ex)
            {
                _logger.Error(ex, $"Не удалось получить информацию о существовании клиента {email?? "null"}");
                throw;
            }
        }

        public async Task<IEnumerable<Client>> GetAllClientsAsync()
        {
            try
            {
                IEnumerable<Client> clients;

                using(var context = ContextFactory.CreateDbContext())
                {
                    clients = await context.Clients.Select(x => new Client()
                    {
                        Name = x.Name,
                        SecondName = x.SecondName,
                        Contacts = x.Contacts,
                        EmailAddress = x.Email,
                        ThirdName = x.ThirdName,
                        Id = x.Id

                    }).ToArrayAsync();  
                }

                return clients;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Не удалось получить информацию о всех клиентах");
                throw;
            }
        }

        public async Task<Client> GetClientAsync(string email)
        {
            try
            {
                using(var context = ContextFactory.CreateDbContext())
                {
                    var dbClient = await GetDbClientAsync(email, context);

                    if(dbClient == null)
                    {
                        return null;
                    }
                    else
                    {
                        return new Client()
                        {
                            Name = dbClient.Name,
                            SecondName = dbClient.SecondName,
                            EmailAddress = dbClient.Email,
                            Contacts = dbClient.Contacts
                        };
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.Error(ex, $"Не удалось плучить информацию о клиенте {email ?? "[null]"}");
                throw;
            }
        }

        public async Task<Client> GetClientAsync(long id)
        {
            try
            {
                using(var context = ContextFactory.CreateDbContext())
                {
                    var client = await context.Clients.FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);

                    if(client == null)
                    {
                        throw new ClientNotFoundException(id.ToString());
                    }

                    return client.ToObjectModel();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Не удалось получить клиента {id}");
                throw;
            }
        }

        public async Task<IEnumerable<Order>> GetClientOrders(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    throw new ArgumentException(nameof(email));
                }

                using(var context = ContextFactory.CreateDbContext())
                {
                    var client = await context.Clients
                        .Include(x => x.Orders)
                        .FirstOrDefaultAsync(x => x.Email == email);

                    if(client == null)
                    {
                        throw new ClientNotFoundException(email);
                    }

                    return client.Orders.Select(x => new Order()
                    {
                        Status = x.Status,
                        ArrivalTime = x.ArrivalTime,
                        Client = new Client()
                        {
                            EmailAddress = client.Email,
                            Name = client.Name,
                            SecondName = client.SecondName,
                            Contacts = client.Contacts,
                            ThirdName = client.ThirdName
                        },
                        Description = "[добавить в бд поле описания заказа]",
                        FinishTime = x.FinishTime,
                        Tags = x.Tags
                    });
                }


            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Не удалось получить заказы клиента {email ?? "[null]"}");
                throw;
            }
        }

        public async Task RemoveClientAsync(string email)
        {
            try
            {
                using(var context = ContextFactory.CreateDbContext())
                {
                    var toRemove = await GetDbClientAsync(email, context);

                    if(toRemove != null)
                    {
                        context.Clients.Remove(toRemove);

                        context.Entry(toRemove).State = EntityState.Deleted;

                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        throw new ClientNotFoundException(email);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Не удалось удалить клиента {email ?? "[null]"}");
                throw;
            }
        }
    }
}
