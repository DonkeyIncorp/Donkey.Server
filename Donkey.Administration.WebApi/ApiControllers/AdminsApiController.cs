using Donkey.Administration.BusinessLogic;
using Donkey.Administration.Environment;
using Microsoft.AspNetCore.Mvc;
using NLog;
using ObjectSystem;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Cors;
using Swashbuckle.Swagger.Annotations;

namespace Donkey.Administration.WebApi
{
    /// <summary>
    /// admins contoller
    /// </summary>
    [ApiController]
    [Route("api/admins")]
    public class AdminsApiController : ControllerBase
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IAdminKeyManager _keyManager;
        private readonly IClientManager _clientManager;
        private readonly IOrderManager _orderManager;

        public AdminsApiController(IAdminKeyManager adminKeyManager, IClientManager clientManager, IOrderManager orderManager)
        {
            _keyManager = adminKeyManager;
            _clientManager = clientManager;
            _orderManager = orderManager;
        }

        /// <summary>
        /// Возвращает коллекцию клиентов, представленных в системе Также можно передать query parameter id={id} для получение одного конкретного заказа
        /// </summary>
        /// <param name="authKey">Ключ API, получаемый ранее</param>
        /// <returns></returns>
        [HttpGet("client")]
        [EnableCors("cors")]
        [SwaggerResponse(200, "Успешно")]
        [SwaggerResponse(400, "Некорректный запрос")]
        [SwaggerResponse(403, "Некорректный ключ API")]
        [SwaggerResponse(500, "Внутренняя ошибка")]
        public async Task<ActionResult<IEnumerable<Client>>> GetAllClientsAsync([FromHeader] string authKey)
        {
            // проверка парсинга - 400
            if (string.IsNullOrEmpty(authKey))
            {
                return BadRequest(ResponseData.NegativeResponse());
            }

            try
            {
                if (HttpContext.Request.Query.ContainsKey("id"))
                {
                    var idQuery = Request.Query["id"].First();
                    long id;

                    if(!long.TryParse(idQuery, out id))
                    {
                        return BadRequest(ResponseData.NegativeResponse());
                    }
                    var clientRequestResult = await GetClientFullInfoAsync(id, authKey).ConfigureAwait(false);

                    return clientRequestResult.Result;
                }

                bool validated = await _keyManager.KeyValidAsync(authKey).ConfigureAwait(false); // 403

                if (!validated)
                {
                    return StatusCode(403, ResponseData.NegativeResponse("Некорректный ключ")); // русня месандж
                }
                else
                {
                    var clients = await _clientManager.GetAllClientsInfoAsync().ConfigureAwait(false);

                    var data = new { Clients = clients };

                    return Ok(ResponseData.PositiveResponse(data));
                }
            }
            catch (ArgumentNullException)
            {
                return BadRequest(ResponseData.NegativeResponse());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Не удалось обработать запрос на получение всех клиентов");
                return StatusCode(500, ResponseData.NegativeResponse("Внутренняя ошибка")); // 500
            }
        }

        [NonAction]
        private async Task<ActionResult<Client>> GetClientFullInfoAsync(long id, string authKey)
        {
            try
            {
                var validated = await _keyManager.KeyValidAsync(authKey).ConfigureAwait(false);

                if (!validated)
                {
                    return StatusCode(403, ResponseData.NegativeResponse("Некорректный ключ")); //
                }
                else
                {
                    var clientInfo = await _clientManager.GetClientInfoAsync(id).ConfigureAwait(false);

                    var orders = await _clientManager.GetClientOrdersAsync(clientInfo.EmailAddress).ConfigureAwait(false);

                    var data = new
                    {
                        Client = clientInfo,
                        Orders = orders
                    };

                    return Ok(ResponseData.PositiveResponse(data));
                }
            }
            catch(ClientNotFoundException ex)
            {
                _logger.Error(ex, $"Не удалось найти клиента {id}");

                return StatusCode(500, ResponseData.NegativeResponse($"Клиент №{id} не найден"));
            }
            catch (ArgumentNullException)
            {
                return BadRequest(ResponseData.NegativeResponse());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Не удалось обработать запрос на получение всех клиентов");

                return StatusCode(500, ResponseData.NegativeResponse("Внутренняя ошибка"));
            }
        }

