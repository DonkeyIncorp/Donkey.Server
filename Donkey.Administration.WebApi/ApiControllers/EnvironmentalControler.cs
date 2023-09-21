using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using NLog;
using System;
using System.Threading.Tasks;
using System.IO;
using Donkey.Administration.Data;
using Donkey.Administration.Environment;
using Microsoft.AspNetCore.Cors;
using Swashbuckle.Swagger.Annotations;

namespace Donkey.Administration.WebApi
{
    /// <summary> 
    /// Env controler
    /// </summary>
    [ApiController]
    [Route("api/environment")]
    public class EnvironmentalController : ControllerBase
    {
        private readonly IHashEncoder _hashEncoder;
        private readonly IGeneralKeyManager _generalKeyManager;
        private readonly IAdminKeyManager _adminKeyManager;
        private readonly IAdminDataAccess _adminDataAccess;

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public EnvironmentalController(
            IHashEncoder hashEncoder,
            IGeneralKeyManager generalKeyManager,
            IAdminKeyManager adminKeyManager,
            IAdminDataAccess adminDataAccess)
        {
            _generalKeyManager = generalKeyManager;
            _adminKeyManager = adminKeyManager;
            _hashEncoder = hashEncoder;
            _adminDataAccess = adminDataAccess;
        }

        /// <summary>
        /// Возвращает хэш переданной строки
        /// </summary>
        /// <param name="data">Строка, которую требуется захэшировать</param>
        /// <returns></returns>
        [HttpPost("hash")]
        [EnableCors("cors")]
        [SwaggerResponse(200, "Успешно")]
        [SwaggerResponse(400, "Некорректный запрос")]
        [SwaggerResponse(500, "Внутренняя ошибка")]
        public ActionResult<ResponseData> GetHash([FromBody] string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return BadRequest(ResponseData.NegativeResponse());
            }

            try
            {
                var hash = _hashEncoder.GetHash(data);

                var res = new { Hash = hash };

                return Ok(ResponseData.PositiveResponse(res));
            }
            catch (ArgumentNullException)
            {
                return BadRequest(ResponseData.NegativeResponse());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка хэширования");

                return StatusCode(500, ResponseData.NegativeResponse("Ошибка хэширования"));
            }
        }

        /// <summary>
        /// Получает серверное время
        /// </summary>
        /// <returns></returns>
        [HttpGet("time")]
        [EnableCors("cors")]
        [SwaggerResponse(200, "Успешно")]
        [SwaggerResponse(400, "Некорректный запрос")]
        [SwaggerResponse(500, "Внутренняя ошибка")]
        public ActionResult<ResponseData> GetServerTime()
        {
            try
            {
                var result = DateTime.Now.ToString("dd.MM.yyyy HH:mm");

                var data = new
                {
                    Time = result
                };

                return Ok(ResponseData.PositiveResponse(data));
            }
            catch (ArgumentNullException)
            {
                return BadRequest(ResponseData.NegativeResponse());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Не удалось получить серверное время");

                return StatusCode(500, ResponseData.NegativeResponse("Внутрення ошибка"));
            }
            
        }

        /// <summary>
        /// Получить ключ API
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        [HttpPost("key")]
        [EnableCors("cors")]
        [SwaggerResponse(200, "Успешно")]
        [SwaggerResponse(400, "Некорректный запрос")]
        [SwaggerResponse(500, "Внутренняя ошибка")]
        public async Task<ActionResult<ResponseData>> GetKeyAsync([FromBody] GetKeyModel request)
        {
            if(request == null)
            {
                return BadRequest(ResponseData.NegativeResponse());
            }

            if (!request.IsCorrect)
            {
                return BadRequest(ResponseData.NegativeResponse());
            }

            try
            {
                //string receiver = request.GetValue("receiver"); // -> 400
                //string firstKey = request.GetValue("firstKey"); // -> 400

                var resultKey = await _generalKeyManager.GetKeyAsync(request.Receiver, request.FirstKey); // -> 500 

                // {message: "Невалидные данные"}
                var data = new { Key = resultKey };

                return Ok(ResponseData.PositiveResponse(data));
            }
            catch (ArgumentException ex)
            {
                _logger.Error(ex, "Ошибка преобразования запроса");

                return BadRequest(ResponseData.NegativeResponse());
            }
            catch (InvalidOperationException ex)
            {
                _logger.Error(ex, "Ошибка проведения операции");

                return StatusCode(500, ResponseData.NegativeResponse("Невалидные данные"));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Не удалось обработать запрос на получение ключа");

                return StatusCode(500, ResponseData.NegativeResponse("Внутренняя ошибка"));
            }
        }

        /// <summary>
        /// Получить ключ API для администраторских операций
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("auth")]
        [EnableCors("cors")]
        [SwaggerResponse(200, "Успешно")]
        [SwaggerResponse(400, "Некорректный запрос")]
        [SwaggerResponse(403, "Некорректный ключ API")]
        [SwaggerResponse(500, "Внутренняя ошибка")]
        public async Task<ActionResult<ResponseData>> AuthAsync([FromHeader] string apiKey, [FromBody] AuthModel request)
        {
            if (request == null)
            {
                return BadRequest(ResponseData.NegativeResponse());
            }

            if (!request.IsCorrect)
            {
                return BadRequest(ResponseData.NegativeResponse());
            }

            //string username = null;

            try
            {
                bool valid = await _generalKeyManager.KeyValidAsync(apiKey).ConfigureAwait(false);

                if (!valid)
                {
                    return StatusCode(403, ResponseData.NegativeResponse("Некорректный ключ"));
                }

                //username = request.GetValue("username"); // 400
                //string passwordHash = request.GetValue("passwordHash"); // 400


                bool success = await _adminDataAccess.ValidateAuthDataAsync(request.Username, request.PasswordHash).ConfigureAwait(false); 

                // "{message : "Неверные логин или пароль"}"

                if (!success)
                {
                    return StatusCode(500, ResponseData.NegativeResponse("Неверные логин или пароль")); // 500
                }

                else
                {
                    var key = await _adminKeyManager.GetKeyAsync(request.Username, request.PasswordHash).ConfigureAwait(false);

                    var data = new { Key = key };

                    return Ok(ResponseData.PositiveResponse(data));
                }
            }
            catch(ArgumentException ex)
            {
                _logger.Error(ex, "Некорректный формат запроса");
                return BadRequest(ResponseData.NegativeResponse());
            }
            
            catch (ClientNotFoundException ex)
            {
                _logger.Error(ex, $"Ошибка поиска клиента");
                return BadRequest(ResponseData.NegativeResponse("Неверные логин или пароль"));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Не удалось провести авторизацию {request.Username ?? "[имя не задано]"}");
                return StatusCode(500, ResponseData.NegativeResponse("Внутренняя ошибка")); // 500 внутренняя ошибка
            }
        }
    }
}
