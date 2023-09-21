using Donkey.Administration.Data;
using NLog;
using ObjectSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Donkey.Administration.BusinessLogic
{
    internal class OrderManager : IOrderManager
    {
        public event Func<OrderAddedEventArgs, Task> OrderAdded
        {
            add
            {
                lock (_addedHandlers)
                {
                    _addedHandlers.Add(value);
                }
            }
            remove
            {
                lock (_addedHandlers)
                {
                    _addedHandlers.Remove(value);
                }
            }
        }


        public event Action<OrderAddedEventArgs> OrderAdding;
        public event Action<OrderRemovedEventArgs> OrderRemoved;
        public event Action<OrderRemovedEventArgs> OrderRemoving;
        public event Action<OrderStatusChangedEventArgs> OrderStatusChanged;

        private readonly IOrderDataAccess _orderDataAccess;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly HashSet<Func<OrderAddedEventArgs, Task>> _addedHandlers;
        public OrderManager(IOrderDataAccess orderDataAccess)
        {
            _orderDataAccess = orderDataAccess;
            _addedHandlers = new HashSet<Func<OrderAddedEventArgs, Task>>();

        }
        public async Task AddOrderAsync(Order order, string email)
        {
            try
            {
                OrderAdding?.Invoke(new OrderAddedEventArgs(order));

                await _orderDataAccess.AddOrderAsync(order, email).ConfigureAwait(false);

                IEnumerable<Task> handlers;

                lock (_addedHandlers)
                {
                    handlers = _addedHandlers.Select(async handler =>
                    {
                        try
                        {
                            await handler(new OrderAddedEventArgs(order)).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex, "Обработчик события добавления заказа вызвал исключение");
                        }
                    });
                }

                await Task.WhenAll(handlers).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Не удалось добавить заказ {order.Description}"); // fix orderID
                throw;
            }
        }

        public Task ChangeOrderStatusAsync(string orderId, OrderStatus newStatus)
        {
            try
            {
                return Task.CompletedTask; // orderStatus
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Не удалось изменить статус заказа {orderId}");
                throw;
            }
        }

        public async Task RemoveOrderAsync(string orderId)
        {
            try
            {
                OrderRemoving?.Invoke(new OrderRemovedEventArgs(null));

                await _orderDataAccess.RemoveOrderAsync(orderId).ConfigureAwait(false);

                OrderRemoved?.Invoke(new OrderRemovedEventArgs(null));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Не удалось удалить заказ {orderId}");
                throw;
            }
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            try
            {
                var orders = await _orderDataAccess.GetAllOrdersAsync();

                return orders;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Не удалось получить информацию обо всех заказах");
                throw;
            }
        }

        public async Task UpdateOrderAsync(Order updatedOrder, string updaterLogin)
        {
            try
            {
                await _orderDataAccess.UpdateOrderAsync(updatedOrder, updaterLogin).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Не удалось обновить заказ {updatedOrder.Guid}");
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
                return await _orderDataAccess.GetOrderFullInfoAsync(guid).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Не удалосб получить информацию о заказе {guid}");
                throw;
            }
        }
    }
}