        /// <summary>
        /// Получает все заказы, представленные в системе. Также можно передать query parameter guid={guid} для получение одного конкретного заказа
        /// </summary>
        /// <param name="authKey"></param>
        /// <returns></returns>
        [HttpGet("order")] // + query parameters get
        [EnableCors("cors")]
        [SwaggerResponse(200, "Успешно")]
        [SwaggerResponse(400, "Некорректный запрос")]
        [SwaggerResponse(403, "Некорректный ключ API")]
        [SwaggerResponse(500, "Внутренняя ошибка")]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders([FromHeader] string authKey)
        {

            if (string.IsNullOrEmpty(authKey))
            {
                return BadRequest(ResponseData.NegativeResponse());
            }

            try
            {
                if (Request.Query.ContainsKey("guid"))
                {
                    var response = await GetOrder(Request.Query["guid"].First(), authKey).ConfigureAwait(false);

                    return response.Result;
                }
                bool valid = await _keyManager.KeyValidAsync(authKey).ConfigureAwait(false);

                if (!valid)
                {
                    return StatusCode(403, ResponseData.NegativeResponse("Некорректный ключ"));
                }
                else
                {
                    var orders = await _orderManager.GetAllOrdersAsync().ConfigureAwait(false);

                    var data = new
                    {
                        Orders = orders
                    };

                    return Ok(ResponseData.PositiveResponse(data));
                }
            }
            catch (ArgumentNullException)
            {
                return BadRequest(ResponseData.NegativeResponse());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Не удалось получить информацию обо всех заказах");

                return StatusCode(500, ResponseData.NegativeResponse("Внутрнняя ошибка")); // 500
            }
        }

        [NonAction]
        private async Task<ActionResult<Order>> GetOrder(string guid, string authKey)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return BadRequest(ResponseData.NegativeResponse());
            }

            try
            {
                bool valid = await _keyManager.KeyValidAsync(authKey).ConfigureAwait(false);

                if (!valid)
                {
                    return StatusCode(403, ResponseData.NegativeResponse("Некорректный ключ"));
                }

                var order = await _orderManager.GetOrderFullInfoAsync(guid).ConfigureAwait(false);

                var data = new
                {
                    Order = order
                };

                return Ok(ResponseData.PositiveResponse(data));
            }
            catch (ArgumentNullException)
            {
                return BadRequest(ResponseData.NegativeResponse());
            }
            catch (ArgumentException ex)
            {
                _logger.Error(ex, "Не удалось обработать запрос на получение заказа");

                return StatusCode(500, ResponseData.NegativeResponse($"Не найден заказ №{guid}"));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Не удалось получить информацю об одном заказе");
                return StatusCode(500, ResponseData.NegativeResponse("Внутренняя ошибка"));
            }
        }

        /// <summary>
        /// Редактирует переданный заказ
        /// </summary>
        /// <param name="authKey">Ключ API, получаемый выше</param>
        /// <param name="updatedOrder">Обновленый объект заказа</param>
        /// <returns></returns>
        [HttpPut("order")]
        [EnableCors("cors")]
        [SwaggerResponse(200, "Успешно")]
        [SwaggerResponse(400, "Некорректный запрос")]
        [SwaggerResponse(403, "Некорректный ключ API")]
        [SwaggerResponse(500, "Внутренняя ошибка")]
        public async Task<ActionResult> UpdateOrderAsync([FromHeader] string authKey, [FromBody] UpdateOrderModel updatedOrder)
        {
            if (updatedOrder == null)
            {
                throw new ArgumentNullException(nameof(updatedOrder));
            }

            if (updatedOrder.Guid == null)
            {
                return BadRequest(ResponseData.NegativeResponse());
            }

            if (string.IsNullOrEmpty(authKey))
            {
                return BadRequest(ResponseData.NegativeResponse());
            }

            try
            {
                bool valid = await _keyManager.KeyValidAsync(authKey).ConfigureAwait(false);

                if (!valid)
                {
                    return StatusCode(403, ResponseData.NegativeResponse("Некорректный ключ"));
                }

                
                var updater = await _keyManager.GetLoginAsync(authKey).ConfigureAwait(false);

                await _orderManager.UpdateOrderAsync(updatedOrder.GetOrder(), updater).ConfigureAwait(false);
                return Ok(ResponseData.PositiveResponse(null, "Заказ успешно обновлен"));
                
            }
            catch (ArgumentNullException)
            {
                return BadRequest(ResponseData.NegativeResponse());
            }
            catch (ArgumentException ex)
            {
                _logger.Error(ex, "Не удалось обновить заказ");

                return StatusCode(500, ResponseData.NegativeResponse("Не найден заказ для обновления"));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Не удалось обновить заказ");
                return StatusCode(500, ResponseData.NegativeResponse("Внутренняя ошибка")); //500
            }
        }
       
    }
}
