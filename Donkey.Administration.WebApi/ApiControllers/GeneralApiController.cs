using Donkey.Administration.BusinessLogic;
using Donkey.Administration.Environment;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using NLog;
using ObjectSystem;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Donkey.Administration.WebApi.ApiControllers
{
    /// <summary>
    /// general api controller
    /// </summary>
    [ApiController]
    [Route("api/general")]
    public class GeneralApiController : ControllerBase
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IGeneralKeyManager _keyManager;
        private readonly IClientManager _clientManager;
        private readonly IOrderManager _orderManager;

        public GeneralApiController(IGeneralKeyManager keyManager, IClientManager clientManager, IOrderManager orderManager)
        {
            _keyManager = keyManager;
            _clientManager = clientManager;
            _orderManager = orderManager;
        }

        /// <summary>
        /// Добавляет указанного клиента
        /// </summary>
        /// <param name="apiKey">Ключ API</param>
        /// <param name="client">Добавляемый клиент</param>
        /// <returns></returns>
        [HttpPost("client")]
        [EnableCors("cors")]
        [SwaggerResponse(200, "Успешно")]
        [SwaggerResponse(400, "Некорректный запрос")]
        [SwaggerResponse(403, "Некорректный ключ API")]
        [SwaggerResponse(500, "Внутренняя ошибка")]
        public async Task<ActionResult> AddClientAsync([FromHeader] string apiKey, [FromBody] AddClientModel client)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                return BadRequest(ResponseData.NegativeResponse());
            }

            try
            {
                var validated = await _keyManager.KeyValidAsync(apiKey).ConfigureAwait(false);

                if (!validated)
                {
                    return StatusCode(403, ResponseData.NegativeResponse("Некорректный ключ"));
                }

                else
                {
                    bool clientExists = await _clientManager.ClientExistsAsync(client.Email).ConfigureAwait(false);

                    if (clientExists)
                    {
                        return StatusCode(500, ResponseData.NegativeResponse("Такая почта уже зарегистрирована в системе")); // 500 Такая почта уже зарегистрирована в системе
                    }

                    await _clientManager.AddClientAsync(client.GetClient()).ConfigureAwait(false);

                    return Ok(ResponseData.PositiveResponse(null, "Клиент успешно добавлен"));
                }
            }
            catch(ArgumentException ex)
            {
                string message = "Некорректный формат запроса";
                _logger.Error(ex, message);
                return BadRequest(ResponseData.NegativeResponse(message));
            }

            catch (Exception ex)
            {
                _logger.Error(ex, "Не удалось обработать запрос на добавление клиента");
                return StatusCode(500, ResponseData.NegativeResponse("Внутренняя ошибка"));
            }

        }

        /// <summary>
        /// Добавляет переданный заказ
        /// </summary>
        /// <param name="apiKey">Ключ API</param>
        /// <param name="addModel">Добавляемый заказ и email заказчика</param>
        /// <returns></returns>
        [HttpPost("order")]
        [EnableCors("conrs")]
        [SwaggerResponse(200, "Успешно")]
        [SwaggerResponse(400, "Некорректный запрос")]
        [SwaggerResponse(403, "Некорректный ключ API")]
        [SwaggerResponse(500, "Внутренняя ошибка")]
        public async Task<ActionResult> AddOrderAsync([FromHeader] string apiKey, [FromBody] AddOrderModel addModel)
        {
            if (addModel == null)
            {
                return BadRequest(ResponseData.NegativeResponse());
            }

            if (string.IsNullOrEmpty(apiKey))
            {
                return BadRequest(ResponseData.NegativeResponse());
            }

            if (!addModel.IsCorrect)
            {
                return BadRequest(ResponseData.NegativeResponse());
            }

            try
            {
                bool valid = await _keyManager.KeyValidAsync(apiKey).ConfigureAwait(false);

                if (!valid)
                {
                    return StatusCode(403, ResponseData.NegativeResponse("Некорректный ключ"));
                }
                else
                {
                    
                    bool clientExists = await _clientManager.ClientExistsAsync(addModel.Email).ConfigureAwait(false);

                    if (!clientExists)
                    {
                        return StatusCode(500, ResponseData.NegativeResponse("Данный email не существует в системе"));
                    }

                    var order = addModel.GetOrder();

                    order.Guid = Guid.NewGuid().ToString();
                    order.ArrivalTime = DateTime.Now;

                    await _orderManager.AddOrderAsync(order, addModel.Email).ConfigureAwait(false);

                    return Ok(ResponseData.PositiveResponse(null, "Заказ успешно добавлен"));
                }
            }
            catch (OrderExistsException)
            {
                return StatusCode(500, ResponseData.NegativeResponse($"Заказ {addModel.Name} для клиента {addModel.Email} уже существует в системе"));
            }
            catch (ArgumentNullException)
            {
                return BadRequest(ResponseData.NegativeResponse());
            }
            catch (OrderLimitException)
            {
                return StatusCode(500, ResponseData.NegativeResponse("Вы уже недавно подавали заявку"));
            }
            catch (Exception ex)
            {
                string message = $"Не удалось добавить заказ";
                _logger.Error(ex, message);

                return StatusCode(500, ResponseData.NegativeResponse("Внутрення ошибка"));
            }
        }
    }
}
