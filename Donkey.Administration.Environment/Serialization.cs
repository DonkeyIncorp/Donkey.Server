using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Donkey.Administration.Environment
{
    public static class Serialization
    {
        public static string Serialize(object toSerialize)
        {
            return JsonConvert.SerializeObject(toSerialize);
        }

        public static T Deserialize<T>(string toDeserialize)
        {
            var datetimeConverter = new IsoDateTimeConverter()
            {
                DateTimeFormat = "dd.MM.yyyy HH:mm"
            };

            return JsonConvert.DeserializeObject<T>(toDeserialize, datetimeConverter);
        }
    }
}
