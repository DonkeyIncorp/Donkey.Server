using Microsoft.EntityFrameworkCore;
using ObjectSystem;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NLog;
using System.Linq;
using Donkey.Administration.Data.Extensions;
using Donkey.Administration.Environment;

namespace Donkey.Administration.Data
{
    internal class OrderDataAccess : DataAccess, IOrderDataAccess
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public OrderDataAccess(IDbContextFactory<DataContext> contextFactory) : base(contextFactory)
        {

        }

        public async Task AddOrderAsync(Order order, string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email));
            }
            if (string.IsNullOrEmpty(order.Guid))
            {
                throw new ArgumentNullException(nameof(order.Guid));
            }

            try
            {
               
                using(var context = ContextFactory.CreateDbContext())
                {
                    var client = await context.Clients.FirstOrDefaultAsync(x => x.Email == email).ConfigureAwait(false);

                    if(client == null)
                    {
                        throw new ClientNotFoundException(email);
                    }

                    bool orderExists = await context.Orders
                        .Include(x => x.Client)
                        .AnyAsync(x => x.Client.Email == email && x.Name == order.Name)
                        .ConfigureAwait(false);

                    if (orderExists)
                    {
                        throw new OrderExistsException(email, client.Email);
                    }

                    var lastClientOrder = await context.Orders
                        .Include(x => x.Client)
                        .Where(x => x.Client.Email == client.Email)
                        .OrderByDescending(x => x.ArrivalTime)
                        .FirstOrDefaultAsync();

                    if(lastClientOrder != null)
                    {
                        var delta = DateTime.Now - lastClientOrder.ArrivalTime;

                        if (delta < TimeSpan.FromMinutes(10))
                        {
                            throw new OrderLimitException();
                        }
                    }
                    

                    var toAddOrder = order.ToDbModel();

                    toAddOrder.Client = client;

                    await context.Orders.AddAsync(toAddOrder);

                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Не удалось добавить заказ #{order.Guid}");
                throw;
            }
        }

        public async Task<IEnumerable<Order>> GetActualOrdersAsync()
        {
            try
            {
                IEnumerable<Order> orders;

                using (var context = ContextFactory.CreateDbContext())
                {
                    orders = await context.Orders
                        .Include(x => x.Client)
                        .Where(x => x.Status == OrderStatus.Created || x.Status == OrderStatus.Delay || x.Status == OrderStatus.Developping)
                        .Select(x => x.ToObjectModel())
                        .ToListAsync()
                        .ConfigureAwait(false);
                }

                return orders;
            }
            catch(Exception ex)
            {
                _logger.Error(ex, "Не удалось получить информацию об актуальных заказах");
                throw;
            }
        }

        public async Task<IEnumerable<Order>> GetAllClientOrdersAsync(string email)
        {
            if(email == null)
            {
                throw new ArgumentNullException(nameof(email));
            }
            try
            {
                IEnumerable<Order> orders;

                using(var context = ContextFactory.CreateDbContext())
                {
                    orders = await context.Orders
                        .Include(x => x.Client)
                        .Where(x => x.Client.Email == email)
                        .Select(x => x.ToObjectModel())
                        .ToListAsync()
                        .ConfigureAwait(false);
                }

                return orders;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Не удалось получить информацию о всех заказах клиента {email}");
                throw;
            }
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            try
            {
                IEnumerable<Order> orders;

                using (var context = ContextFactory.CreateDbContext())
                {
                    var dbOrders = await context.Orders
                        .Include(x => x.Client)
                        .Include(x => x.LastUpdateAdmin)
                        .ToArrayAsync()
                        .ConfigureAwait(false);

                    orders = dbOrders.Select(x => x.ToObjectModel());
                }

                return orders;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Не удалось получить информацию о всех заказах");
                throw;
            }
        }

        public async Task<Order> GetOrderFullInfoAsync(string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                throw new ArgumentNullException(nameof(guid));
            }

            try
            {
                using(var context = ContextFactory.CreateDbContext())
                {
                    var order = await context.Orders.Include(x => x.Client).FirstOrDefaultAsync(x => x.Guid == guid).ConfigureAwait(false);

                    if(order == null)
                    {
                        throw new ArgumentException($"Не удалось найти заказ №{guid}");
                    }

                    return order.ToObjectModel();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Не удалось получить информацию о заказе {guid}");
                throw;
            }
        }

        public async Task RemoveOrderAsync(string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                throw new ArgumentNullException(nameof(guid));
            }

            try
            {
                using (var context = ContextFactory.CreateDbContext())
                {
                    var order = await context.Orders.FirstOrDefaultAsync(x => x.Guid == guid).ConfigureAwait(false);

                    if(order == null)
                    {
                        throw new ArgumentException($"Не удалось найти заказ {guid} для удаления");
                    }

                    context.Orders.Remove(order);
                    context.Entry(order).State = EntityState.Deleted;

                    await context.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Не удалось удалить заказ {guid}");
                throw;
            }
        }

        public async Task UpdateOrderAsync(Order updatedOrder, string updaterLogin)
        {
            if(updatedOrder == null)
            {
                throw new ArgumentNullException(nameof(updatedOrder));
            }

            if (string.IsNullOrEmpty(updatedOrder.Guid))
            {
                throw new ArgumentException("У обновляегмого заказа не указан идентификатор");
            }

            if (string.IsNullOrEmpty(updaterLogin))
            {
                throw new ArgumentNullException(nameof(updaterLogin));
            }

            try
            {
                using(var context = ContextFactory.CreateDbContext())
                {
                    var dbOrder = await context.Orders.FirstOrDefaultAsync(x => x.Guid == updatedOrder.Guid).ConfigureAwait(false);

                    if(dbOrder == null)
                    {
                        throw new ArgumentException($"Не удалось найти заказ {updatedOrder.Guid} для обновления");
                    }

                    var admin = await context.Admins.FirstOrDefaultAsync(x => x.Username == updaterLogin).ConfigureAwait(false);

                    if(admin == null)
                    {
                        throw new ClientNotFoundException(updaterLogin);
                    }

                    //bool clientExists = await context.Clients.AnyAsync(x => x.Id == updatedOrder.Client.Id).ConfigureAwait(false);

                    //if (!clientExists)
                    //{
                    //    throw new ClientNotFoundException(updatedOrder.Client.Id.ToString());
                    //}

                    dbOrder.ArrivalTime = updatedOrder.ArrivalTime;
                    //dbOrder.Client = updatedOrder.Client.ToDbModel();
                    dbOrder.FinishTime = updatedOrder.FinishTime;
                    dbOrder.Status = updatedOrder.Status;
                    dbOrder.Tags = updatedOrder.Tags.ToArray();
                    dbOrder.LastUpdateAdmin = admin;
                    dbOrder.LastUpdateTime = DateTime.Now;
                    dbOrder.Name = updatedOrder.Name;
                    dbOrder.PercentCompleted = updatedOrder.PercentCompleted;
                    dbOrder.Description = updatedOrder.Description;
                    dbOrder.Resources = updatedOrder.Resources;
                    

                    context.Entry(dbOrder).State = EntityState.Modified;

                    await context.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Не удалось обновить заказ {updatedOrder.Guid}");
                throw;
            }
        }
    }
}
