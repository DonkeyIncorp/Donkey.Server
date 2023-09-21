using Newtonsoft.Json.Linq;
using System;
using Donkey.Administration.Environment;
using System.Collections.Generic;
using System.Text;
using NLog;

namespace Donkey.Administration.WebApi
{
    public class Request
    {
        private JObject _parsed;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public string Key { get; set; }

        public object Data { get; set; }

        public string GetValue(string key)
        {
            if (Data == null)
            {
                throw new ArgumentNullException();
            }

            if (_parsed == null)
            {
                string parsing = Data.ToString();

                _parsed = JObject.Parse(parsing);
            }

            if (_parsed.ContainsKey(key))
            {
                return _parsed[key].ToString();
            }
            else
            {
                throw new ArgumentException($"Требуемый ключ ({key}) не найден");
            }
        }

        public bool TryGetValue<T>(string key, out T value)
            where T: class
        {
            if(Data == null)
            {
                throw new ArgumentNullException();
            }
            try
            {
                if (_parsed == null)
                {
                    string parsing = Data.ToString();
                    _parsed = JObject.Parse(parsing);
                }

                if (_parsed.ContainsKey(key))
                {
                    if (typeof(T) == typeof(string))
                    {
                        value = _parsed[key].ToString() as T;
                        return true;
                    }
                    value = Serialization.Deserialize<T>(_parsed[key].ToString());
                    return true;
                }
                else
                {
                    value = default;
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ошибка парсинга запроса");

                value = null;
                return false;
            }
        }
    }
}
